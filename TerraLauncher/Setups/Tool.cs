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
	public class Tool : Setup {
		//========== PROPERTIES ==========
		#region Properties

		public override string Arguments { get; set; } = "";
		public string ProjectPath { get; set; } = "";
		public string ProjectDirectory {
			get { return Path.GetDirectoryName(ProjectPath); }
		}
		protected override string TypeName {
			get { return "Tool"; }
		}
		protected override string DefaultIcon {
			get { return "Tool"; }
		}
		public override SetupOption[] Options {
			get {
				List<SetupOption> options = new List<SetupOption>();
				options.Add(new SetupOption("Launch Tool", "Launch", Launch));
				options.Add(new SetupOption("Open Tool Folder", "Home", OpenExeFolder));

				if (!string.IsNullOrWhiteSpace(ProjectPath)) {
					options.Add(new SetupOption("Open Project", "Hammer", OpenProject));
					options.Add(new SetupOption("Open Project Folder", "Folder", OpenProjectFolder));
				}
				options.Add(new SetupOption("Edit Tool Setup", "Gear", EditTool));
				return options.ToArray();
			}
		}

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		public Tool() {
			Name = "New Tool";
			Icon = "Tool";
		}
		public override ISetup Clone() {
			Tool tool = new Tool();
			CloneBase(tool);
			tool.Arguments = tool.Arguments;
			tool.ProjectPath = ProjectPath;
			return tool;
		}

		#endregion
		//=========== LOADING ============
		#region Loading

		protected override void ReadSetup(XmlElement setup) {
			XmlNode node;

			node = setup.SelectSingleNode("Arguments");
			if (node != null) Arguments = node.InnerText;

			node = setup.SelectSingleNode("ProjectPath");
			if (node != null) ProjectPath = node.InnerText;
		}
		protected override void WriteSetup(XmlElement setup, XmlDocument doc) {
			XmlElement element;

			if (!string.IsNullOrWhiteSpace(Arguments)) {
				element = doc.CreateElement("Arguments");
				element.AppendChild(doc.CreateTextNode(Arguments));
				setup.AppendChild(element);
			}

			if (!string.IsNullOrWhiteSpace(ProjectPath)) {
				element = doc.CreateElement("ProjectPath");
				element.AppendChild(doc.CreateTextNode(ProjectPath));
				setup.AppendChild(element);
			}
		}

		#endregion
		//=========== OPTIONS ============
		#region Options
		
		public void OpenProject() {
			Sounds.PlayOpen();
			try {
				if (File.Exists(ProjectPath)) {
					ProcessStartInfo start = new ProcessStartInfo();
					start.FileName = ProjectPath;
					start.WindowStyle = ProcessWindowStyle.Normal;
					start.CreateNoWindow = true;
					start.UseShellExecute = true;
					start.WorkingDirectory = ExeDirectory;

					Process.Start(start);
				}
			}
			catch { }
		}
		public void OpenProjectFolder() {
			Sounds.PlayOpen();
			try {
				if (Directory.Exists(ProjectDirectory)) {
					Process.Start(ProjectDirectory);
				}
			}
			catch { }
		}
		public void EditTool() {
			if (EditToolWindow.ShowDialog(Config.MainWindow, this)) {
				Entry?.Update();
				Config.Modified = true;
				Config.SaveConfig();
			}
		}

		#endregion
	}
}
