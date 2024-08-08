using System;
using Uplauncher.Sound.Network;

namespace Uplauncher.Sound.UplSound
{
	public class UpClient
	{
		public class DisconnectedArgs : EventArgs
		{
			public UpClient Host
			{
				get;
				private set;
			}

			public DisconnectedArgs(UpClient host)
			{
				Host = host;
			}
		}

		private SimpleClient m_client;

		public event EventHandler<DisconnectedArgs> Disconnected;

		public UpClient(SimpleClient client)
		{
			m_client = client;
			if (client != null)
			{
				m_client.DataReceived += ClientDataReceive;
				m_client.Disconnected += ClientDisconnected;
			}
		}

		public void Dipose()
		{
			m_client.DataReceived -= ClientDataReceive;
			m_client.Disconnected -= ClientDisconnected;
			m_client.Stop();
		}

		private void ClientDataReceive(object sender, SimpleClient.DataReceivedEventArgs e)
		{
		}

		private void ClientDisconnected(object sender, SimpleClient.DisconnectedEventArgs e)
		{
			OnDisconnected(new DisconnectedArgs(this));
		}

		private void OnDisconnected(DisconnectedArgs e)
		{
			if (Disconnected != null)
			{
				Disconnected(this, e);
			}
		}

		public void Send(byte[] data)
		{
			if (m_client.Runing)
			{
				m_client.Send(data);
			}
		}
	}
}
