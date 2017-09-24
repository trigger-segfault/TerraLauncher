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

namespace TerraLauncher.Controls.Terraria {
	/// <summary>
	/// Interaction logic for TerrariaTooltip.xaml
	/// </summary>
	public partial class TerrariaTooltip : UserControl {
		public TerrariaTooltip() {
			InitializeComponent();
		}


		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(TerrariaTooltip), new
			PropertyMetadata("Tooltip"));

		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}
