using Uplauncher.Sound.Game;
using Uplauncher.Sound.Reg;

namespace Uplauncher.Sound
{
	public class Initialization
	{
		public static GameServer gameServer = new GameServer();

		public static RegServer regServer = new RegServer();

		public static void Init()
		{
			gameServer.StartAuthentificate();
			regServer.StartAuthentificate();
		}
	}
}
