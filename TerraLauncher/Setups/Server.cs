using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using TerraLauncher.Windows;

namespace TerraLauncher.Setups {
	public class Server : Setup {
		//========== PROPERTIES ==========
		#region Properties

		public override string Arguments { get; set; } = "";
		public string WorldDirectory { get; set; } = "Default";
		public bool IsTMod { get; set; } = false;
		protected override string TypeName {
			get { return "Server"; }
		}
		protected override string DefaultIcon {
			get { return "Server"; }
		}
		public override SetupOption[] Options {
			get {
				List<SetupOption> options = new List<SetupOption>();
				options.Add(new SetupOption("Launch Server", "Launch", Launch));
				options.Add(new SetupOption("Open Worlds Folder", "Folder", OpenWorldsFolder));
				options.Add(new SetupOption("Open Server Folder", "Home", OpenExeFolder));
				options.Add(new SetupOption("Edit Server Setup", "Gear", EditServer));
				return options.ToArray();
			}
		}

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		public Server() {
			Name = "New Server";
			Icon = "Server";
		}
		public override ISetup Clone() {
			Server server = new Server();
			CloneBase(server);
			server.Arguments = server.Arguments;
			server.WorldDirectory = WorldDirectory;
			server.IsTMod = IsTMod;
			return server;
		}

		#endregion
		//=========== LOADING ============
		#region Loading

		protected override void ReadSetup(XmlElement setup) {
			XmlNode node;
			XmlAttribute attribute;

			bool boolValue;

			node = setup.SelectSingleNode("Arguments");
			if (node != null)
				Arguments = node.InnerText;

			node = setup.SelectSingleNode("WorldDirectory");
			if (node != null)
				WorldDirectory = node.InnerText;
			if (WorldDirectory == "")
				WorldDirectory = "Default";

			node = setup.SelectSingleNode("IsTMod");
			if (node != null && bool.TryParse(node.InnerText, out boolValue))
				IsTMod = boolValue;
		}
		protected override void WriteSetup(XmlElement setup, XmlDocument doc) {
			XmlElement element;
			
			if (!string.IsNullOrWhiteSpace(Arguments)) {
				element = doc.CreateElement("Arguments");
				element.AppendChild(doc.CreateTextNode(Arguments));
				setup.AppendChild(element);
			}

			if (!string.IsNullOrWhiteSpace(WorldDirectory)) {
				element = doc.CreateElement("WorldDirectory");
				element.AppendChild(doc.CreateTextNode(WorldDirectory));
				setup.AppendChild(element);
			}

			element = doc.CreateElement("IsTMod");
			element.AppendChild(doc.CreateTextNode(IsTMod.ToString()));
			setup.AppendChild(element);
		}

		#endregion
		//=========== OPTIONS ============
		#region Options
		
		public void OpenWorldsFolder() {
			Sounds.PlayOpen();
			try {
				if (WorldDirectory == "Default") {
					string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria");
					if (IsTMod)
						path = Path.Combine(path, "ModLoader");
					path = Path.Combine(path, "Worlds");
					Process.Start(path);
				}
				else if (Directory.Exists(WorldDirectory)) {
					Process.Start(WorldDirectory);
				}
			}
			catch { }
		}
		public void EditServer() {
			if (EditServerWindow.ShowDialog(Config.MainWindow, this)) {
				Entry?.Update();
				Config.Modified = true;
				Config.SaveConfig();
			}
		}

		#endregion
	}
}
