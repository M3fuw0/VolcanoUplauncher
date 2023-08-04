#region License GNU GPL

// AddFileTask.cs
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
using System.Net;
using System.Windows;
using System.Xml.Serialization;
using Uplauncher.Properties;
using Uplauncher.Utils;

namespace Uplauncher.Patcher
{
    [XmlType("Entry")]
    public class MetaFileEntry
    {
        // Assume you have a list of URLs:
        List<string> serverUrls = new List<string> { Constants.UpdateSiteURL, Constants.SecondaryUpdateSiteURL /*, more URLs as needed */ };


        public event Action<MetaFileEntry> Downloaded;

        private void OnApplied()
        {
            var handler = Downloaded;
            handler?.Invoke(this);
        }

        [XmlAttribute("url")]
        public string RelativeURL
        {
            get;
            set;
        }

        [XmlAttribute("local")]
        public string LocalURL
        {
            get;
            set;
        }

        [XmlAttribute("MD5")]
        public string FileMD5
        {
            get;
            set;
        }
        
        
        [XmlAttribute("size")]
        public long FileSize
        {
            get;
            set;
        }
        public void Download(UplauncherModelView uplauncher)
        {
            var fullPath = Path.GetFullPath("./" + LocalURL);
            var isUplauncherExeFile = fullPath.Equals(Path.GetFullPath(Constants.CurrentExePath),
                StringComparison.InvariantCultureIgnoreCase);
#if DEBUG
            if (isUplauncherExeFile)
            {
                OnApplied();
                return;
            }
#endif
            uplauncher.SetState($"Vérifie si {RelativeURL} existe déjà...");
            //uplauncher.SetState(string.Format("Check if {0} already exists ...", RelativeURL));

            if (File.Exists(fullPath))
            {
                var md5 = Cryptography.GetFileMD5HashBase64(fullPath);

                if (md5 == FileMD5)
                {
                    uplauncher.SetState($"Le fichier {RelativeURL} existe déjà... Suivant !");
                    //uplauncher.SetState(string.Format("File {0} already exists... Next !", RelativeURL));

                    OnApplied();
                    return;
                }
            }

            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? throw new InvalidOperationException());

            uplauncher.SetState($"Téléchargement de {RelativeURL} ...");
            //uplauncher.SetState(string.Format("Download {0} ...", RelativeURL));
            //14/05/2023
            //
            
            foreach (var url in serverUrls)
            {
                try
                {
                    if (isUplauncherExeFile)
                    {
                        uplauncher.WebClient.DownloadFileCompleted += OnUplauncherDownloaded;
                        uplauncher.WebClient.DownloadFileAsync(new Uri(url + RelativeURL),
                            "./" + Constants.ExeReplaceTempPath, Constants.ExeReplaceTempPath);
                    }
                    else
                    {
                        uplauncher.WebClient.DownloadFileCompleted += OnFileDownloaded;
                        uplauncher.WebClient.DownloadFileAsync(new Uri(url + RelativeURL), "./" + LocalURL, LocalURL);
                    }
                    // If the download starts successfully, break out of the loop
                    break;
                }
                catch (WebException)
                {
                    // If an exception occurs, don't do anything. The loop will move on to the next server.
                    // If this was the last server, you might want to display an error message.
                    //MessageBox.Show("Aucun serveur est disponible.");
                }
            }
            //if (isUplauncherExeFile)
            //{
            //    uplauncher.WebClient.DownloadFileCompleted += OnUplauncherDownloaded;

            //    uplauncher.WebClient.DownloadFileAsync(new Uri(Constants.UpdateSiteURL + RelativeURL),
            //        "./" + Constants.ExeReplaceTempPath, Constants.ExeReplaceTempPath);
            //}
            //else
            //{
            //    uplauncher.WebClient.DownloadFileCompleted += OnFileDownloaded;
            //    uplauncher.WebClient.DownloadFileAsync(new Uri(Constants.UpdateSiteURL + RelativeURL), "./" + LocalURL, LocalURL);

            //}
        }

        private void OnFileDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            ((WebClient) sender).DownloadFileCompleted -= OnFileDownloaded;
            OnApplied();
        }

        private static void OnUplauncherDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            ((WebClient) sender).DownloadFileCompleted -= OnUplauncherDownloaded;

            var file = Path.GetTempFileName() + ".exe";
            File.WriteAllBytes(file, Resources.UplauncherReplacer);

            var procInfo = new ProcessStartInfo
            {
                FileName = file,
                Arguments =
                    $"{Process.GetCurrentProcess().Id} \"{Path.GetFullPath(Constants.ExeReplaceTempPath)}\" \"{Path.GetFullPath(Constants.CurrentExePath)}\"",
                Verb = "runas"
            };

            try
            {
                Process.Start(procInfo);
                Directory.Delete("majs");   // 18/12/2020
                //NotifyIcon.Visible = false;
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                //The user refused the elevation
            }
        }
    }
}