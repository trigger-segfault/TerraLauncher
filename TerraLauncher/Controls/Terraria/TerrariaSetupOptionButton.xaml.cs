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

namespace TerraLauncher.Controls.Terraria {
	/// <summary>
	/// Interaction logic for SetupOption.xaml
	/// </summary>
	public partial class TerrariaSetupOptionButton : UserControl {
		private SetupOption option;
		public TerrariaSetupOptionButton(SetupOption option) {
			InitializeComponent();
			this.option = option;
			tooltip.Text = option.Tooltip;
			BitmapSource bitmap = Setup.GetOptionIcon(option.Icon);
			image.Source = bitmap;
			image.Width = Math.Min(28, bitmap.PixelWidth);
			image.Height = Math.Min(28, bitmap.PixelHeight);
		}

		private void OnButton(object sender, MouseButtonEventArgs e) {
			option.Action();
		}
		public void Action() {
			option.Action();
		}
	}
}
