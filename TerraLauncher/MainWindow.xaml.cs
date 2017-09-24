using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TerraLauncher.Windows;
using TerraLauncher.Properties;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using TerraLauncher.Controls.Terraria;
using TerraLauncher.Setups;
using System.ComponentModel;

namespace TerraLauncher {
	/**<summary>The main window running Terraria Item Modifier.</summary>*/
	public partial class MainWindow : Window {
		//========== CONSTANTS ===========
		#region Constants



		#endregion
		//=========== MEMBERS ============
		#region Members

		bool loaded = false;
		Stack<TerrariaSetupList> gameStack = new Stack<TerrariaSetupList>();
		Stack<TerrariaSetupList> serverStack = new Stack<TerrariaSetupList>();
		Stack<TerrariaSetupList> toolStack = new Stack<TerrariaSetupList>();
		SetupTypes currentTab = SetupTypes.Game;

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		private Stack<TerrariaSetupList> CurrentSetupStack {
			get {
				switch (currentTab) {
				case SetupTypes.Game: return gameStack;
				case SetupTypes.Server: return serverStack;
				case SetupTypes.Tool: return toolStack;
				}
				return null;
			}
		}
		private SetupFolder CurrentSetupList {
			get {
				switch (currentTab) {
				case SetupTypes.Game: return Config.Games;
				case SetupTypes.Server: return Config.Servers;
				case SetupTypes.Tool: return Config.Tools;
				}
				return null;
			}
		}
		private Grid CurrentSetupGrid {
			get {
				switch (currentTab) {
				case SetupTypes.Game: return gridGames;
				case SetupTypes.Server: return gridServers;
				case SetupTypes.Tool: return gridTools;
				}
				return null;
			}
		}
		
		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the main window.</summary>*/
		public MainWindow() {
			InitializeComponent();

			LoadSettings();
			Opacity = 0;

			// Setup Config Path key for Trigger Tool integration
			RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
			key = key.CreateSubKey("TriggersToolsGames");
			key = key.CreateSubKey("TerraLauncher");
			key.SetValue("ConfigPath", Config.ConfigPath);
		}

		#endregion
		//=========== SETTINGS ===========
		#region Settings

		/**<summary>Loads the application settings.</summary>*/
		private void LoadSettings() {
			Config.LoadConfig(this);

			LoadSetups();

			int width = Settings.Default.WindowWidth;
			int height = Settings.Default.WindowHeight;
			if (width >= MinWidth)
				Width = width;
			if (height >= MinHeight)
				Height = height;

			SetupTypes setupTabsValue;
			if (Enum.TryParse(Settings.Default.CurrentTab, out setupTabsValue)) {
				currentTab = setupTabsValue;
			}
			UpdateTab();
			UpdateFolder();
		}
		/**<summary>Saves the application settings.</summary>*/
		private void SaveSettings() {
			if (Config.Modified)
				Config.SaveConfig();

			Settings.Default.WindowWidth = (int)Width;
			Settings.Default.WindowHeight = (int)Height;
			Settings.Default.CurrentTab = currentTab.ToString();
			Settings.Default.Save();
		}

		#endregion
		//=========== HELPERS ============
		#region Helpers

		private void ReloadSetups() {
			RemoveSetups(gridGames, gameStack);
			RemoveSetups(gridServers, serverStack);
			RemoveSetups(gridTools, toolStack);
			LoadSetups();
		}
		private void RemoveSetups(Grid gridList, Stack<TerrariaSetupList> stack) {
			gridList.Children.Clear();
			stack.Clear();
		}

		private void LoadSetups() {
			TerrariaSetupList setupList = new TerrariaSetupList();
			setupList.PopulateList(Config.Games, (folder) => {
				NavigateForward(gridGames, gameStack, folder);
			});
			gameStack.Push(setupList);
			gridGames.Children.Add(setupList);
			setupList = new TerrariaSetupList();
			setupList.PopulateList(Config.Servers, (folder) => {
				NavigateForward(gridServers, serverStack, folder);
			});
			serverStack.Push(setupList);
			gridServers.Children.Add(setupList);
			setupList = new TerrariaSetupList();
			setupList.PopulateList(Config.Tools, (folder) => {
				NavigateForward(gridTools, toolStack, folder);
			});
			toolStack.Push(setupList);
			gridTools.Children.Add(setupList);
		}

