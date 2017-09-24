using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using TAFactory.IconPack;
using TerraLauncher.Controls.Terraria;

namespace TerraLauncher.Setups {
	public class SetupOption {
		public string Tooltip;
		public string Icon;
		public Action Action;

		public SetupOption(string tooltip, string icon, Action action) {
			Tooltip = tooltip;
			Icon = icon;
			Action = action;
		}
	}
	public interface ISetup {
		string Name { get; set; }
		string Details { get; set; }
		string Icon { get; set; }
		ISetup Clone();

		BitmapSource LoadIcon();
	}

	public abstract class Setup : ISetup {
		//========== PROPERTIES ==========
		#region Properties

		public TerrariaSetupEntry Entry { get; set; } = null;
		public string Name { get; set; } = "";
		public string Icon {
			get { return icon; }
			set {
				icon = value;
				loadedIcon = null;
			}
		}
		public string Details { get; set; } = "";
		public string ExePath { get; set; } = "";
		public string ExeDirectory {
			get { return Path.GetDirectoryName(ExePath); }
		}
		public abstract string Arguments { get; set; }
		protected abstract string TypeName { get; }
		protected abstract string DefaultIcon { get; }

		public abstract SetupOption[] Options { get; }

		public static Dictionary<string, BitmapImage> SetupIcons { get; } = new Dictionary<string, BitmapImage>();
		public static Dictionary<string, BitmapImage> SetupOptions { get; } = new Dictionary<string, BitmapImage>();


		#endregion
		//=========== MEMBERS ============
		#region Members

		private string icon = "";
		protected BitmapSource loadedIcon = null;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		static Setup() {
			AddIcon("Tree");
			AddIcon("TreeJungle");
			AddIcon("TreeCorruption");
			AddIcon("TreeCrimson");
			AddIcon("TreeCorruptionHallow");
			AddIcon("TreeCrimsonHallow");
			AddIcon("Server");
			AddIcon("ServerTree");
			AddIcon("ServerTreeJungle");
			AddIcon("TShock");
			AddIcon("Tool");
			AddIcon("Folder");

			AddOptionIcon("Launch");
			AddOptionIcon("Folder");
			AddOptionIcon("Home");
			AddOptionIcon("Hammer");
			AddOptionIcon("Key");
			AddOptionIcon("Gear");
			AddOptionIcon("FolderEnter");
			AddOptionIcon("FolderLeave");
		}
		public abstract ISetup Clone();
		protected void CloneBase(Setup setup) {
			setup.Name = Name;
			setup.Icon = Icon;
			setup.Details = Details;
			setup.ExePath = ExePath;
		}

		#endregion
		//=========== HELPERS ============
		#region Helpers

		private static void AddIcon(string name) {
			SetupIcons.Add(name, new BitmapImage(new Uri("pack://application:,,,/Resources/Terraria/SetupIcons/SetupIcon" + name + ".png")));
		}
		private static void AddOptionIcon(string name) {
			SetupOptions.Add(name, new BitmapImage(new Uri("pack://application:,,,/Resources/Terraria/SetupOptions/SetupOption" + name + ".png")));
		}

		#endregion
		//=========== LOADING ============
		#region Loading

		public void Read(XmlElement setup) {
			XmlNode node;

			node = setup.SelectSingleNode("Name");
			if (node != null) Name = node.InnerText;

			node = setup.SelectSingleNode("Details");
			if (node != null) Details = node.InnerText;

			node = setup.SelectSingleNode("Icon");
			if (node != null) Icon = node.InnerText;

			node = setup.SelectSingleNode("ExePath");
			if (node != null) ExePath = node.InnerText;

			ReadSetup(setup);
		}
		public void Write(XmlElement setup, XmlDocument doc) {
			XmlElement element;

			element = doc.CreateElement("Name");
			element.AppendChild(doc.CreateTextNode(Name));
			setup.AppendChild(element);
			
			element = doc.CreateElement("Details");
			element.AppendChild(doc.CreateTextNode(Details));
			setup.AppendChild(element);
			
			element = doc.CreateElement("Icon");
			element.AppendChild(doc.CreateTextNode(Icon));
			setup.AppendChild(element);

			element = doc.CreateElement("ExePath");
			element.AppendChild(doc.CreateTextNode(ExePath));
			setup.AppendChild(element);

			WriteSetup(setup, doc);
		}
		protected abstract void WriteSetup(XmlElement setup, XmlDocument doc);
		protected abstract void ReadSetup(XmlElement setup);

