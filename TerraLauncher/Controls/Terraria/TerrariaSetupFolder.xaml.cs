using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TerraLauncher.Setups;
using TerraLauncher.Windows;

namespace TerraLauncher.Controls.Terraria {
	/// <summary>
	/// Interaction logic for TerrariaSetupFolder.xaml
	/// </summary>
	public partial class TerrariaSetupFolder : UserControl {

		private SetupFolder folder;
		private Action navigate;
		

		public TerrariaSetupFolder(SetupFolder folder, bool isParent, Action navigate) {
			InitializeComponent();

			folder.Entry = this;
			this.navigate = navigate;
			this.folder = folder;
			SetupOption option = new SetupOption("", "", navigate);

			if (isParent) {
				option.Tooltip = "Go back to the parent folder";
				option.Icon = "FolderLeave";
				labelName.Content = "Go Back";
				labelEntries.Content = "Parent: " + folder.Name;
			}
			else {
				option.Tooltip = "Open the subfolder";
				option.Icon = "FolderEnter";
				labelName.Content = folder.Name;
				labelEntries.Content = "Folder: " + folder.Entries.Count + " Entries";
				
				TerrariaSetupOptionButton button2 = new TerrariaSetupOptionButton(option);
				stackPanelOptions.Children.Add(button2);

				option = new SetupOption("Edit Folder", "Gear", folder.EditFolder);
			}

			TerrariaSetupOptionButton button = new TerrariaSetupOptionButton(option);
			stackPanelOptions.Children.Add(button);

			BitmapSource bitmap = folder.LoadIcon();
			imageIcon.Source = bitmap;
			imageIcon.Width = Math.Min(68, bitmap.PixelWidth);
			imageIcon.Height = Math.Min(68, bitmap.PixelHeight);
		}

		protected override void OnRender(DrawingContext d) {
			DrawCropped.DrawFrame(d, CroppedFrames.SetupFrame, ActualWidth, ActualHeight);
			base.OnRender(d);
		}

		public void Launch() {
			((TerrariaSetupOptionButton)stackPanelOptions.Children[0]).Action();
		}

		public void Update() {
			SetupOption option = new SetupOption("", "", navigate);

			labelName.Content = folder.Name;

			BitmapSource bitmap = folder.LoadIcon();
			imageIcon.Source = bitmap;
			imageIcon.Width = Math.Min(68, bitmap.PixelWidth);
			imageIcon.Height = Math.Min(68, bitmap.PixelHeight);
		}
	}
}
