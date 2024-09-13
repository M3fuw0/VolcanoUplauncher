using DiscordRPC;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using Uplauncher.Helpers;
using Application = System.Windows.Application;
using Button = DiscordRPC.Button;

namespace Uplauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static DiscordRpcClient _client;
        private DispatcherTimer _timer;
        private static DateTime _startTime;
        private DispatcherTimer _processCheckTimer;
        private static string _gameDetails = "Joue à Pyrasis 2.51";
        private static string _websiteUrl = "Allez sur https://pyrasis.cc";
        //private Mutex mutex;
        //private const string MutexId = "Global\\KarashiUplauncherMultiDofus";

        public MainWindow()
        {
            ModelView = new UplauncherModelView(DateTime.Now, this) { View = this };

            //if (CheckAndHandleMultipleInstances())
            //{
            //    return;
            //}
            //else
            //{
            //    //Process.Start(Constants.PatchPath, "-restart");
            //}


            if (ApplicationRunningHelper.AlreadyRunning())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Une autre instance de l'Uplauncher est déjà en cours d'exécution. Voulez-vous fermer cette instance et continuer ?",
                    "Instance déjà en cours",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    //Application.Current.Shutdown();
                    //Form2.ShutdownApp();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    //return true; // Indique que l'application doit se terminer
                }
                else
                {
                    TerminateOtherInstances();
                    Process.Start(Constants.PatchPath, "-restart");
                }
            }

            InitializeComponent();
            InitializeDiscordRpc();

            UplauncherModelView model = new UplauncherModelView();
            //model.LoadConfiguration();

            //// Essayez de créer un Mutex global en utilisant un nom unique.
            //mutex = new Mutex(true, MutexId, out bool createdNew);

            //// Si le Mutex existe déjà, c'est que l'application est déjà en cours d'exécution.
            //if (!createdNew)
            //{
            //    // Manipuler la situation, par exemple en affichant un message ou en terminant l'opération.
            //    MessageBox.Show("Form1 est déjà en cours d'exécution.");
            //}
        }

        public UplauncherModelView ModelView
        {
            get;
            set;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ModelView.CheckUpdates();
            UpdateGameDetails("Démarrage de l'uplauncher ...", "En attendant allez sur https://pyrasis.cc");
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if ((bool)typeof(Application).GetProperty("IsShuttingDown", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, new object[0]))
            {
                ModelView.NotifyIcon.Visible = false;
                return;
            }

            e.Cancel = true;
            ModelView.HideWindowInTrayIcon();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            DragMove();
        }

        //private void NumberComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (DataContext is UplauncherModelView uplauncherModelView)
        //    {
        //        if (NumberComboBox.SelectedItem != null)
        //        {
        //            string selectedItem = ((ComboBoxItem)NumberComboBox.SelectedItem).Content.ToString();
        //            if (int.TryParse(selectedItem, out int numberOfClients))
        //            {
        //                uplauncherModelView.NumberOfClientsToStart = numberOfClients;
        //                uplauncherModelView.StartClients(numberOfClients);
        //            }
        //        }
        //    }
        //}

        //private void NumberComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (this.DataContext is UplauncherModelView uplauncherModelView)
        //    {
        //        if (NumberComboBox.SelectedItem != null)
        //        {
        //            string selectedItem = ((ComboBoxItem)NumberComboBox.SelectedItem).Content.ToString();
        //            if (int.TryParse(selectedItem, out int numberOfClients))
        //            {
        //                //numberOfClients = numberOfClients /*> 1 ? numberOfClients - 1 : numberOfClients*/;
        //                uplauncherModelView.NumberOfClientsToStart = numberOfClients;
        //                //uplauncherModelView.StartClients(numberOfClients);
        //            }
        //        }
        //    }
        //}

        //private void NumberComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (DataContext is UplauncherModelView uplauncherModelView)
        //    {
        //        if (NumberComboBox.SelectedItem != null)
        //        {
        //            string selectedItem = ((ComboBoxItem)NumberComboBox.SelectedItem).Content.ToString();
        //            if (int.TryParse(selectedItem, out int numberOfClients))
        //            {
        //                numberOfClients = numberOfClients < 10 ? numberOfClients + 0 : numberOfClients;
        //                uplauncherModelView.NumberOfClientsToStart = numberOfClients;
        //            }
        //        }
        //    }
        //}

        private void NumberComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (CheckAndHandleMultipleInstances())
            //{
            //    return;
            //}
            if (DataContext is UplauncherModelView uplauncherModelView)
            {
                //if (NumberComboBox.SelectedItem is ComboBoxItem selectedItem)
                //{
                //    if (int.TryParse(selectedItem.Content.ToString(), out int numberOfClients))
                //    {
                //        // Add 1 to numberOfClients if it is 2 or more
                //        if (numberOfClients >= 2)
                //        {
                //            numberOfClients += 1;
                //        }

                //        // Now numberOfClients will be incremented by 1 if it was 2 or more
                //        uplauncherModelView.NumberOfClientsToStart = numberOfClients;

                //        // Uncomment if needed for debugging
                //        // MessageBox.Show($"Nombre de clients sélectionné : {numberOfClients}");
                //    }
                //}
            }
        }

        private bool CheckAndHandleMultipleInstances()
        {
            if (ApplicationRunningHelper.AlreadyRunning())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Une autre instance de l'Uplauncher est déjà en cours d'exécution. Voulez-vous fermer cette instance et continuer ?",
                    "Instance déjà en cours",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    //Application.Current.Shutdown();
                    //Form2.ShutdownApp();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    return true; // Indique que l'application doit se terminer
                }
                else
                {
                    TerminateOtherInstances();
                }
            }

            return false; // Continuer l'exécution normale
        }

        //private void TerminateOtherInstances()
        //{
        //    var currentProcess = Process.GetCurrentProcess();
        //    var uplauncherProcesses = Process.GetProcessesByName("Uplauncher");

        //    foreach (var process in uplauncherProcesses)
        //    {
        //        if (process.Id != currentProcess.Id)
        //        {
        //            process.Kill();
        //        }
        //    }
        //}

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedLanguage = ((ComboBoxItem)LanguageComboBox.SelectedItem).Content.ToString();
            string newLanguageCode;
            //en,es,de,nl,pt,cz,sk,pl,ro,tr
            // Convertir le texte sélectionné en code de langue approprié
            switch (selectedLanguage)
            {
                case "Français":
                    newLanguageCode = "fr";
                    break;
                case "English":
                    newLanguageCode = "en";
                    break;
                case "Español":
                    newLanguageCode = "es";
                    break;
                case "Português":
                    newLanguageCode = "pt";
                    break;
                case "Deutsch":
                    newLanguageCode = "de";
                    break;
                case "Nederlands":
                    newLanguageCode = "nl";
                    break;
                case "Čeština":
                    newLanguageCode = "cz";
                    break;
                case "Slovenský":
                    newLanguageCode = "sk";
                    break;
                case "Polski":
                    newLanguageCode = "pl";
                    break;
                case "Română":
                    newLanguageCode = "ro";
                    break;
                case "Türkçe":
                    newLanguageCode = "tr";
                    break;
                case "Dansk":
                    newLanguageCode = "dk";
                    break;
                case "Suomalainen":
                    newLanguageCode = "fi";
                    break;
                // Ajoutez d'autres cas pour d'autres langues
                default:
                    newLanguageCode = "fr";
                    break;
            }
            // Mettre à jour le fichier XML avec la nouvelle langue
            string filePath = Path.Combine(Constants.GameDirPath, Constants.ConfigFile);
            UpdateLanguage(filePath, newLanguageCode);
        }

        private void UpdateLanguage(string filePath, string newLanguage)
        {
            if (File.Exists(filePath)) // Ajouter cette vérification
            {
                XDocument xmlDoc = XDocument.Load(filePath);
                XElement langElement = xmlDoc.Root.Descendants("entry")
                    .FirstOrDefault(e => e.Attribute("key")?.Value == "lang.current");

                if (langElement != null)
                {
                    langElement.Value = newLanguage;
                    xmlDoc.Save(filePath);
                }
            }
            else
            {
                // Vous pouvez traiter ici le cas où le fichier n'existe pas encore.
                // Par exemple, vous pouvez créer le fichier avec la langue sélectionnée,
                // ou afficher un message indiquant que le fichier n'a pas été trouvé.
            }
        }

        //public void LoadConfiguration()
        //{
        //    var configPath = "karashi_app/config.xml";
        //    var xdoc = XDocument.Load(configPath);
        //    var legacyEntry = xdoc.Descendants("entry")
        //        .FirstOrDefault(e => (string)e.Attribute("key") == "legacy");

        //    if (legacyEntry != null)
        //    {
        //        IsLegacyModeEnabled = bool.Parse(legacyEntry.Value);
        //    }
        //}

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            string gameDirectoryPath = "pyrasis_app";
            string checksumFilePath = ".\\checksum.pyrasis";

            // Supprime le dossier pyrasis_app s'il existe
            if (Directory.Exists(gameDirectoryPath))
            {
                Directory.Delete(gameDirectoryPath, true);
            }

            // Supprime checksum.pyrasis s'il existe
            if (File.Exists(checksumFilePath))
            {
                File.Delete(checksumFilePath);
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            Process.Start(hyperlink.NavigateUri.ToString());
        }

        private void InitializeDiscordRpc()
        {
            _client = new DiscordRpcClient(Constants.ClientId, autoEvents: true);
            _client.Initialize();

            _startTime = DateTime.UtcNow;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdatePresenceWithElapsedTime(_gameDetails, _websiteUrl, Constants.LargeImageKey, Constants.SmallImageKey);
        }

        // Ajoutez cette méthode pour initialiser le timer
        public void InitializeProcessCheckTimer()
        {
            _processCheckTimer = new DispatcherTimer();
            _processCheckTimer.Interval = TimeSpan.FromSeconds(5);
            _processCheckTimer.Tick += ProcessCheckTimer_Tick;
            _processCheckTimer.Start();
        }

        // Ajoutez cette méthode pour gérer les ticks du timer
        private void ProcessCheckTimer_Tick(object sender, EventArgs e)
        {
            var dofusProcesses = Process.GetProcessesByName("Dofus");
            int dofusCount = dofusProcesses.Length;

            if (dofusCount > 0)
            {
                string details = dofusCount == 1 ? "1 fenêtre de jeu ouverte" : $"{dofusCount} fenêtres de jeu ouvertes";
                UpdateGameDetails("En train de jouer à Pyrasis", details);
            }
            else
            {
                UpdateGameDetails("Le jeu est à jour", "Prêt à jouer sur Pyrasis !");
            }
        }

        private static void UpdatePresenceWithElapsedTime(string details, string state, string largeImageKey, string smallImageKey)
        {
            if (_client == null || !_client.IsInitialized) return;

            _client.SetPresence(new RichPresence
            {
                Details = details,
                State = state,
                Assets = new Assets()
                {
                    LargeImageKey = largeImageKey,
                    //SmallImageKey = smallImageKey
                },
                Buttons = new[]
                {
                    new Button
                    {
                        Label = Constants.LabelButton1,
                        Url = Constants.UrlButton1
                    },
                    new Button
                    {
                        Label = Constants.LabelButton2,
                        Url = Constants.UrlButton2
                    }
                },
                Timestamps = new Timestamps()
                {
                    Start = _startTime
                }
            });
        }

        public void UpdateGameDetails(string details, string url)
        {
            _gameDetails = details;
            _websiteUrl = url;
            UpdatePresenceWithElapsedTime(_gameDetails, _websiteUrl, Constants.LargeImageKey, Constants.SmallImageKey);
        }

        private void TerminateOtherInstances()
        {
            var currentProcess = Process.GetCurrentProcess();
            var uplauncherProcesses = Process.GetProcessesByName("Uplauncher");

            foreach (var process in uplauncherProcesses)
            {
                if (process.Id != currentProcess.Id)
                {
                    process.Kill();
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_timer != null)
            {
                _timer.Stop();
            }

            if (_client != null && _client.IsInitialized)
            {
                _client.ClearPresence();
                _client.Dispose();
            }

            //mutex?.ReleaseMutex();
        }
    }
}
