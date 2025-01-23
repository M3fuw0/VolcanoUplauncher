using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Uplauncher.Helpers;
using SharpRaven;
using SharpRaven.Data;
using System.Diagnostics;
using System.Net;
using Uplauncher.Exceptions;
using System.Threading.Tasks;

namespace Uplauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly RavenClient _ravenClient;

        public RavenClient ExceptionLogger { get; protected set; }
        public const bool IsExceptionLoggerEnabled = true;
        private bool isOnlyInstance;
        public string Version { get; protected set; }
        public static string ExceptionLoggerDSN = "https://26201117acb6eba645fdf0c939aed407:7b6ec22c2ac8e1a8a886951036a4399a@o375233.ingest.us.sentry.io/4508695127588864";
        private static Mutex singleInstanceMutex = new Mutex(true, "PyrasisUplauncher");
        public static string FastestServerUrl { get; private set; }
        public static string CurrentVersion =>
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string configFilePath = @".\vulcano_app\config.xml";

            // Vérifier et corriger la première ligne du fichier
            if (CheckAndFixFirstChar(configFilePath))
            {
                // Un espace a été supprimé, vous pouvez effectuer des opérations supplémentaires ici si nécessaire
            }

            SupprimerFichiersEtDossier();
            
            FastestServerUrl = GetFastestServerUrl(new List<string>
            {
                Constants.UpdateSiteURL,
                Constants.SecondaryUpdateSiteURL
            });

            if (string.IsNullOrEmpty(FastestServerUrl))
            {
                // Si aucun serveur n'est disponible
                MessageBox.Show("Aucun serveur de mise à jour n'est disponible. Vérifiez votre connexion.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(); // Arrêter l'application si aucun serveur n'est accessible
            }

            InitializeUpdaterConnexion();
        }

        public App()
        {
            _ravenClient = new RavenClient(ExceptionLoggerDSN)
            {
                Environment = IsExceptionLoggerEnabled ? "PRODUCTION" : "DEBUG",
                Release = Version
            };

            // ajouter des métadonnées contextuelles globales
            _ravenClient.Tags["Application"] = "Uplauncher";
            _ravenClient.Tags["Version"] = Version ?? "unknown";
            //_ravenClient.User = new SentryUser
            //{
            //    Username = Environment.UserName,
            //    Id = Environment.UserDomainName
            //};
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (ApplicationRunningHelper.AlreadyRunning())
                Shutdown();

            DispatcherUnhandledException += OnUnhandledException;

            // gérer les exceptions non gérées dans les threads autres que UI
            AppDomain.CurrentDomain.UnhandledException += (o, args) =>
            {
                if (args.ExceptionObject is Exception exception)
                {
                    try
                    {
                        // envoyer l'exception à sentry
                        _ravenClient.Capture(new SentryEvent(exception));
                        Debug.WriteLine($"Exception non gérée (background thread) : {exception.Message}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Erreur lors de l'envoi de l'exception à Sentry : {ex.Message}");
                    }
                }
            };

            // gérer les exceptions des tâches (TaskScheduler)
            TaskScheduler.UnobservedTaskException += (o, args) =>
            {
                try
                {
                    // envoyer l'exception à sentry
                    _ravenClient.Capture(new SentryEvent(args.Exception));
                    Debug.WriteLine($"Exception non observée : {args.Exception.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erreur lors de l'envoi à Sentry : {ex.Message}");
                }

                args.SetObserved(); // marquer comme géré pour éviter de planter
            };
        }

        private string GetFastestServerUrl(List<string> serverUrls)
        {
            string fastestUrl = null;
            long fastestTime = long.MaxValue;

            foreach (var url in serverUrls)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "HEAD"; // Requête rapide, juste pour obtenir les headers
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        stopwatch.Stop();
                        long elapsedTime = stopwatch.ElapsedMilliseconds;

                        if (elapsedTime < fastestTime)
                        {
                            fastestTime = elapsedTime;
                            fastestUrl = url;
                        }
                    }
                }
                catch (WebException)
                {
                    // Si une exception se produit, ignorer ce serveur et essayer le suivant.
                    continue;
                }
            }

            //MessageBox.Show("url : " + fastestUrl);
            return fastestUrl;
        }

        private bool CheckAndFixFirstChar(string filePath)
        {
            // Vérifier si le fichier existe
            if (!File.Exists(filePath))
            {
                // Si le fichier n'existe pas, retourner false et passer à la fonction suivante
                return false;
            }

            // Lire toutes les lignes du fichier
            string[] lines = File.ReadAllLines(filePath);

            // Vérifier s'il y a au moins une ligne
            if (lines.Length > 0)
            {
                // Vérifier si le premier caractère de la première ligne est un espace
                if (lines[0].Length > 0 && lines[0][0] == ' ')
                {
                    // Si c'est le cas, supprimer l'espace et réécrire le fichier
                    lines[0] = lines[0].TrimStart(' ');
                    File.WriteAllLines(filePath, lines);

                    // Retourner true pour indiquer qu'un espace a été supprimé
                    return true;
                }
            }

            // Si le fichier est vide ou si la première ligne ne commence pas par un espace, retourner false
            return false;
        }

        private void SupprimerFichiersEtDossier()
        {
            // Chemin de base où se trouvent les fichiers et dossiers à supprimer
            string cheminDeBase = Path.Combine(Environment.CurrentDirectory, "vulcano_app");

            // Définir les chemins des fichiers à supprimer
            string[] fichiersASupprimer = 
            {
                Path.Combine(cheminDeBase, "CleanDofusInvoker.swf"),
                Path.Combine(cheminDeBase, "data.rar"),
                Path.Combine(cheminDeBase, "ui.rar"),
                Path.Combine(cheminDeBase, "ui", "Ankama_Storage", "Ankama_Storage.d2ui"),  // Ajout du fichier spécifié
                Path.Combine(cheminDeBase, "ui", "Ankama_ContextMenu", "Ankama_ContextMenu.d2ui"),
                Path.Combine(cheminDeBase, "ui", "Ankama_GameUiCore", "Ankama_GameUiCore.d2ui"),
                Path.Combine(cheminDeBase, "ui", "Ankama_Grimoire", "Ankama_Grimoire.d2ui")
            };

            // Boucle pour supprimer chaque fichier s'il existe
            foreach (var fichier in fichiersASupprimer)
            {
                if (File.Exists(fichier))
                {
                    try
                    {
                        File.Delete(fichier);
                        Debug.WriteLine($"Le fichier {fichier} a été supprimé.");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Erreur lors de la suppression du fichier {fichier}: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine($"Le fichier {fichier} n'existe pas, pas de suppression.");
                }
            }

            // Définir le chemin du dossier à supprimer
            string dossierASupprimer = Path.Combine(cheminDeBase, "ui", "Ankama_StorageEX");

            // Vérifier si le dossier existe puis le supprimer
            if (Directory.Exists(dossierASupprimer))
            {
                try
                {
                    Directory.Delete(dossierASupprimer, true);
                    Debug.WriteLine($"Le dossier {dossierASupprimer} a été supprimé.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erreur lors de la suppression du dossier {dossierASupprimer}: {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine($"Le dossier {dossierASupprimer} n'existe pas, pas de suppression.");
            }
        }

        private void InitializeUpdaterConnexion()
        {
            try
            {
                Uplauncher.Updater.UpdaterConnexionHandler.GetInstance();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur dans InitializeUpdaterConnexion : {ex.Message}");
            }
        }

        //private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        //{
        //    Clipboard.SetText(e.Exception.ToString());
        //    MessageBox.Show("Erreur (copié) : " + e.Exception);
        //}

        //private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        //{
        //    try
        //    {
        //        // envoyer l'exception à sentry
        //        _ravenClient.Capture(new SentryEvent(e.Exception));

        //        Clipboard.SetText(e.Exception.ToString());
        //        MessageBox.Show("Erreur (copiée dans le presse-papiers) : " + e.Exception.Message,
        //            "Erreur non gérée",
        //            MessageBoxButton.OK,
        //            MessageBoxImage.Error);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Erreur lors de l'envoi à Sentry : {ex.Message}");
        //    }
        //    // empêcher l'application de planter automatiquement
        //    //e.Handled = true;
        //}

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                // créer un événement Sentry
                var sentryEvent = new SentryEvent(e.Exception);

                // créer un dictionnaire pour Extra et y ajouter des données
                //sentryEvent.Extra = new Dictionary<string, object>
                //{
                //    { "MachineName", Environment.MachineName },
                //    { "OSVersion", Environment.OSVersion.ToString() }
                //};

                sentryEvent.Extra = new Dictionary<string, object>
                {
                    { "MachineName", Environment.MachineName },
                    { "OSVersion", Environment.OSVersion.ToString() },
                    { "UserDetails", new { Username = Environment.UserName, Domain = Environment.UserDomainName } }
                };

                // envoyer l'événement à sentry
                _ravenClient.Capture(sentryEvent);

                Clipboard.SetText(e.Exception.ToString());
                MessageBox.Show("Erreur (copiée dans le presse-papiers) : " + e.Exception.Message,
                    "Erreur non gérée",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de l'envoi à Sentry : {ex.Message}");
            }

            // empêcher l'application de planter automatiquement
            e.Handled = true;
        }
    }
}
