using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace TerraLauncher.Controls.Terraria {
	/// <summary>
	/// Interaction logic for TerrariaConfig.xaml
	/// </summary>
	public partial class TerrariaSetupEntry : UserControl {

		private Setup setup;

		public TerrariaSetupEntry(Setup setup) {
			InitializeComponent();

			setup.Entry = this;
			this.setup = setup;
			foreach (SetupOption option in setup.Options) {
				TerrariaSetupOptionButton button = new TerrariaSetupOptionButton(option);
				stackPanelOptions.Children.Add(button);
			}
			labelName.Content = setup.Name;
			labelDetails.Content = setup.Details;
			BitmapSource bitmap = setup.LoadIcon();
			imageIcon.Source = bitmap;
			imageIcon.Width = Math.Min(68, bitmap.PixelWidth);
			imageIcon.Height = Math.Min(68, bitmap.PixelHeight);
		}
		
		protected override void OnRender(DrawingContext d) {
			CroppedFrame frame = CroppedFrames.SetupFrame;
			if (IsFocused)
				frame = CroppedFrames.SetupFrameFocused;

			DrawCropped.DrawFrame(d, CroppedFrames.SetupFrame, ActualWidth, ActualHeight);
			base.OnRender(d);
		}

		public void Launch() {
			((TerrariaSetupOptionButton)stackPanelOptions.Children[0]).Action();
		}

		private void OnGotFocus(object sender, RoutedEventArgs e) {

		}

		private void OnLostFocus(object sender, RoutedEventArgs e) {

		}

		public void Update() {
			stackPanelOptions.Children.Clear();
			foreach (SetupOption option in setup.Options) {
				TerrariaSetupOptionButton button = new TerrariaSetupOptionButton(option);
				stackPanelOptions.Children.Add(button);
			}
			labelName.Content = setup.Name;
			labelDetails.Content = setup.Details;
			BitmapSource bitmap = setup.LoadIcon();
			imageIcon.Source = bitmap;
			imageIcon.Width = Math.Min(68, bitmap.PixelWidth);
			imageIcon.Height = Math.Min(68, bitmap.PixelHeight);
		}
	}
}
