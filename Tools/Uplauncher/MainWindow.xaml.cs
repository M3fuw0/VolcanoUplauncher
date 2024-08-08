using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using DiscordRPC;
using System.Windows.Threading;
using System.Xml.Linq;
using Button = DiscordRPC.Button;

namespace Uplauncher
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static DiscordRpcClient _client;
        private DispatcherTimer _timer;
        private static DateTime _startTime;
        public MainWindow()
        {
            ModelView = new UplauncherModelView(DateTime.Now) { View = this };

            InitializeComponent();
            InitializeDiscordRpc();
        }

        public UplauncherModelView ModelView { get; set; }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ModelView.CheckUpdates();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if ((bool)typeof(Application)
                    .GetProperty("IsShuttingDown",
                        BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Static)
                    .GetValue(null, new object[0]))
            {
                ModelView.NotifyIcon.Visible = false;
                return;
            }

            e.Cancel = true;
            ModelView.HideWindowInTrayIcon();
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedLanguage = ((ComboBoxItem)LanguageComboBox.SelectedItem).Content.ToString();
            string newLanguageCode;

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
                case "Poland":
                    newLanguageCode = "pl";
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            DragMove();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            string gameDirectoryPath = "sulax_app";
            string checksumFilePath = ".\\checksum.sulax";

            // Supprime le dossier sulax_app s'il existe
            if (Directory.Exists(gameDirectoryPath))
            {
                Directory.Delete(gameDirectoryPath, true);
            }

            // Supprime checksum.sulax s'il existe
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
            UpdatePresenceWithElapsedTime(Constants.GameDetails, Constants.WebsiteUrl, Constants.LargeImageKey, Constants.SmallImageKey);
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
        }
    }
}