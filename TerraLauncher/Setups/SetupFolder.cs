using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;
using TerraLauncher.Controls.Terraria;
using TerraLauncher.Windows;

namespace TerraLauncher.Setups {
	public class SetupFolder : ISetup {
		//========== PROPERTIES ==========
		#region Properties

		public TerrariaSetupFolder Entry { get; set; } = null;
		public string Name { get; set; } = "Folder";
		public string Details { get; set; } = "";
		public string Icon { get; set; } = "Folder";
		public SetupFolder Parent { get; set; } = null;
		public List<ISetup> Entries { get; } = new List<ISetup>();

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		public SetupFolder() {
			Name = "Root";
		}
		public SetupFolder(string name) {
			Name = name;
		}
		public SetupFolder(SetupFolder parent) {
			Parent = parent;
		}
		public ISetup Clone() {
			SetupFolder folder = new SetupFolder();
			folder.Name = Name;
			folder.Icon = Icon;
			foreach (ISetup entry in Entries) {
				folder.Entries.Add(entry.Clone());
			}
			return folder;
		}
		public SetupFolder CloneFolder() {
			return (SetupFolder)Clone();
		}

		#endregion
		//=========== LOADING ============
		#region Loading

		public void Read<T>(XmlElement folder) where T : Setup {
			XmlNode node;
			XmlElement element;
			
			Entries.Clear();

			if (Parent != null) {
				node = folder.SelectSingleNode("Name");
				if (node != null) Name = node.InnerText;

				node = folder.SelectSingleNode("Details");
				if (node != null) Details = node.InnerText;

				node = folder.SelectSingleNode("Icon");
				if (node != null) Icon = node.InnerText;
			}

			foreach (XmlNode folderNode in folder) {
				element = folderNode as XmlElement;
				if (element != null) {
					if (element.Name == "Folder") {
						SetupFolder subFolder = new SetupFolder(this);
						subFolder.Read<T>(element);
						Entries.Add(subFolder);
					}
					else if (element.Name == typeof(T).Name) {
						Setup setup = Activator.CreateInstance<T>();
						setup.Read(element);
						Entries.Add(setup);
					}
				}
			}
		}
		public void Write<T>(XmlElement folder, XmlDocument doc) {
			XmlElement element;

			if (Parent != null) {
				element = doc.CreateElement("Name");
				element.AppendChild(doc.CreateTextNode(Name));
				folder.AppendChild(element);

				element = doc.CreateElement("Details");
				element.AppendChild(doc.CreateTextNode(Details));
				folder.AppendChild(element);

				element = doc.CreateElement("Icon");
				element.AppendChild(doc.CreateTextNode(Icon));
				folder.AppendChild(element);
			}

			foreach (ISetup entry in Entries) {
				if (entry is SetupFolder) {
					SetupFolder subFolder = (SetupFolder)entry;
					XmlElement subFolderElement = doc.CreateElement("Folder");
					subFolder.Write<T>(subFolderElement, doc);
					folder.AppendChild(subFolderElement);
				}
				else if (entry is Setup) {
					Setup setup = (Setup)entry;
					XmlElement setupElement = doc.CreateElement(typeof(T).Name);
					setup.Write(setupElement, doc);
					folder.AppendChild(setupElement);
				}
			}
		}

		#endregion

		public BitmapSource LoadIcon() {
			return Setup.LoadFolderIcon(Icon);
		}
		public void EditFolder() {
			if (EditFolderWindow.ShowDialog(Config.MainWindow, this)) {
				Entry?.Update();
				Config.Modified = true;
				Config.SaveConfig();
			}
		}

	}
}
