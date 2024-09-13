using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Uplauncher.Helpers;
using SharpRaven;
using SharpRaven.Data;
using Uplauncher.Exceptions;

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
        public static string ExceptionLoggerDSN = "https://64af200c775b802f211a18ab4b92b995:fea6e72c623ece463fbb5a3c14f565d4@o375233.ingest.sentry.io/4505646529904640";
        private ExceptionManager _exceptionManager;
        public string dsn = "https://64af200c775b802f211a18ab4b92b995:fea6e72c623ece463fbb5a3c14f565d4@o375233.ingest.sentry.io/4505646529904640";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var ravenClient = new RavenClient(dsn);
            string configFilePath = @".\pyrasis_app\config.xml";

            // Vérifier et corriger la première ligne du fichier
            if (CheckAndFixFirstChar(configFilePath))
            {
                // Un espace a été supprimé, vous pouvez effectuer des opérations supplémentaires ici si nécessaire
            }

            //SupprimerFichiersEtDossier();
            // Continuer avec le démarrage normal de l'application
            // Initialisation de RavenClient et ExceptionManager
            //ExceptionManager ravenClient = // ... Initialisation de RavenClient
            _exceptionManager = new ExceptionManager(ravenClient);
            // Gestion des exceptions non gérées
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        public App()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            _ravenClient = new RavenClient(dsn);

            if (IsExceptionLoggerEnabled)
            {
                ExceptionLogger = new RavenClient(ExceptionLoggerDSN);
                ExceptionLogger.Release = Version;
#if DEBUG
                ExceptionLogger.Environment = "DEBUG";
#else
                 ExceptionLogger.Environment = "RELEASE";
#endif
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (ApplicationRunningHelper.AlreadyRunning())
                Shutdown();

            DispatcherUnhandledException += OnUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _exceptionManager.RegisterException(e.Exception);
            e.Handled = true; // Pour empêcher l'application de se fermer
        }

        public bool CheckAndFixFirstChar(string filePath)
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
            string cheminDeBase = Path.Combine(Environment.CurrentDirectory, "karashi_app");

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

        //private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        //{
        //    //_ravenClient.Capture(new SentryEvent(e.Exception));
        //    Clipboard.SetText(e.Exception.ToString());
        //    MessageBox.Show("Erreur (copié) : " + e.Exception);
        //    //e.Handled = true; //pour éviter que l'app se ferme après le crash
        //    _ravenClient.Capture(new SentryEvent(e.Exception));
        //}

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //var sentryEvent = new SentryEvent("Une erreur s'est produite.");
            //var sentryEvent = new RavenClient(ExceptionLoggerDSN);
            //RavenClient
            //var sentryEvent = new SentryEvent(e.Exception)
            //{
            //    //Culprit = e.Exception.Message,
            //    //TimeStamp = DateTime.UtcNow,
            //    Level = SharpRaven.Data.ErrorLevel.Error,
            //};

            //sentryEvent.Tags.Add("Platform", Environment.OSVersion.ToString());

            //string projectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            //sentryEvent.Tags.Add("Project", projectName);

            //var className = e.Exception.TargetSite?.DeclaringType?.Name ?? "Unknown";
            //sentryEvent.Logger = className ?? "Unknown";

            //// Récupérer le nom du produit pour ServerName
            //var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            //var productAttribute = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), false).FirstOrDefault() as System.Reflection.AssemblyProductAttribute;
            //var productName = productAttribute?.Product;
            //sentryEvent.Tags.Add("ServerName", productName ?? "Unknown Product");

            ////sentryEvent.User = new SharpRaven.Data.User { Id = Environment.UserName };

            //// Obtenir le code d'erreur de l'exception et le formater en hexadécimal
            //string errorCode = $"0x{e.Exception.HResult:X8}";
            //sentryEvent.Tags.Add("EventID", errorCode);
            //sentryEvent.Tags.Add("Exceptions", e.Exception.ToString());
            //sentryEvent.Tags.Add("Culprit", e.Exception.Message);
            //sentryEvent.Tags.Add("Error Message", e.Exception.Message);
            //sentryEvent.Tags.Add("Logger", className);
            //sentryEvent.Tags.Add("User", Environment.UserName); 
            //lo
            //sentryEvent.Tags.Add("EventID", errorCode);

            //sentryEvent.Extra = new Dictionary<string, string>
            //{
            //    { "EventID", errorCode }, // Utiliser le vrai code d'erreur
            //    { "Exceptions", e.Exception.ToString() },
            //    { "Error Message", e.Exception.Message },
            //    { "Logger", className },
            //    { "User", Environment.UserName }
            //    // ... Ajoutez tous les autres attributs ici
            //};

            if (ExceptionLogger != null)
            {
                var sentryEvent = new SentryEvent(e.Exception);
                ExceptionLogger.Capture(sentryEvent);
            }

            //_ravenClient.Capture(sentryEvent);

            Clipboard.SetText(e.Exception.ToString());
            MessageBox.Show("Erreur (copié) : " + e.Exception);
        }
    }
}
