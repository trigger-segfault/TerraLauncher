using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TerraLauncher.Properties;

namespace TerraLauncher {
	public static class Sounds {

		public static readonly string TempPath = Path.Combine(Path.GetTempPath(), "TriggersToolsGames", "TerrariaLauncher");

		private static SoundPlayer Tick;
		private static SoundPlayer Open;
		private static SoundPlayer Close;

		static Sounds() {
			Tick = new SoundPlayer(Resources.MenuTick);
			Open = new SoundPlayer(Resources.MenuOpen);
			Close = new SoundPlayer(Resources.MenuClose);
		}

		public static void PlayTick() {
			if (!Config.Muted)
				Tick.Play();
		}
		public static void PlayOpen() {
			if (!Config.Muted)
				Open.Play();
		}
		public static void PlayClose() {
			if (!Config.Muted)
				Close.Play();
		}
	}
}
