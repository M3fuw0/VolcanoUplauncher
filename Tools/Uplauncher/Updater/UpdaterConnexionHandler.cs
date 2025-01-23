using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using SharpRaven;
using SharpRaven.Data;

namespace Uplauncher.Updater
{
    public class UpdaterConnexionHandler
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
        private static UpdaterConnexionHandler _instance;
        private static TcpClient _currentConnection;
        private static TcpListener _tcpListener; // Ajout du serveur TCP

        private readonly RavenClient _ravenClient;

        // DSN spécifique pour Sentry dédié à l'updater
        private const string UpdaterSentryDSN = "https://73888f33c1487a4a89e9dfa48d04f37a:d86f6504c9a4bf38691b5815e96921b1@o375233.ingest.us.sentry.io/4508695125295104";

        private UpdaterConnexionHandler()
        {
            if (_instance != null)
            {
                throw new Exception("La classe UpdaterConnexionHandler est un singleton.");
            }

            _ravenClient = new RavenClient(UpdaterSentryDSN)
            {
                Environment = "UPDATER",
                Release = App.CurrentVersion ?? "unknown"
            };

            StartServer(); // Lancer le serveur TCP au moment de l'initialisation
        }

        public static UpdaterConnexionHandler GetInstance()
        {
            return _instance ?? (_instance = new UpdaterConnexionHandler());
        }

        public static TcpClient GetConnection()
        {
            return _currentConnection;
        }

        private void StartServer()
        {
            try
            {
                // Initialisation du serveur TCP sur le port 4242
                _tcpListener = new TcpListener(IPAddress.Loopback, 4242);
                _tcpListener.Start();

                Log.Info("Le serveur TCP a démarré sur localhost:4242");
                Debug.WriteLine("Le serveur TCP a démarré sur localhost:4242");
                AcceptClientsAsync(); // Démarrer l'écoute des connexions
            }
            catch (Exception ex)
            {
                Log.Error($"Erreur lors du démarrage du serveur TCP : {ex.Message}");
                Debug.WriteLine($"Erreur lors du démarrage du serveur TCP : {ex.Message}");
                SendSentryLog("UpdaterConnexionHandler", $"Erreur lors du démarrage du serveur TCP : {ex.Message}", "error");
            }
        }

        private async void AcceptClientsAsync()
        {
            try
            {
                while (true)
                {
                    // Attente d'une connexion client
                    var client = await _tcpListener.AcceptTcpClientAsync();
                    Log.Info("Connexion client acceptée.");
                    Debug.WriteLine("Connexion client acceptée.");
                    HandleClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Erreur lors de l'acceptation d'un client : {ex.Message}");
                Debug.WriteLine($"Erreur lors de l'acceptation d'un client : {ex.Message}");
                SendSentryLog("UpdaterConnexionHandler", $"Erreur lors de l'acceptation d'un client : {ex.Message}", "error");
            }
        }

        private async void HandleClientAsync(TcpClient client)
        {
            try
            {
                using (var stream = client.GetStream())
                {
                    var buffer = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Log.Info($"Message reçu : {receivedMessage}");
                        Debug.WriteLine($"Message reçu : {receivedMessage}");

                        // Répondre au client
                        string response = $"Message reçu : {receivedMessage}";
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Erreur lors de la communication avec le client : {ex.Message}");
                Debug.WriteLine($"Erreur lors de la communication avec le client : {ex.Message}");
                SendSentryLog("UpdaterConnexionHandler", $"Erreur client : {ex.Message}", "error");
            }
        }

        private void SendSentryLog(string loggerName, string message, string level)
        {
            try
            {
                var sentryEvent = new SentryEvent(new SentryMessage(message))
                {
                    Level = (ErrorLevel)Enum.Parse(typeof(ErrorLevel), level, true),
                    Tags = { { "Logger", loggerName }, { "ClientVersion", App.CurrentVersion ?? "unknown" } },
                    Extra = new
                    {
                        MachineName = Environment.MachineName,
                        OSVersion = Environment.OSVersion.ToString(),
                        UserName = Environment.UserName,
                        DomainName = Environment.UserDomainName
                    }
                };

                _ravenClient.Capture(sentryEvent);
            }
            catch (Exception logEx)
            {
                Log.Error($"Erreur lors de l'envoi du log à Sentry : {logEx.Message}");
                Debug.WriteLine($"Erreur lors de l'envoi du log à Sentry : {logEx.Message}");
            }
        }
    }

    public static class PartManager
    {
        public static void Initialize()
        {
            Console.WriteLine("PartManager initialized");
            Debug.WriteLine("PartManager initialized");
        }
    }
}
