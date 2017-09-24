using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using TerraLauncher.Properties;
using TerraLauncher.Setups;
using TerraLauncher.Util;

namespace TerraLauncher.Windows {
	/// <summary>
	/// Interaction logic for EditSetupsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window {
		
		public SettingsWindow(SetupTypes startupTab) {
			InitializeComponent();

			tabControl.SelectedIndex = (int)startupTab;
			
			treeViewGames.Populate(Config.Games, SetupTypes.Game);
			treeViewServers.Populate(Config.Servers, SetupTypes.Server);
			treeViewTools.Populate(Config.Tools, SetupTypes.Tool);

			int width = Settings.Default.SettingsWidth;
			int height = Settings.Default.SettingsHeight;
			if (width >= MinWidth)
				Width = width;
			if (height >= MinHeight)
				Height = height;

			checkBoxCloseGame.IsChecked = Config.CloseOnGameLaunch;
			checkBoxCloseServer.IsChecked = Config.CloseOnServerLaunch;
			checkBoxCloseTool.IsChecked = Config.CloseOnToolLaunch;

			checkBoxDisableTransitions.IsChecked = Config.DisableTransitions;
			checkBoxMuted.IsChecked = Config.Muted;
			checkBoxIntegration.IsChecked = Config.Integration;

			spinnerScrollSpeed.Value = (int)(Config.ScrollSpeed * 100);
		}

		public static bool ShowDialog(Window owner, SetupTypes startupTab) {
			SettingsWindow window = new SettingsWindow(startupTab);
			window.Owner = owner;
			var result = window.ShowDialog();

			Settings.Default.SettingsWidth = (int)window.Width;
			Settings.Default.SettingsHeight = (int)window.Height;

			if (result.HasValue && result.Value) {
				if (window.treeViewGames.Modified) {
					Config.Games = window.treeViewGames.GenerateHierarchy();
					Config.Modified = true;
				}
				if (window.treeViewServers.Modified) {
					Config.Servers = window.treeViewServers.GenerateHierarchy();
					Config.Modified = true;
				}
				if (window.treeViewTools.Modified) {
					Config.Tools = window.treeViewTools.GenerateHierarchy();
					Config.Modified = true;
				}
				Config.CloseOnGameLaunch = window.checkBoxCloseGame.IsChecked.Value;
				Config.CloseOnServerLaunch = window.checkBoxCloseServer.IsChecked.Value;
				Config.CloseOnToolLaunch = window.checkBoxCloseTool.IsChecked.Value;

				Config.DisableTransitions = window.checkBoxDisableTransitions.IsChecked.Value;
				Config.Muted = window.checkBoxMuted.IsChecked.Value;
				Config.Integration = window.checkBoxIntegration.IsChecked.Value;
				Config.ScrollSpeed = (double)window.spinnerScrollSpeed.Value / 100.0;

				Config.SaveConfig();
				return true;
			}
			return false;
		}

		private void OnOKClicked(object sender, RoutedEventArgs e) {
			DialogResult = true;
		}

		//============ EVENTS ============
		#region Events
		//--------------------------------
		#region Menu Items
		
		private void OnAbout(object sender, RoutedEventArgs e) {
			AboutWindow.Show(this);
		}
		private void OnHelp(object sender, RoutedEventArgs e) {
			Process.Start("https://github.com/trigger-death/TerraLauncher/wiki");
		}
		private void OnCredits(object sender, RoutedEventArgs e) {
			CreditsWindow.Show(this);
		}
		private void OnViewOnGitHub(object sender, RoutedEventArgs e) {
			Process.Start("https://github.com/trigger-death/TerraLauncher");
		}

		#endregion
		//--------------------------------
		#endregion
	}
}