		#endregion
		//=========== OPTIONS ============
		#region Options

		public void Launch() {
			Sounds.PlayOpen();
			try {
				if (File.Exists(ExePath)) {
					ProcessStartInfo start = new ProcessStartInfo();
					start.FileName = ExePath;
					start.Arguments = Arguments;
					start.WindowStyle = ProcessWindowStyle.Normal;
					start.CreateNoWindow = true;
					start.UseShellExecute = true;
					start.WorkingDirectory = ExeDirectory;

					Process proc = Process.Start(start);

					bool close = false;
					switch (TypeName) {
					case "Game": close = Config.CloseOnGameLaunch; break;
					case "Server": close = Config.CloseOnServerLaunch; break;
					case "Tool": close = Config.CloseOnToolLaunch; break;
					}
					
					bool ctrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
					bool shift = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
					if (proc != null && (close || ctrl) && !shift)
						Config.MainWindow.Close();
				}
			}
			catch { }
		}
		public void OpenExeFolder() {
			Sounds.PlayOpen();
			try {
				if (Directory.Exists(ExeDirectory)) {
					Process.Start(ExeDirectory);
				}
			}
			catch { }
		}

		public virtual BitmapSource LoadIcon() {
			if (loadedIcon == null)
				loadedIcon = LoadIcon(Icon, DefaultIcon);
			return loadedIcon;
		}
		public static BitmapSource GetOptionIcon(string name) {
			return SetupOptions[name];
		}
		
		public static BitmapSource LoadIcon(string icon, string defaultIcon = "Tree") {
			if (string.IsNullOrWhiteSpace(icon)) {
				return SetupIcons[defaultIcon];
			}
			else if (SetupIcons.ContainsKey(icon)) {
				return SetupIcons[icon];
			}
			try {
				return LoadIconFromFile(icon);
			}
			catch {
				return SetupIcons[defaultIcon];
			}
		}
		public static BitmapSource LoadFolderIcon(string icon) {
			if (string.IsNullOrWhiteSpace(icon)) {
				return SetupIcons["Folder"];
			}
			else if (SetupIcons.ContainsKey(icon)) {
				return SetupIcons[icon];
			}
			try {
				return LoadIconFromFile(icon);
			}
			catch {
				return SetupIcons["Folder"];
			}
		}

		private static BitmapSource LoadIconFromFile(string filePath) {
			string ext = Path.GetExtension(filePath).ToLower();
			if (ext == ".exe" || ext == ".ico") {
				Icon icon = null;
				if (ext == ".exe") {
					IconExtractor extractor = new IconExtractor(filePath);
					icon = extractor.GetIconAt(0);
					icon = IconHelper.GetBestFitIcon(icon, new System.Drawing.Size(48, 48));
				}
				else {
					icon = new System.Drawing.Icon(filePath);
					icon = IconHelper.GetBestFitIcon(icon, new System.Drawing.Size(48, 48));
				}
				
				BitmapSource bitmap = Imaging.CreateBitmapSourceFromHIcon(
					icon.Handle,
					new Int32Rect(0, 0, icon.Width, icon.Height),
					BitmapSizeOptions.FromEmptyOptions()
				);

				// Resize if needed
				int width = bitmap.PixelWidth;
				int height = bitmap.PixelHeight;
				// Just incase the icon is having an identity crisis and isn't square
				int maxDimension = Math.Max(width, height);
				if (maxDimension > 48) {
					bitmap = CreateResizedImage(bitmap, width*48/maxDimension, height*48/maxDimension);
				}

				return bitmap;
			}
			else {
				BitmapImage bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.CacheOption = BitmapCacheOption.OnLoad;
				bitmap.UriSource = new Uri(filePath);
				bitmap.EndInit();
				return bitmap;
			}
		}

		// https://stackoverflow.com/a/15779942/7517185
		private static BitmapFrame CreateResizedImage(BitmapSource source, int width, int height, int margin = 0) {
			var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);
			
			var group = new DrawingGroup();
			RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
			group.Children.Add(new ImageDrawing(source, rect));

			var drawingVisual = new DrawingVisual();
			using (var drawingContext = drawingVisual.RenderOpen())
				drawingContext.DrawDrawing(group);

			var resizedImage = new RenderTargetBitmap(
				width, height,         // Resized dimensions
				96, 96,                // Default DPI values
				PixelFormats.Default); // Default pixel format
			resizedImage.Render(drawingVisual);

			return BitmapFrame.Create(resizedImage);
		}


		#endregion
	}
}
