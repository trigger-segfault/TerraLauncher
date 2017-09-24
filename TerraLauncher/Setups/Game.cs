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
using TerraLauncher.Controls.Terraria;
using TerraLauncher.Windows;

namespace TerraLauncher.Setups {
	public class Game : Setup {
		//========== PROPERTIES ==========
		#region Properties

		public string SaveDirectory { get; set; } = "Default";
		public bool IsTMod { get; set; } = false;
		public override string Arguments {
			get {
				if (SaveDirectory != "Default")
					return "-savedirectory \"" + SaveDirectory + "\"";
				return "";
			}
			set { }
		}
		protected override string TypeName {
			get { return "Game"; }
		}
		protected override string DefaultIcon {
			get { return "Tree"; }
		}
		public override SetupOption[] Options {
			get {
				List<SetupOption> options = new List<SetupOption>();
				options.Add(new SetupOption("Launch Game", "Launch", Launch));
				options.Add(new SetupOption("Open Save Folder", "Folder", OpenSaveFolder));
				options.Add(new SetupOption("Open Executable Folder", "Home", OpenExeFolder));
				options.Add(new SetupOption("Edit Game Setup", "Gear", EditGame));
				return options.ToArray();
			}
		}

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		public Game() {
			Name = "New Game";
			Icon = "Tree";
		}
		public override ISetup Clone() {
			Game game = new Game();
			CloneBase(game);
			game.SaveDirectory = SaveDirectory;
			game.IsTMod = IsTMod;
			return game;
		}

		#endregion
		//=========== LOADING ============
		#region Loading

		protected override void ReadSetup(XmlElement setup) {
			XmlNode node;
			XmlAttribute attribute;

			bool boolValue;
				
			node = setup.SelectSingleNode("SaveDirectory");
			if (node != null) {
				SaveDirectory = node.InnerText;
			}
			if (SaveDirectory == "")
				SaveDirectory = "Default";
			
			node = setup.SelectSingleNode("IsTMod");
			if (node != null && bool.TryParse(node.InnerText, out boolValue))
				IsTMod = boolValue;
		}
		protected override void WriteSetup(XmlElement setup, XmlDocument doc) {
			XmlElement element;
			
			element = doc.CreateElement("SaveDirectory");
			element.AppendChild(doc.CreateTextNode(SaveDirectory));
			setup.AppendChild(element);

			element = doc.CreateElement("IsTMod");
			element.AppendChild(doc.CreateTextNode(IsTMod.ToString()));
			setup.AppendChild(element);
		}

		#endregion
		//=========== OPTIONS ============
		#region Options

		public void OpenSaveFolder() {
			Sounds.PlayOpen();
			try {
				if (SaveDirectory == "Default") {
					string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria");
					if (IsTMod)
						path = Path.Combine(path, "ModLoader");
					Process.Start(path);
				}
				else if (Directory.Exists(SaveDirectory)) {
					Process.Start(SaveDirectory);
				}
			}
			catch { }
		}
		public void EditGame() {
			if (EditGameWindow.ShowDialog(Config.MainWindow, this)) {
				Entry?.Update();
				Config.Modified = true;
				Config.SaveConfig();
			}
		}

		#endregion
	}
}
