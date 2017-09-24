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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TerraLauncher.Setups;

namespace TerraLauncher.Controls.Terraria {
	/// <summary>
	/// Interaction logic for TerrariaSetupList.xaml
	/// </summary>
	public partial class TerrariaSetupList : UserControl {

		private const double FolderTime = 0.4;
		private const double TabTime = 0.3;

		Action<SetupFolder> navigateForward;
		Action navigateBack;
		SetupFolder folder;

		public SetupFolder Folder {
			get { return folder; }
		}

		public TerrariaSetupList() {
			InitializeComponent();

			scrollViewer.ScrollSpeed = Config.ScrollSpeed;
		}

		public void PopulateList(SetupFolder folder, Action<SetupFolder> navigateForward, Action navigateBack = null) {
			this.navigateForward = navigateForward;
			this.navigateBack = navigateBack;
			this.folder = folder;
			list.Children.Clear();
			if (folder.Parent != null) {
				TerrariaSetupFolder control = new TerrariaSetupFolder(folder.Parent, true, () => {
					navigateBack();
				});
				list.Children.Add(control);
			}
			foreach (object bothEntry in folder.Entries) {
				if (bothEntry is SetupFolder) {
					SetupFolder subFolder = (SetupFolder)bothEntry;
					TerrariaSetupFolder control = new TerrariaSetupFolder(subFolder, false, () => {
						navigateForward(subFolder);
					});
					list.Children.Add(control);
				}
				else if (bothEntry is Setup) {
					Setup entry = (Setup)bothEntry;
					TerrariaSetupEntry control = new TerrariaSetupEntry(entry);
					list.Children.Add(control);
				}
			}
		}

		public void EnterFolder(bool back, double width) {
			var anim = CreateAnimation(true, back, false, width);
			anim.Completed += (s, _) => {
				Dispatcher.Invoke(() => { IsEnabled = true; });
			};
			IsEnabled = false;
			Visibility = Visibility.Visible;
			list.BeginAnimation(TerrariaSetupList.MarginProperty, anim);
		}
		public void LeaveFolder(bool back, double width) {
			var anim = CreateAnimation(false, back, false, width);
			anim.Completed += (s, _) => {
				Dispatcher.Invoke(() => {
					IsEnabled = true;
					Visibility = Visibility.Hidden;
					if (back) {
						DependencyObject parent = VisualTreeHelper.GetParent(this);
						((Grid)parent).Children.Remove(this);
					}
				});
			};
			IsEnabled = false;

			list.BeginAnimation(TerrariaSetupList.MarginProperty, anim);
		}

		public void EnterTab(bool back, double width, bool middlePass) {
			var anim = CreateAnimation(true, back, true, width, middlePass);
			anim.Completed += (s, _) => {
				Dispatcher.Invoke(() => { IsEnabled = true; });
			};
			IsEnabled = false;
			list.BeginAnimation(TerrariaSetupList.MarginProperty, anim);
		}
		public void LeaveTab(bool back, double width, bool middlePass, Action updateTabs) {
			var anim = CreateAnimation(false, back, true, width, middlePass);
			anim.Completed += (s, _) => {
				Dispatcher.Invoke(() => {
					IsEnabled = true;
					updateTabs?.Invoke();
				});
			};
			IsEnabled = false;

			list.BeginAnimation(TerrariaSetupList.MarginProperty, anim);
		}
		public void PassTab(bool back, double width) {
			double d = width + 24;
			if (back)
				d = -d;
			var anim = new ThicknessAnimation(
				new Thickness(d, 0, -d, 0),
				new Thickness(-d, 0, d, 0),
				TimeSpan.FromSeconds(TabTime * 1.5)
			);
			anim.EasingFunction = CreateEasing();
			anim.Completed += (s, _) => {
				Dispatcher.Invoke(() => {
					IsEnabled = true;
				});
			};
			IsEnabled = false;

			list.BeginAnimation(TerrariaSetupList.MarginProperty, anim);
		}

		public ThicknessAnimation CreateAnimation(bool enter, bool back, bool isTab, double width, bool middlePass = false) {
			double d = width + 24;
			if (middlePass)
				d *= 2;
			if (back == enter)
				d = -d;
			var anim = new ThicknessAnimation(
				new Thickness(enter ? d : 0, 0, enter ? -d : 0, 0),
				new Thickness(enter ? 0 : d, 0, enter ? 0 : -d, 0),
				TimeSpan.FromSeconds((isTab ? TabTime : FolderTime) * (middlePass ? 1.5 : 1))
			);
			anim.EasingFunction = CreateEasing();
			return anim;
		}

		public IEasingFunction CreateEasing() {
			var ease = new ElasticEase();
			ease.Oscillations = 0;
			ease.EasingMode = EasingMode.EaseInOut;
			return ease;
		}
	}
}
