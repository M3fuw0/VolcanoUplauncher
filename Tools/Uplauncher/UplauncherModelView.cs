#region License GNU GPL
// UplauncherModelView.cs
// 
// Copyright (C) 2013 - BehaviorIsManaged
// 
// This program is free software; you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the Free Software Foundation;
// either version 2 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details. 
// You should have received a copy of the GNU General Public License along with this program; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml.Linq;
using Uplauncher.Helpers;
using Uplauncher.Patcher;
using Uplauncher.Properties;
using Uplauncher.Sound;
using Uplauncher.Sound.UplSound;
using Uplauncher.Utils;
using Application = System.Windows.Forms.Application;
using Clipboard = System.Windows.Forms.Clipboard;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Uplauncher
{
    public class UplauncherModelView : INotifyPropertyChanged
    {
        private WebClient m_client = new WebClient();
        private readonly SoundProxy m_soundProxy = new SoundProxy();
        private Stack<MetaFileEntry> m_currentTasks;
        private readonly DateTime? m_lastUpdateCheck;
        private static readonly Color DefaultMessageColor = Colors.Black;

        private readonly FileSizeFormatProvider m_bytesFormatProvider = new FileSizeFormatProvider();
        private MetaFile m_metaFile;

        private UpServer server = new UpServer(); //18/12/2020
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly BackgroundWorker m_MD5Worker = new BackgroundWorker();
        private MainWindow _mainWindow;

        public UplauncherModelView(DateTime? lastUpdateCheck, MainWindow mainWindow)
        {
            m_lastUpdateCheck = lastUpdateCheck;
            _mainWindow = mainWindow;
            NotifyIcon = new NotifyIcon
                {
                    Visible = true,
                    Icon = Resources.dofus_icon_48,
                    ContextMenu = new ContextMenu(new[]
                    { 
                        new MenuItem("Ouvrir", OnTrayClickShow),
                        new MenuItem("Lancer le Jeu", OnTrayClickGame),
                        new MenuItem("Discord", OnTrayClickVote),
                        new MenuItem("Quitter", OnTrayClickExit)
                    })
                };

            NotifyIcon.DoubleClick += OnTrayDoubleClick;

            //Task.Factory.StartNew(CheckVoteTiming);
        }

        public WebClient WebClient => m_client;


        //#region PlayCommand

        private DelegateCommand m_playCommand;

        public DelegateCommand PlayCommand => m_playCommand ?? (m_playCommand = new DelegateCommand(OnPlay, CanPlay));

	    private bool CanPlay(object parameter)
        {
            return !IsUpdating && IsUpToDate;
        }

        public int NumberOfClientsToStart { get; set; }

        private void OnPlay(object parameter)
        {
            if (!CanPlay(parameter))
            {
                return;
            }

            //int numberOfClientsToStart = (int)parameter;
            int numberOfClientsToStart = NumberOfClientsToStart;

            if (!m_lastUpdateCheck.HasValue || DateTime.Now - m_lastUpdateCheck.Value > TimeSpan.FromMinutes(5.0))
            {
                CheckUpdates();
            }
            if (!File.Exists(Constants.DofusExePath))
            {
                MessageBox.Show(string.Format(Resources.Dofus_Not_Found, Constants.DofusExePath), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            //18/12/2020
            if (!server.IsStart)
            {
                server.StartAuthentificate();
            }      
            Initialization.Init();       //
            if (!m_soundProxy.Started)
            {
                m_soundProxy.StartProxy();
            }
            if (m_soundProxy.Started && (m_regProcess == null || m_regProcess.HasExited))
            {
                StartRegApp();
            }
            IsNumberComboBoxEnabled = true;
            IsLanguageComboBoxEnabled = true;
            IsStatusTextBoxVisible = true;
            for (int i = 0; i < numberOfClientsToStart; i++)
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo(Constants.DofusExePath, m_soundProxy.Started ? ("--reg-client-port=" + m_soundProxy.ClientPort) : string.Empty)
                };

                bool started = process.Start();
                process.WaitForInputIdle();

                if (!started)
                {
                    MessageBox.Show(Resources.Cannot_Start_Dofus, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }

                // Si ce n'est pas le premier client, attendez 5 secondes
                if (i > 1)
                {
                    Thread.Sleep(5000); // Attendre 5 secondes
                }
                string details = i == 1 ? "1 fenêtre de jeu ouverte" : $"{i} fenêtres de jeu ouvertes";
                _mainWindow.UpdateGameDetails("En train de jouer à Pyrasis", details);
            }
            SetState("Jeu lancé.");
            _mainWindow.InitializeProcessCheckTimer(); // Initialiser le timer après les mises à jour
            HideWindowInTrayIcon();
        }

        //public void StartClients(object parameter)
        //{
        //    OnPlay(parameter);
        //}

        public void StartClients(int numberOfClients)
        {
            OnPlay(numberOfClients);
        }

        //private void ClientNumberComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    NumberOfClientsToStart = int.Parse(((ComboBoxItem)ClientNumberComboBox.SelectedItem).Content.ToString());
        //    OnPlay(null);
        //}

        private DelegateCommand _openSelectDofusClientsCommand;

        public DelegateCommand OpenSelectDofusClientsCommand => _openSelectDofusClientsCommand ?? (_openSelectDofusClientsCommand = new DelegateCommand(OpenSelectDofusClients));

        private void OpenSelectDofusClients(object parameter)
        {
            SelectDofusClientsWindow selectWindow = new SelectDofusClientsWindow();
            selectWindow.DataContext = this;
            selectWindow.ShowDialog();
        }

        private void StartRegApp()
        {
            if (!File.Exists(Constants.DofusExePath))
            {
                NotifyIcon.ShowBalloonTip(4000, Constants.ApplicationName, Constants.RegExePath + " est introuvable. Les sons ne seront pas activés", ToolTipIcon.Warning);
                return;
            }

            m_regProcess = new Process
            {
                StartInfo = new ProcessStartInfo(Constants.DofusRegExePath, "--reg-engine-port=" + m_soundProxy.RegPort),
            };

            if (!m_regProcess.Start())
            {
                NotifyIcon.ShowBalloonTip(4000, Constants.ApplicationName, "Impossible de lancer" + Constants.RegExePath + ". Raison inconnue", ToolTipIcon.Warning);
            }
        }

        //#endregion

        //#region VoteCommand

        private DelegateCommand m_voteCommand;
        private Process m_regProcess;

        public DelegateCommand VoteCommand => m_voteCommand ?? (m_voteCommand = new DelegateCommand(OnVote, CanVote));

	    private bool CanVote(object parameter)
        {
            return true;
        }

        private void OnVote(object parameter)
        {
            Process.Start(Constants.VoteURL);
            LastVote = DateTime.Now;
        }

        //#endregion

        //#region SiteCommand

        private DelegateCommand m_siteCommand;

        public DelegateCommand SiteCommand => m_siteCommand ?? (m_siteCommand = new DelegateCommand(OnSite, CanSite));

	    private bool CanSite(object parameter)
        {
            return true;
        }

        private void OnSite(object parameter)
        {
            //if (!CanSite(parameter))
                //return;
                Process.Start(Constants.SiteURL);
        }

        //#endregion

        //#region CloseCommand

        private DelegateCommand m_closeCommand;

        public DelegateCommand CloseCommand => m_closeCommand ?? (m_closeCommand = new DelegateCommand(OnClose, CanClose));

	    private static bool CanClose(object parameter)
        {
            return true;
        }

        private void OnClose(object parameter)
        {
            if (!CanClose(parameter))
                return;

            HideWindowInTrayIcon();
        }

        //#endregion

        //#region RepairGameCommand

        private DelegateCommand m_repairGameCommand;

        public DelegateCommand RepairGameCommand => m_repairGameCommand ?? (m_repairGameCommand = new DelegateCommand(OnRepairGame, CanRepairGame));

	    private bool CanRepairGame(object parameter)
        {
            return !IsUpdating;
        }

        private void OnRepairGame(object parameter)
        {
            if (CanRepairGame(parameter) && !IsUpdating)
            {
                //DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Êtes-vous sur de vouloir réparer le jeu ? Tous les fichiers seront vérifiés puis re-téléchargés si besoin !", "Réparer le jeu", MessageBoxButtons.YesNo);
                var dialogResult = MessageBox.Show(@"Êtes-vous sur de vouloir réparer le jeu? Tous les fichiers seront vérifiés puis re-téléchargés si besoin !", "Réparer le jeu", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    foreach (Process item in from process in Process.GetProcesses()
                        where process.ProcessName == "Dofus"
                        select process)
                    {
                        item.Kill();
                    }
                    File.Delete(Constants.LocalChecksumFile);
                    Process.Start(Constants.PatchPath, "-restart");
                    CheckUpdates();
                    //Process.Start(Application.ExecutablePath);
                    foreach (Process item2 in from process in Process.GetProcesses()
                                              where process.ProcessName == "Uplauncher"
                                              select process)
                    {
                        item2.Kill();
                    }
                }
            }
        }

        //#endregion

        private DelegateCommand m_deleteGameFilesCommand;

        public DelegateCommand DeleteGameFilesCommand => m_deleteGameFilesCommand ?? (m_deleteGameFilesCommand = new DelegateCommand(OnDeleteGameFiles, CanPlay));

        private void OnDeleteGameFiles(object parameter)
        {
            if (CanRepairGame(parameter) && !IsUpdating)
            {
                var dialogResult = MessageBox.Show(@"Êtes-vous sur de vouloir supprimer le jeu ? Tous les fichiers seront supprimés.", "Supprimer le jeu", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    foreach (Process item in from process in Process.GetProcesses()
                                             where process.ProcessName == "Dofus"
                                             select process)
                    {
                        item.Kill();
                    }

                    string gameDirectoryPath = Constants.GameDirPath;
                    string checksumFilePath = Constants.ChecksumFilePath;

                    if (Directory.Exists(gameDirectoryPath))
                    {
                        var dirInfo = new DirectoryInfo(gameDirectoryPath);
                        int totalFiles = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Count();
                        int totalDirs = dirInfo.EnumerateDirectories("*", SearchOption.AllDirectories).Count();

                        int currentFile = 0;

                        foreach (var directory in dirInfo.EnumerateDirectories("*", SearchOption.AllDirectories))
                        {
                            Directory.Delete(directory.FullName, true);
                            currentFile++;
                            SetState($"Suppression du sous-dossier {directory.FullName} ({currentFile} sur {totalFiles + totalDirs})");
                        }

                        foreach (var file in dirInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
                        {
                            File.Delete(file.FullName);
                            currentFile++;
                            SetState($"Suppression du fichier {file.FullName} ({currentFile} sur {totalFiles + totalDirs})");
                        }
                        Directory.Delete(gameDirectoryPath);
                        SetState($"Le dossier de jeu {gameDirectoryPath} a été supprimé.");
                    }

                    if (File.Exists(Constants.LocalChecksumFile))
                    {
                        File.Delete(Constants.LocalChecksumFile);
                        SetState("Le jeu a été supprimé.");
                    }

                    var restartDialogResult = MessageBox.Show("Voulez-vous redémarrer l'uplauncher maintenant ?", "Redémarrer l'uplauncher", MessageBoxButtons.YesNo);
                    if (restartDialogResult == DialogResult.Yes)
                    {
                        Process.Start(Constants.PatchPath, "-restart");
                    }
                }
            }
        }

        //private DelegateCommand m_multiDofusCommand;

        //public DelegateCommand MultiDofusCommand => m_multiDofusCommand ?? (m_multiDofusCommand = new DelegateCommand(OpenMultiDofus, CanPlay));

        //private void OpenMultiDofus(object parameter)
        //{
        //    MyApplication.Main(Array.Empty<string>());
        //}

        //#region ChangeLanguageCommand

        private DelegateCommand m_changeLanguageCommand;

        public DelegateCommand ChangeLanguageCommand => m_changeLanguageCommand ?? (m_changeLanguageCommand = new DelegateCommand(OnChangeLanguage, CanChangeLanguage));

	    private static bool CanChangeLanguage(object parameter)
        {
            return false;
        }

        private static void OnChangeLanguage(object parameter)
        {
            if (parameter == null || !CanChangeLanguage(parameter))
                return;
        }

        //private bool CanConsole(object parameter)
        //{
        //    return !IsUpdating;
        //}

        //private void OnConsole(object parameter)
        //{
        //    if (CanConsole(parameter) && !IsUpdating)
        //    {
        //        MessageBox.Show("Les options sont en cours de développement.", "Pyrasis UpLauncher");
        //    }
        //}

        //#endregion

        //#region TrayIcon

        public void HideWindowInTrayIcon()
        {
            View.Hide();
	        NotifyIcon?.ShowBalloonTip(4000, Constants.ApplicationName, "La fenêtre a été placé dans la barre de notifications.", ToolTipIcon.Info);
        }

        public void HideWindowInTrayIconWinform()
        {
            View.Hide();
	        NotifyIcon?.ShowBalloonTip(4000, Constants.ApplicationName, "La fenêtre a été placé dans la barre de notifications.", ToolTipIcon.Info);
        }

        private void OnTrayClickVote(object sender, EventArgs eventArgs)
        {
            View.Show();
            OnVote(null);
        }

        private void OnTrayClickGame(object sender, EventArgs eventArgs)
        {
            View.Show();
            if (CanPlay(eventArgs))
                OnPlay(eventArgs);
        }

        private void OnTrayClickShow(object sender, EventArgs eventArgs)
        {
            View.Show();
        }

        private void OnTrayDoubleClick(object sender, EventArgs e)
        {
            View.Show();
        }

        private static void OnTrayClickExit(object sender, EventArgs eventArgs)
        {
            System.Windows.Application.Current.Shutdown();
        }

        //#endregion

        //#region Vote Timer

        private void CheckVoteTiming()
        {
            var processStart = Process.GetCurrentProcess().StartTime;
            if (DateTime.Now - processStart > TimeSpan.FromMinutes(3))
            {
                if (LastVote == null || DateTime.Now - LastVote >= TimeSpan.FromHours(2))
                    NotifyIcon.ShowBalloonTip(5000, Constants.ApplicationName, "Vous pouvez à nouveau voter pour le serveur", ToolTipIcon.Warning);
            }
            Thread.Sleep(2 * 60 * 1000);
            Task.Factory.StartNew(CheckVoteTiming);
        }

        //#endregion

        // Assume you have a list of URLs:
        List<string> serverUrls = new List<string> { Constants.UpdateSiteURL, Constants.SecondaryUpdateSiteURL /*, more URLs as needed */ };

        public void CheckUpdates()
        {
            //code
            foreach (Process item in from process in Process.GetProcesses()
                     where process.ProcessName == "Dofus"
                     select process)
            {
                item.Kill();
            }
            foreach (Process item2 in from processReg in Process.GetProcesses()
                where processReg.ProcessName == "Reg"
                select processReg)
            {
                item2.Kill();
            }
            if (IsUpdating)
            {
                return;
            }
            if (VerifyVersion())
            {
                IsUpdating = true;
                m_playCommand.RaiseCanExecuteChanged();
                SetState("Téléchargement des informations...");
                m_client = new WebClient();
                ServicePointManager.SecurityProtocol = (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
                ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
                m_client.DownloadProgressChanged += OnDownloadProgressChanged;
                m_client.DownloadStringCompleted += OnPatchDownloaded;

                // Define the list of server URLs
                List<string> serverUrls = new List<string> { Constants.UpdateSiteURL, Constants.SecondaryUpdateSiteURL /*, more URLs as needed */ };

                foreach (var url in serverUrls)
                {
                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + Constants.RemotePatchFile);
                        request.Timeout = 15000; // Timeout set to 15 seconds

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                m_client = new WebClient();
                                m_client.DownloadProgressChanged += OnDownloadProgressChanged;
                                m_client.DownloadStringCompleted += OnPatchDownloaded;
                                m_client.DownloadStringAsync(new Uri(url + Constants.RemotePatchFile), Constants.RemotePatchFile);
                                break;
                            }
                        }
                    }
                    catch (WebException)
                    {
                        // If an exception occurs, don't do anything. The loop will move on to the next server.
						SetState("Le serveur est indisponible.");
                    }
                }
            }

            else if (Directory.Exists("majs"))
            {
                DownloadUpLauncher(Constants.UplauncherURL, Constants.UplauncherMaj);
            }
            else
            {
                Directory.CreateDirectory("majs");
                DownloadUpLauncher(Constants.UplauncherURL, Constants.UplauncherMaj);
            }
        }

        private void OnPatchDownloaded(object sender, DownloadStringCompletedEventArgs e)
        {
            ProgressDownloadSpeedInfo = string.Empty;
            m_client.DownloadStringCompleted -= OnPatchDownloaded;
            try
            {
                m_metaFile = XmlUtils.Deserialize<MetaFile>(new StringReader(e.Result));

                m_MD5Worker.WorkerReportsProgress = true;
                m_MD5Worker.DoWork += MD5Worker_DoWork;
                m_MD5Worker.ProgressChanged += MD5Worker_ProgressChanged;
                m_MD5Worker.RunWorkerCompleted += MD5Worker_RunWorkerCompleted;

                //18/12/2020
                IsUpdating = true;
                m_playCommand.RaiseCanExecuteChanged();
                m_client = new WebClient();
                ServicePointManager.SecurityProtocol = (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
                m_client.DownloadProgressChanged += OnDownloadProgressChanged;
                m_client.DownloadStringCompleted += OnPatchDownloaded;

                // if a checksum of the client already exist with compare it to the remote one
                if (!File.Exists(Constants.LocalChecksumFile))
                {
                    m_MD5Worker.RunWorkerAsync();
                }
                else
                {
                    LocalChecksum = File.ReadAllText(Constants.LocalChecksumFile);
                    if (string.IsNullOrEmpty(LocalChecksum))
                        m_MD5Worker.RunWorkerAsync();
                    else
                        CompareChecksums();
                }
            }
            catch (Exception ex)
            {
                HandleDownloadError(false, ex, (string)e.UserState);
            }
        }

        // create the md5 file from the whole directory
        private void MD5Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //
            m_MD5Worker.DoWork -= MD5Worker_DoWork;
            string currentDirectory = Directory.GetCurrentDirectory();
            HashSet<string> filesNames = new HashSet<string>(m_metaFile.Tasks.Select(x => x.LocalURL));
            List<string> list = (from x in Directory.GetFiles(currentDirectory, "*.*", SearchOption.AllDirectories)
                where filesNames.Contains(GetRelativePath(Path.GetFullPath(x), Path.GetFullPath("./")))
                select x into p
                orderby p
                select p).ToList();
            MD5 mD = MD5.Create();
            DateTime now = DateTime.Now;
            long location = 0L;
            int location2 = 0;
            foreach (string item in list.Take(list.Count - 1))
            {
                string text = item.Substring(currentDirectory.Length + 1);
                byte[] bytes = Encoding.UTF8.GetBytes(text.ToLower());
                mD.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
                Interlocked.Add(ref location, bytes.Length);
                byte[] array = File.ReadAllBytes(item);
                mD.TransformBlock(array, 0, array.Length, array, 0);
                Interlocked.Add(ref location, array.Length);
                Interlocked.Increment(ref location2);
                int percentProgress = location2 * 100 / list.Count;
                m_MD5Worker.ReportProgress(percentProgress, location / (DateTime.Now - now).TotalSeconds);
            }
            if (list.Count > 0)
            {
                byte[] array2 = File.ReadAllBytes(list.Last());
                mD.TransformFinalBlock(array2, 0, array2.Length);
            }
            LocalChecksum = ((list.Count > 0) ? BitConverter.ToString(mD.Hash).Replace("-", "").ToLower() : string.Empty);
            File.WriteAllText(Constants.LocalChecksumFile, LocalChecksum);
        }

        private void MD5Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetState(string.Format(m_bytesFormatProvider, "Vérification de l'intégrité des fichiers en cours... ({0} % terminé) ({1:fs}/s)", e.ProgressPercentage, (double)e.UserState), Colors.Red);
            _mainWindow.UpdateGameDetails("Vérification de l'intégrité des fichiers en cours...", $"{e.ProgressPercentage}% terminé"); // Mise à jour des détails
        }

        private void MD5Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            m_MD5Worker.RunWorkerCompleted -= MD5Worker_RunWorkerCompleted;
            DownloadProgress = 100.00;
            SetState("Vérification de l'intégrité des fichiers terminé.", Colors.Red);
            _mainWindow.UpdateGameDetails("Vérification de l'intégrité des fichiers terminé", $"{DownloadProgress}% terminé");
            CompareChecksums();
        }

        //private void CompareChecksums()
        //{
        //    try
        //    {
        //        if (m_metaFile != null && m_metaFile.FolderChecksum != LocalChecksum)
        //        {
        //            IsUpToDate = false;
        //            m_playCommand.RaiseCanExecuteChanged();

        //            m_currentTasks = new Stack<MetaFileEntry>(m_metaFile.Tasks);
        //            GlobalDownloadProgress = true;
        //            TotalBytesToDownload = m_metaFile.Tasks.Sum(x => x.FileSize);
        //            DownloadProgress = 0.00;
        //            ProgressDownloadSpeedInfo = string.Empty;
        //            ProcessTask();
        //        }
        //        else
        //        {
        //            File.WriteAllText(Constants.LocalChecksumFile, LocalChecksum);
        //            SetState("Le jeu est à jour.", Colors.Red);
        //            IsUpdating = false;
        //            IsUpToDate = true;

        //            View.Dispatcher.BeginInvoke( /*(Action)delegate*/new Action(() =>
        //            {
        //                m_playCommand.RaiseCanExecuteChanged();
        //                m_repairGameCommand.RaiseCanExecuteChanged();
        //                //m_consoleCommand.RaiseCanExecuteChanged(); //
        //            })); //);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleDownloadError(false, ex, Constants.UpdateSiteURL + Constants.RemotePatchFile);
        //    }
        //}

        private void CompareChecksums()
        {
            foreach (var url in serverUrls)
            {
                try
                {
                    CompareChecksumsForServer(url);
                    // If the comparison is successful, break out of the loop
                    break;
                }
                catch (Exception ex)
                {
                    // If an exception occurs, don't do anything. The loop will move on to the next server.
                    // If this was the last server, you might want to display an error message.
                    HandleDownloadError(false, ex, url + Constants.RemotePatchFile);
                }
            }
        }

        private void CompareChecksumsForServer(string serverUrl)
        {
            try
            {
                if (m_metaFile != null && m_metaFile.FolderChecksum != LocalChecksum)
                {
                    IsUpToDate = false;
                    m_playCommand.RaiseCanExecuteChanged();

                    m_currentTasks = new Stack<MetaFileEntry>(m_metaFile.Tasks);
                    GlobalDownloadProgress = true;
                    TotalBytesToDownload = m_metaFile.Tasks.Sum(x => x.FileSize);
                    DownloadProgress = 0.00;
                    ProgressDownloadSpeedInfo = string.Empty;
                    ProcessTask();
                }
                else
                {
                    File.WriteAllText(Constants.LocalChecksumFile, LocalChecksum);
                    SetState("Le jeu est à jour.", Colors.Red);
                    _mainWindow.UpdateGameDetails("Le jeu est à jour", "Prêt à jouer sur Pyrasis !"); // Mise à jour des détails
                    IsUpdating = false;
                    IsUpToDate = true;

                    View.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        m_playCommand.RaiseCanExecuteChanged();
                        m_repairGameCommand.RaiseCanExecuteChanged();
                        //m_consoleCommand.RaiseCanExecuteChanged(); //
                        IsNumberComboBoxEnabled = true;
                        IsLanguageComboBoxEnabled = true;
                        IsStatusTextBoxVisible = true;
                    }));//);
                }
            }
            catch (Exception ex)
            {
                throw ex; //HandleDownloadError(false, ex, Constants.UpdateSiteURL + Constants.RemotePatchFile);
            }
        }

        private void ProcessTask()
        {
            ThreadPool.QueueUserWorkItem(_ =>
                {
                    if (m_currentTasks.Count == 0)
                    {
                        OnUpdateEnded(true);
                    _mainWindow.UpdateGameDetails("Le jeu est à jour", "Prêt à jouer sur Pyrasis !"); //voir si ça debug
                        return;
                    }

                    var task = m_currentTasks.Pop();

                    task.Downloaded += OnTaskApplied;
                    task.Download(this);
                });
            //
        }

        private void OnTaskApplied(MetaFileEntry x)
        {
            TotalDownloadedBytes += x.FileSize;
            DownloadProgress = ((double)TotalDownloadedBytes / (double)TotalBytesToDownload) * 100.00;
            _mainWindow.UpdateGameDetails("Téléchargement en cours...", $"{DownloadProgress:F0}% terminé");
            ProcessTask();
        }

        private void OnUpdateEnded(bool success)
        {
            if (success)
            {
                SetState("Le jeu est à jour.", Colors.Red);
                _mainWindow.UpdateGameDetails("Le jeu est à jour", "Prêt à jouer sur Pyrasis !"); // Mise à jour des détails
                LocalChecksum = m_metaFile.FolderChecksum;
                File.WriteAllText(Constants.LocalChecksumFile, LocalChecksum);
            }

            IsUpToDate = success;
            IsUpdating = false;
            GlobalDownloadProgress = false;
            ProgressDownloadSpeedInfo = string.Empty;
            _mainWindow.UpdateGameDetails("Le jeu est à jour", "Prêt à jouer sur Pyrasis !");

            View.Dispatcher.BeginInvoke(new Action(() =>
            {
                m_playCommand.RaiseCanExecuteChanged();
                m_repairGameCommand.RaiseCanExecuteChanged();
                //m_consoleCommand.RaiseCanExecuteChanged(); //
                IsNumberComboBoxEnabled = true;
                IsLanguageComboBoxEnabled = true;
                IsStatusTextBoxVisible = true;
            }));
        }

        private void HandleDownloadError(bool cancelled, Exception ex, string url)
        {
            if (cancelled)
            {
                SetState("Mise à jour interrompue.", Colors.Red);
                _mainWindow.UpdateGameDetails("Erreur de mise à jour", "Veuillez réessayer"); // Mise à jour des détails
            }
            else
            {
                var remoteURL = url;

                MessageBox.Show(string.Format(Resources.Download_File_Error, remoteURL, ex), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Clipboard.SetText(ex.ToString());
                if (ex.InnerException != null)
                    SetState($"Erreur lors de la mise à jour : {ex.InnerException.Message}", Colors.Red);
                _mainWindow.UpdateGameDetails("Erreur de mise à jour", "Veuillez contacter le support !");
            }

            OnUpdateEnded(false);
        }


        private DateTime? m_lastProgressChange;
        private long m_lastGlobalDownloadedBytes;
        private long m_lastFileDownloadedBytes;
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!GlobalDownloadProgress)
            {
                DownloadProgress = ((double) e.BytesReceived/e.TotalBytesToReceive)*100.00;

                if (m_lastProgressChange != null &&
                    (DateTime.Now - m_lastProgressChange.Value) > TimeSpan.FromSeconds(1))
                {
                    ProgressDownloadSpeedInfo = string.Format(m_bytesFormatProvider, "{0:fs} / {1:fs} ({2:fs}/s)",
                        (e.BytesReceived), e.TotalBytesToReceive,
                        (e.BytesReceived - m_lastFileDownloadedBytes)/
                        (DateTime.Now - m_lastProgressChange.Value).TotalSeconds);

                    
                    m_lastProgressChange = DateTime.Now;
                    m_lastFileDownloadedBytes = e.BytesReceived;
                }
                DownloadProgress = ((double)TotalDownloadedBytes / (double)TotalBytesToDownload) * 100.00;//
                _mainWindow.UpdateGameDetails("Téléchargement en cours...", $"{DownloadProgress:F0}% terminé");//
                //_mainWindow.UpdateGameDetails("Téléchargement en cours ...", $"{DownloadProgress:F2}% terminé");
            }
            else
            {
                if (m_lastProgressChange != null && (DateTime.Now - m_lastProgressChange.Value) > TimeSpan.FromSeconds(1))
                {
                    ProgressDownloadSpeedInfo = string.Format(m_bytesFormatProvider, "{0:fs} / {1:fs} ({2:fs}/s)",
                        (TotalDownloadedBytes + e.BytesReceived), TotalBytesToDownload,
                        ((TotalDownloadedBytes + e.BytesReceived) - m_lastGlobalDownloadedBytes)/
                        (DateTime.Now - m_lastProgressChange.Value).TotalSeconds);

                    
                    m_lastProgressChange = DateTime.Now;
                    m_lastGlobalDownloadedBytes = TotalDownloadedBytes + e.BytesReceived;
                    DownloadProgress = ((double)TotalDownloadedBytes / (double)TotalBytesToDownload) * 100.00;//
                    _mainWindow.UpdateGameDetails("Vérirication en cours...", $"{DownloadProgress:F0}% terminé");//
                }
            }

            if (m_lastProgressChange == null)
                m_lastProgressChange = DateTime.Now;
        }

        //Uplauncher
		
		private bool _isNumberComboBoxEnabled;

        public bool IsNumberComboBoxEnabled
        {
            get => _isNumberComboBoxEnabled;
            set
            {
                _isNumberComboBoxEnabled = value;
                RaisePropertyChanged("IsNumberComboBoxEnabled");
            }
        }

        private void UpdateLanguageInConfigFile(string languageCode)
        {
            string appFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            string appFolder = Path.Combine(appFolderPath, "app");
            string configFile = Path.Combine(appFolder, "config.xml");

            // Charge le fichier XML et sélectionne l'élément <entry key="lang.current">
            XDocument config = XDocument.Load(configFile);
            XElement languageEntry = config.Descendants("entry").FirstOrDefault(x => x.Attribute("key")?.Value == "lang.current");

            // Modifie l'élément avec la langue sélectionnée et enregistre le fichier XML
            if (languageEntry != null)
            {
                languageEntry.Value = languageCode;
                config.Save(configFile);
            }
        }

        private void UpdateLanguage(string filePath, string newLanguage)
        {
            XDocument xmlDoc = XDocument.Load(Constants.ConfigFile/*filePath*/);
            XElement langElement = xmlDoc.Root.Descendants("entry")
                .FirstOrDefault(e => e.Attribute("key")?.Value == "lang.current");

            if (langElement != null)
            {
                langElement.Value = newLanguage;
                xmlDoc.Save(filePath);
            }
        }

        private bool _isLanguageComboBoxEnabled;

        public bool IsLanguageComboBoxEnabled
        {
            get => _isLanguageComboBoxEnabled;
            set
            {
                _isLanguageComboBoxEnabled = value;
                RaisePropertyChanged("IsLanguageComboBoxEnabled");
            }
        }

        private bool _isStatusTextBoxVisible;
        public bool IsStatusTextBoxVisible
        {
            get => _isStatusTextBoxVisible;
            set
            {
                _isStatusTextBoxVisible = value;
                RaisePropertyChanged("IsStatusTextBoxVisible");
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //public DelegateCommand ConsoleCommand => m_consoleCommand ?? (m_consoleCommand = new DelegateCommand(OnConsole, CanConsole));
        private bool VerifyVersion()
        {
            if (!File.Exists(Constants.Version))
            {
                StreamWriter streamWriter = File.CreateText(Constants.Version);
                streamWriter.Write("1");
                streamWriter.Close();
                StreamReader streamReader = new StreamReader(Constants.VersionPath);
                string b = streamReader.ReadToEnd();
                streamReader.Close();
                currentversion = ReadRemoteTextFile(Constants.VersionURL);
                if (currentversion == b)
                {
                    return true;
                }
                return false;
            }
            StreamReader streamReader2 = new StreamReader(Constants.VersionPath);
            string b2 = streamReader2.ReadToEnd();
            streamReader2.Close();
            currentversion = ReadRemoteTextFile(Constants.VersionURL);
            if (currentversion == b2)
            {
                return true;
            }
            return false;
        }

        private void UpdateDone(object sender, AsyncCompletedEventArgs e)
        {
            m_MD5Worker.RunWorkerCompleted -= MD5Worker_RunWorkerCompleted;
            DownloadProgress = 100.00;
            SetState($"Téléchargement terminé. L'UpLauncher va redémarrer...", Colors.Red);
            _mainWindow.UpdateGameDetails("Téléchargement terminé", "L'uplauncher va redémarrer...");
            View.Dispatcher.BeginInvoke((Action)delegate
            {
                m_playCommand.RaiseCanExecuteChanged();
                m_repairGameCommand.RaiseCanExecuteChanged();
                //m_consoleCommand.RaiseCanExecuteChanged();    //
            });
            File.Delete(Constants.Version);
            StreamWriter streamWriter = File.CreateText(Constants.Version);
            streamWriter.Write(currentversion);
            streamWriter.Close();
            //Process.Start(Constants.PatchPath, "-restart");
            Process.Start(Application.StartupPath + Constants.PatchPath);
            Application.Exit();
        }
        private void UpdateProgressChange(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!GlobalDownloadProgress)
            {
                DownloadProgress = (double)e.BytesReceived / (double)e.TotalBytesToReceive * 100.00;
                if (m_lastProgressChange.HasValue && DateTime.Now - m_lastProgressChange.Value > TimeSpan.FromSeconds(1.00))
                {                                                                         
                    ProgressDownloadSpeedInfo = string.Format(m_bytesFormatProvider, "{0:fs} / {1:fs} ({2:fs}/s)", e.BytesReceived, e.TotalBytesToReceive, (double)(e.BytesReceived - m_lastFileDownloadedBytes) / (DateTime.Now - m_lastProgressChange.Value).TotalSeconds);
                    m_lastProgressChange = DateTime.Now;
                    m_lastFileDownloadedBytes = e.BytesReceived;
                }
            }
            else if (m_lastProgressChange.HasValue && DateTime.Now - m_lastProgressChange.Value > TimeSpan.FromSeconds(1.00))
            {
                ProgressDownloadSpeedInfo = string.Format(m_bytesFormatProvider, "{0:fs} / {1:fs} ({2:fs}/s)", TotalDownloadedBytes + e.BytesReceived, TotalBytesToDownload, (double)(TotalDownloadedBytes + e.BytesReceived - m_lastGlobalDownloadedBytes) / (DateTime.Now - m_lastProgressChange.Value).TotalSeconds);
                m_lastProgressChange = DateTime.Now;
                m_lastGlobalDownloadedBytes = TotalDownloadedBytes + e.BytesReceived;
            }
            if (!m_lastProgressChange.HasValue)
            {
                m_lastProgressChange = DateTime.Now;
            }
            SetState($"Téléchargement de la mise à jour de l'uplauncher...", Colors.Orange);
            _mainWindow.UpdateGameDetails("Téléchargement de la mise à jour de l'uplauncher...", "Patientez !");
        }

        private void DownloadUpLauncher(string Url, string DownloadTo)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += UpdateProgressChange;
            webClient.DownloadFileCompleted += UpdateDone;
            webClient.DownloadFileAsync(new Uri(Url), DownloadTo);
        }
        private string ReadRemoteTextFile(string Url)
        {
            Uri requestUri = new Uri(Url);
            WebRequest webRequest = WebRequest.Create(requestUri);
            WebResponse response = webRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream ?? throw new InvalidOperationException());
            return streamReader.ReadToEnd();
        }

        private DelegateCommand m_consoleCommand;

        private string currentversion;

        public UplauncherModelView()
        {
        }

        //Fin

        public void SetState(string message)
        {
            StateMessageColor = DefaultMessageColor;
            StateMessage = message;
        }

	    private void SetState(string message, Color color)
        {
            StateMessageColor = color;
            StateMessage = message;
        }

        public MainWindow View
        {
	        private get;
            set;
        }

        public NotifyIcon NotifyIcon
        {
            get;
            private set;
        }

	    private bool IsUpdating
        {
            get;
            set;
        }

	    private bool IsUpToDate
        {
            get;
            set;
        }

	    private string LocalChecksum
        {
            get;
	        set;
        }

	    private DateTime? LastVote
        {
            get;
	        set;
        }

        public double DownloadProgress
        {
            get;
            set;
        }

	    private Color StateMessageColor
        {
            get;
            set;
        }

        public string StateMessage
        {
            get;
            set;
        }

	    private long TotalBytesToDownload
        {
            get;
            set;
        }

	    private long TotalDownloadedBytes
        {
            get;
            set;
        }

	    private bool GlobalDownloadProgress
        {
            get;
            set;
        }

        public string ProgressDownloadSpeedInfo
        {
            get;
            set;
        }

        static string GetRelativePath(string fullPath, string relativeTo)
        {
            var foldersSplitted = fullPath.Split(new[] { relativeTo.Replace("/", "\\").Replace("\\\\", "\\") }, StringSplitOptions.RemoveEmptyEntries); // cut the source path and the "rest" of the path

            return foldersSplitted.Length > 0 ? foldersSplitted.Last() : ""; // return the "rest"
        }
    }
}