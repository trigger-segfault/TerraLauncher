using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TerraLauncher.Setups;
using TerraLauncher.Util;

namespace TerraLauncher {
	public enum SetupTypes {
		Game = 0,
		Server = 1,
		Tool = 2
	}

	public static class Config {

		public const int ConfigVersion = 1;
		public const string ConfigName = "TerraLauncher.xml";
		public static readonly string ConfigPath = Path.Combine(
			Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
			ConfigName
		);

		public static MainWindow MainWindow { get; private set; }

		public static bool CloseOnGameLaunch { get; set; } = true;
		public static bool CloseOnServerLaunch { get; set; } = false;
		public static bool CloseOnToolLaunch { get; set; } = false;

		public static bool DisableTransitions { get; set; } = false;
		public static bool Muted { get; set; } = false;
		public static bool Integration { get; set; } = true;
		public static double ScrollSpeed { get; set; } = 1.0;

		public static bool Modified { get; set; } = false;

		public static SetupFolder Games { get; set; } = new SetupFolder("Game List");
		public static SetupFolder Servers { get; set; } = new SetupFolder("Server List");
		public static SetupFolder Tools { get; set; } = new SetupFolder("Tool List");

		public static bool LoadConfig(MainWindow mainWindow) {
			try {
				MainWindow = mainWindow;

				if (!File.Exists(ConfigPath) && !string.IsNullOrEmpty(TerrariaLocator.TerrariaPath)) {
					string path = TerrariaLocator.TerrariaPath;
					Game game = new Game();
					game.Name = "Terraria";
					game.Icon = "Tree";
					game.ExePath = path;
					
					string version =FileVersionInfo.GetVersionInfo(path).FileVersion.ToString();
					if (!string.IsNullOrEmpty(version))
						game.Details = "v" + version;
					Games.Entries.Add(game);

					path = Path.Combine(Path.GetDirectoryName(path), "TerrariaServer.exe");
					Server server = new Server();
					server.Name = "Terraria Server";
					server.Icon = "ServerTree";
					server.ExePath = path;
					version = FileVersionInfo.GetVersionInfo(path).FileVersion.ToString();
					if (!string.IsNullOrEmpty(version))
						server.Details = "v" + version;
					Servers.Entries.Add(server);

					SaveConfig();
					return false;
				}

				XmlNode node;
				XmlElement element;
				XmlAttribute attribute;
				XmlDocument doc = new XmlDocument();
				doc.Load(ConfigPath);

				int intValue;
				ushort ushortValue;
				bool boolValue;
				double doubleValue;

				node = doc.SelectSingleNode("TerraLauncher/Version");
				if (node != null && int.TryParse(node.InnerText, out intValue) && (intValue > ConfigVersion || intValue <= 0))
					return false;
				
				#region Settings

				node = doc.SelectSingleNode("TerraLauncher/CloseOnLaunch");
				if (node != null) {
					attribute = node.Attributes["Game"];
					if (attribute != null && bool.TryParse(attribute.InnerText, out boolValue))
						CloseOnGameLaunch = boolValue;

					attribute = node.Attributes["Server"];
					if (attribute != null && bool.TryParse(attribute.InnerText, out boolValue))
						CloseOnServerLaunch = boolValue;

					attribute = node.Attributes["Tool"];
					if (attribute != null && bool.TryParse(attribute.InnerText, out boolValue))
						CloseOnToolLaunch = boolValue;
				}

				node = doc.SelectSingleNode("TerraLauncher/DisableTransitions");
				if (node != null && bool.TryParse(node.InnerText, out boolValue))
					DisableTransitions = boolValue;

				node = doc.SelectSingleNode("TerraLauncher/Muted");
				if (node != null && bool.TryParse(node.InnerText, out boolValue))
					Muted = boolValue;

				node = doc.SelectSingleNode("TerraLauncher/Integration");
				if (node != null && bool.TryParse(node.InnerText, out boolValue))
					Integration = boolValue;

				node = doc.SelectSingleNode("TerraLauncher/ScrollSpeed");
				if (node != null && double.TryParse(node.InnerText, out doubleValue))
					ScrollSpeed = doubleValue;

				#endregion
				//--------------------------------
				#region Games/Servers/Tools

				XmlElement gameFolder = doc.SelectSingleNode("TerraLauncher/Games") as XmlElement;
				if (gameFolder != null) {
					Games.Read<Game>(gameFolder);
				}

				XmlElement serverFolder = doc.SelectSingleNode("TerraLauncher/Servers") as XmlElement;
				if (serverFolder != null) {
					Servers.Read<Server>(serverFolder);
				}

				XmlElement toolFolder = doc.SelectSingleNode("TerraLauncher/Tools") as XmlElement;
				if (toolFolder != null) {
					Tools.Read<Tool>(toolFolder);
				}

				#endregion
			}
			catch (Exception) {
				return false;
			}
			return true;
		}

		public static bool SaveConfig() {
			try {
				XmlElement element;
				XmlDocument doc = new XmlDocument();
				doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

				XmlElement launcher = doc.CreateElement("TerraLauncher");
				doc.AppendChild(launcher);

				XmlElement version = doc.CreateElement("Version");
				version.AppendChild(doc.CreateTextNode(ConfigVersion.ToString()));
				launcher.AppendChild(version);

				#region Settings

				element = doc.CreateElement("CloseOnLaunch");
				element.SetAttribute("Game", CloseOnGameLaunch.ToString());
				element.SetAttribute("Server", CloseOnServerLaunch.ToString());
				element.SetAttribute("Tool", CloseOnToolLaunch.ToString());
				launcher.AppendChild(element);
				
				element = doc.CreateElement("DisableTransitions");
				element.AppendChild(doc.CreateTextNode(DisableTransitions.ToString()));
				launcher.AppendChild(element);

				element = doc.CreateElement("Muted");
				element.AppendChild(doc.CreateTextNode(Muted.ToString()));
				launcher.AppendChild(element);

				element = doc.CreateElement("Integration");
				element.AppendChild(doc.CreateTextNode(Integration.ToString()));
				launcher.AppendChild(element);

				element = doc.CreateElement("ScrollSpeed");
				element.AppendChild(doc.CreateTextNode(ScrollSpeed.ToString()));
				launcher.AppendChild(element);

				#endregion
				//--------------------------------
				#region Games/Servers/Tools

				element = doc.CreateElement("Games");
				Games.Write<Game>(element, doc);
				launcher.AppendChild(element);

				element = doc.CreateElement("Servers");
				Servers.Write<Server>(element, doc);
				launcher.AppendChild(element);

				element = doc.CreateElement("Tools");
				Tools.Write<Tool>(element, doc);
				launcher.AppendChild(element);

				#endregion

				doc.Save(ConfigPath);

				Modified = false;
			}
			catch (Exception) {
				return false;
			}
			return true;
		}

	}
}
