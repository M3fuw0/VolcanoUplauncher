using System;
using System.Net;
using System.Net.Sockets;

namespace Uplauncher.Sound.Network
{
	public class SimpleServer
	{
		public delegate void ConnectionAcceptedDelegate(Socket acceptedSocket);

		private Socket socketListener;

		private bool runing = false;

		public bool Connected => runing;

		public event ConnectionAcceptedDelegate ConnectionAccepted;

		public SimpleServer()
		{
			socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public void Start(int listenPort)
		{
			if (!runing)
			{
				runing = true;
				socketListener.Bind(new IPEndPoint(IPAddress.Any, listenPort));
				socketListener.Listen(5);
				socketListener.BeginAccept(BeiginAcceptCallBack, socketListener);
			}
		}

		public void Stop()
		{
			runing = false;
			socketListener.Shutdown(SocketShutdown.Both);
		}

		private void BeiginAcceptCallBack(IAsyncResult result)
		{
			if (runing)
			{
				Socket socket = (Socket)result.AsyncState;
				Socket client = socket.EndAccept(result);
				OnConnectionAccepted(client);
				socketListener.BeginAccept(BeiginAcceptCallBack, socketListener);
			}
		}

		private void OnConnectionAccepted(Socket client)
		{
			ConnectionAccepted?.Invoke(client);
		}
	}
}
