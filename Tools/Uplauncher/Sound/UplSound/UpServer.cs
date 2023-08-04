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

		public void StartAuthentificate()
		{
			m_server.Start(4242);
			m_server.ConnectionAccepted += AccepteClient;
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