		private void NavigateForward(Grid gridList, Stack<TerrariaSetupList> stack, SetupFolder folder) {
			TerrariaSetupList setupList = new TerrariaSetupList();
			setupList.PopulateList(folder, (folder2) => {
				NavigateForward(gridList, stack, folder2);
			}, () => {
				NavigateBack(gridList, stack);
			});
			var last = stack.Peek();
			stack.Push(setupList);
			gridList.Children.Add(setupList);
			if (!Config.DisableTransitions) {
				last.LeaveFolder(false, gridList.ActualWidth);
				setupList.EnterFolder(false, gridList.ActualWidth);
			}
			else {
				last.Visibility = Visibility.Hidden;
			}
			UpdateFolder();
		}
		private void NavigateBack(Grid gridList, Stack<TerrariaSetupList> stack) {
			var last = stack.Pop();
			var setupList = stack.Peek();
			if (!Config.DisableTransitions) {
				last.LeaveFolder(true, gridList.ActualWidth);
				setupList.EnterFolder(true, gridList.ActualWidth);
			}
			else {
				gridList.Children.Remove(last);
				setupList.Visibility = Visibility.Visible;
			}
			UpdateFolder();
		}
		
		#endregion
		//============ EVENTS ============
		#region Events
		
		private void OnWindowLoaded(object sender, RoutedEventArgs e) {
			Sounds.PlayOpen();
			var anim = new DoubleAnimation(0, 1, (Duration)TimeSpan.FromSeconds(0.4));
			anim.Completed += (s, _) => { loaded = true; } ;
			this.BeginAnimation(UIElement.OpacityProperty, anim);
		}
		private void OnWindowClosing(object sender, CancelEventArgs e) {
			SaveSettings();
			this.Closing -= OnWindowClosing;
			e.Cancel = true;
			var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.5));
			anim.Completed += (s, _) => this.Close();
			this.BeginAnimation(UIElement.OpacityProperty, anim);
		}

		#endregion

		private void OnGamesTab(object sender, MouseButtonEventArgs e) {
			if (currentTab != SetupTypes.Game) {
				if (!Config.DisableTransitions) {
					double width = CurrentSetupGrid.ActualWidth;
					bool middlePass = false;// currentTab == SetupTypes.Tool;
					var last = CurrentSetupStack.Peek();
					var middle = serverStack.Peek();
					var next = gameStack.Peek();
					gridGames.Visibility = Visibility.Visible;
					last.LeaveTab(true, width, middlePass, UpdateTab);
					next.EnterTab(true, width, middlePass);
					if (middlePass) {
						gridServers.Visibility = Visibility.Visible;
						middle.PassTab(true, width);
					}
				}
				currentTab = SetupTypes.Game;
				UpdateFolder();
				if (Config.DisableTransitions)
					UpdateTab();
			}
		}

		private void OnServersTab(object sender, MouseButtonEventArgs e) {
			if (currentTab != SetupTypes.Server) {
				if (!Config.DisableTransitions) {
					double width = CurrentSetupGrid.ActualWidth;
					bool back = currentTab == SetupTypes.Tool;
					var last = CurrentSetupStack.Peek();
					var next = serverStack.Peek();
					gridServers.Visibility = Visibility.Visible;
					last.LeaveTab(back, width, false, UpdateTab);
					next.EnterTab(back, width, false);
				}
				currentTab = SetupTypes.Server;
				UpdateFolder();
				if (Config.DisableTransitions)
					UpdateTab();
			}
		}

		private void OnToolsTab(object sender, MouseButtonEventArgs e) {
			if (currentTab != SetupTypes.Tool) {
				if (!Config.DisableTransitions) {
					double width = CurrentSetupGrid.ActualWidth;
					bool middlePass = false;// currentTab == SetupTypes.Game;
					var last = CurrentSetupStack.Peek();
					var middle = serverStack.Peek();
					var next = toolStack.Peek();
					gridTools.Visibility = Visibility.Visible;
					last.LeaveTab(false, width, middlePass, UpdateTab);
					next.EnterTab(false, width, middlePass);
					if (middlePass) {
						gridServers.Visibility = Visibility.Visible;
						middle.PassTab(false, width);
					}
				}
				currentTab = SetupTypes.Tool;
				UpdateFolder();
				if (Config.DisableTransitions)
					UpdateTab();
			}
		}

		private void UpdateTab() {
			gridGames.Visibility = (currentTab == SetupTypes.Game ? Visibility.Visible : Visibility.Hidden);
			gridServers.Visibility = (currentTab == SetupTypes.Server ? Visibility.Visible : Visibility.Hidden);
			gridTools.Visibility = (currentTab == SetupTypes.Tool ? Visibility.Visible : Visibility.Hidden);
		}
		private void UpdateFolder() {
			string tabLabel = currentTab.ToString() + " List";
			if (CurrentSetupStack.Count > 1)
				tabLabel += " " + new string('>', CurrentSetupStack.Count - 1) + " " + CurrentSetupStack.Peek().Folder.Name;
			labelListType.Content = tabLabel;
		}

		private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Back || e.Key == Key.BrowserBack) {
				if (CurrentSetupStack.Count > 1)
					NavigateBack(CurrentSetupGrid, CurrentSetupStack);
			}
		}

		private void OnEditSetups(object sender, MouseButtonEventArgs e) {
			if (SettingsWindow.ShowDialog(this, currentTab)) {
				ReloadSetups();
			}
		}
	}
}
