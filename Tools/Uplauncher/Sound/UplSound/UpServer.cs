using System.Net;
using System;
using System.Net.Sockets;
using Uplauncher.Sound.Network;

namespace Uplauncher.Sound.UplSound
{
	public class UpServer
	{
		private SimpleServer m_server;

		private UpClient m_client;

		public UpClient Client => m_client;

		public bool IsStart => m_server.Connected;

		public UpServer()
		{
			m_server = new SimpleServer();
		}

        /*public void StartAuthentificate()
		{
			m_server.Start(54249);
			m_server.ConnectionAccepted += AccepteClient;
		}*/

        public void StartAuthentificate()
        {
            int port = FindAvailablePort(54000, 55000);
            if (port > 0)
            {
                m_server.Start(port);
                m_server.ConnectionAccepted += AccepteClient;
            }
            else
            {
                throw new Exception("Aucun port disponible n'a �t� trouv� dans la plage sp�cifi�e.");
            }
        }

        private int FindAvailablePort(int startRange, int endRange)
        {
            Random random = new Random();
            int port = random.Next(startRange, endRange + 1);
            bool isAvailable = false;

            // Essayez jusqu'� 10000 fois de trouver un port non utilis�.
            for (int i = 0; i < 10000 && !isAvailable; i++)
            {
                if (IsPortAvailable(port))
                {
                    isAvailable = true;
                }
                else
                {
                    port = random.Next(startRange, endRange + 1); // Essayez un nouveau port si le pr�c�dent est utilis�
                }
            }

            return isAvailable ? port : -1; // Retournez -1 si aucun port disponible n'est trouv�
        }

        private bool IsPortAvailable(int port)
        {
            TcpListener tcpListener = null;

            try
            {
                // Tente de lier le port
                tcpListener = new TcpListener(IPAddress.Loopback, port);
                tcpListener.Start();
                return true;
            }
            catch (SocketException)
            {
                return false; // Le port est probablement d�j� utilis�
            }
            finally
            {
                tcpListener?.Stop();
            }
        }

        private void AccepteClient(Socket client)
		{
			SimpleClient client2 = new SimpleClient(client);
			m_client = new UpClient(client2);
		}

		private void ClientDisconnected(object sender, UpClient.DisconnectedArgs e)
		{
			m_client = null;
		}
	}
}
