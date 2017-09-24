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
using TerraLauncher.Util;
using TerraLauncher.Windows;

namespace TerraLauncher.Controls {
	/// <summary>
	/// Interaction logic for SetupTreeView.xaml
	/// </summary>
	public partial class SetupTreeView : UserControl {

		static BitmapImage iconGame;
		static BitmapImage iconGameTMod;
		static BitmapImage iconServer;
		static BitmapImage iconTool;
		static BitmapImage iconFolderOpen;
		static BitmapImage iconFolderClosed;

		static BitmapImage iconAddGame;
		static BitmapImage iconAddServer;
		static BitmapImage iconAddTool;
		static BitmapImage iconAddFolder;
		static BitmapImage iconRemoveGame;
		static BitmapImage iconRemoveServer;
		static BitmapImage iconRemoveTool;
		static BitmapImage iconRemoveFolder;
		static BitmapImage iconRemove;

		SetupTypes setupType;

		Point lastMouseDown;
		TreeViewItem draggedItem;
		TreeViewItem dropTarget;
		
		public bool Modified { get; private set; } = false;

		public SetupTreeView() {
			InitializeComponent();
			
			if (iconGame == null && !DesignerProperties.GetIsInDesignMode(this)) {
				string uri = "pack://application:,,,/Resources/Icons/";
				iconAddGame = new BitmapImage(new Uri(uri + "GameAdd.png"));
				iconAddServer = new BitmapImage(new Uri(uri + "ServerAdd.png"));
				iconAddTool = new BitmapImage(new Uri(uri + "ToolAdd.png"));
				iconAddFolder = new BitmapImage(new Uri(uri + "FolderAdd.png"));
				iconRemoveGame = new BitmapImage(new Uri(uri + "GameRemove.png"));
				iconRemoveServer = new BitmapImage(new Uri(uri + "ServerRemove.png"));
				iconRemoveTool = new BitmapImage(new Uri(uri + "ToolRemove.png"));
				iconRemoveFolder = new BitmapImage(new Uri(uri + "FolderRemove.png"));
				iconRemove = new BitmapImage(new Uri(uri + "Remove.png"));

				iconGame = new BitmapImage(new Uri(uri + "TreeView/TreeViewGame.png"));
				iconGameTMod = new BitmapImage(new Uri(uri + "TreeView/TreeViewGameTMod.png"));
				iconServer = new BitmapImage(new Uri(uri + "TreeView/TreeViewServer.png"));
				iconTool = new BitmapImage(new Uri(uri + "TreeView/TreeViewTool.png"));
				iconFolderOpen = new BitmapImage(new Uri(uri + "TreeView/TreeViewFolderOpen.png"));
				iconFolderClosed = new BitmapImage(new Uri(uri + "TreeView/TreeViewFolderClosed.png"));

			}
		}

		public SetupFolder GenerateHierarchy() {
			TreeViewItem rootItem = (TreeViewItem)treeView.Items[0];
			SetupFolder rootFolder = (SetupFolder)rootItem.Tag;
			PopulateHierarchy(rootItem, rootFolder);
			return rootFolder;
		}

		private void PopulateHierarchy(TreeViewItem parent, SetupFolder folder) {
			folder.Entries.Clear();
			foreach (var itemObj in parent.Items) {
				TreeViewItem item = (TreeViewItem)itemObj;
				ISetup setup = item.Tag as ISetup;
				folder.Entries.Add(setup);
				if (setup is SetupFolder) {
					((SetupFolder)setup).Parent = folder;
					PopulateHierarchy(item, (SetupFolder)setup);
				}
			}
		}

		public void Populate(SetupFolder folder, SetupTypes setupType) {
			folder = folder.CloneFolder();
			this.setupType = setupType;
			TreeViewItem root = MakeFolderItem(folder, true);
			treeView.Items.Add(root);
			Populate(root, folder);
			switch (setupType) {
			case SetupTypes.Game: imageAddSetup.Source = iconAddGame; break;
			case SetupTypes.Server: imageAddSetup.Source = iconAddServer; break;
			case SetupTypes.Tool: imageAddSetup.Source = iconAddTool; break;
			}
			buttonAddSetup.ToolTip = "Add " + setupType + " Setup";
			UpdateButtons();
		}
		private void Populate(TreeViewItem parent, SetupFolder folder) {
			foreach (object o in folder.Entries) {
				if (o is SetupFolder) {
					TreeViewItem item = MakeFolderItem(o as SetupFolder);
					parent.Items.Add(item);
					Populate(item, o as SetupFolder);
				}
				else if (o is Setup) {
					parent.Items.Add(MakeSetupItem(o as Setup));
				}
			}
		}


		private TreeViewItem MakeFolderItem(SetupFolder folder, bool root = false) {
			TreeViewItem item = new TreeViewItem();
			item.Tag = folder;
			item.AllowDrop = true;
			item.IsExpanded = true;
			item.Expanded += (s, _) => {
				UpdateItem((TreeViewItem)s);
			};
			item.Collapsed += (s, _) => {
				UpdateItem((TreeViewItem)s);
			};

			StackPanel stackPanel = new StackPanel();
			stackPanel.Orientation = Orientation.Horizontal;
			stackPanel.IsHitTestVisible = false;
			stackPanel.Margin = new Thickness(2);
			item.Header = stackPanel;

			Image image = new Image();
			image.VerticalAlignment = VerticalAlignment.Center;
			image.Width = 16;
			image.Height = 16;
			image.Source = iconFolderOpen;
			stackPanel.Children.Add(image);

			TextBlock text = new TextBlock();
			text.VerticalAlignment = VerticalAlignment.Center;
			text.Text = folder.Name;
			text.Margin = new Thickness(5, 1, 2, 1);
			stackPanel.Children.Add(text);

			return item;
		}
		private TreeViewItem MakeSetupItem(Setup setup) {
			TreeViewItem item = new TreeViewItem();
			item.Tag = setup;

			StackPanel stackPanel = new StackPanel();
			stackPanel.IsHitTestVisible = false;
			stackPanel.Orientation = Orientation.Horizontal;
			stackPanel.Margin = new Thickness(2);
			item.Header = stackPanel;

			Image image = new Image();
			image.VerticalAlignment = VerticalAlignment.Center;
			image.Width = 16;
			image.Height = 16;
			switch (setupType) {
			case SetupTypes.Game:
				image.Source = (((Game)setup).IsTMod ? iconGameTMod : iconGame);
				break;
			case SetupTypes.Server:
				image.Source = iconServer;
				break;
			case SetupTypes.Tool:
				image.Source = iconTool;
				break;
			}
			stackPanel.Children.Add(image);

			TextBlock name = new TextBlock();
			name.VerticalAlignment = VerticalAlignment.Center;
			name.Text = setup.Name;
			name.Margin = new Thickness(5, 1, 2, 1);
			stackPanel.Children.Add(name);

			TextBlock details = new TextBlock();
			details.VerticalAlignment = VerticalAlignment.Center;
			details.FontSize = 10;
			if (!string.IsNullOrWhiteSpace(setup.Details))
				details.Text = " (" + setup.Details + ")";
			details.Margin = new Thickness(2, 1, 2, 1);
			stackPanel.Children.Add(details);

			item.PreviewMouseDoubleClick += OnSetupPreviewMouseDoubleClick;

			return item;
		}

		private void OnSetupPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			OnEdit(null, null);
		}

		private void UpdateItem(TreeViewItem item) {
			object o = item.Tag;

			StackPanel stackPanel = (StackPanel)item.Header;
			Image image = (Image)stackPanel.Children[0];
			TextBlock name = (TextBlock)stackPanel.Children[1];
			
			if (o is SetupFolder) {
				SetupFolder folder = (SetupFolder)o;
				name.Text = folder.Name;
				image.Source = (item.IsExpanded ? iconFolderOpen : iconFolderClosed);
			}
			else if (o is Setup) {
				TextBlock details = (TextBlock)stackPanel.Children[2];

				Setup setup = (Setup)o;

				name.Text = setup.Name;

				if (!string.IsNullOrWhiteSpace(setup.Details))
					details.Text = " (" + setup.Details + ")";
				else
					details.Text = "";

				if (setupType == SetupTypes.Game) {
					image.Source = (((Game)setup).IsTMod ? iconGameTMod : iconGame);
				}
			}
		}

		private void OnAddFolder(object sender, RoutedEventArgs e) {
			TreeViewItem parent = treeView.SelectedItem as TreeViewItem;
			int index = 0;
			if (parent == null) {
				parent = treeView.Items[0] as TreeViewItem;
			}

			object o = parent.Tag;
			if (o is Setup) {
				TreeViewItem child = parent;
				parent = parent.Parent as TreeViewItem;
				index = parent.Items.IndexOf(child) + 1;
			}

			SetupFolder folder = new SetupFolder();
			TreeViewItem item = MakeFolderItem(folder);
			parent.Items.Insert(index, item);
			item.IsSelected = true;
			item.Focus();
			UpdateButtons();
			Modified = true;
		}
		private void OnAddSetup(object sender, RoutedEventArgs e) {
			TreeViewItem parent = treeView.SelectedItem as TreeViewItem;
			int index = 0;
			if (parent == null) {
				parent = treeView.Items[0] as TreeViewItem;
			}

			object o = parent.Tag;
			if (o is Setup) {
				TreeViewItem child = parent;
				parent = parent.Parent as TreeViewItem;
				index = parent.Items.IndexOf(child) + 1;
			}

			Setup setup = null;
			switch (setupType) {
			case SetupTypes.Game: setup = new Game(); break;
			case SetupTypes.Server: setup = new Server(); break;
			case SetupTypes.Tool: setup = new Tool(); break;
			}
			TreeViewItem item = MakeSetupItem(setup);
			parent.Items.Insert(index, item);
			item.IsSelected = true;
			item.Focus();
			UpdateButtons();
			Modified = true;
		}

		private void OnRemove(object sender, RoutedEventArgs e) {
			TreeViewItem item = treeView.SelectedItem as TreeViewItem;

			TreeViewItem parent = item.Parent as TreeViewItem;

			if (item != null && parent != null) {
				string type = "Folder";
				if (item.Tag is Setup)
					type = setupType.ToString();
				var result = TriggerMessageBox.Show(Window.GetWindow(this), MessageIcon.Question, "Are you sure you want to remove this " + type.ToLower() + "?", "Remove " + type, MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes) {
					TreeViewItem newSelection = FindMoveItem(item, -1);
					if (newSelection == null)
						newSelection = FindMoveItem(item, 1);
					parent.Items.Remove(item);
					UpdateButtons();

					if (newSelection != null) {
						newSelection.IsSelected = true;
						newSelection.Focus();
					}
					Modified = true;
				}
			}
		}

		private void OnEdit(object sender, RoutedEventArgs e) {
			TreeViewItem item = treeView.SelectedItem as TreeViewItem;
			if (item != null && item != treeView.Items[0]) {
				if (item.Tag is SetupFolder) {
					if (EditFolderWindow.ShowDialog(Window.GetWindow(this), item.Tag as SetupFolder)) {
						UpdateItem(item);
						Modified = true;
					}
				}
				else if (item.Tag is Game) {
					if (EditGameWindow.ShowDialog(Window.GetWindow(this), item.Tag as Game)) {
						UpdateItem(item);
						Modified = true;
					}
				}
				else if (item.Tag is Server) {
					if (EditServerWindow.ShowDialog(Window.GetWindow(this), item.Tag as Server)) {
						UpdateItem(item);
						Modified = true;
					}
				}
				else if (item.Tag is Tool) {
					if (EditToolWindow.ShowDialog(Window.GetWindow(this), item.Tag as Tool)) {
						UpdateItem(item);
						Modified = true;
					}
				}
				item.IsSelected = true;
				item.Focus();
			}
		}

		private void OnMoveDown(object sender, RoutedEventArgs e) {
			TreeViewItem item = treeView.SelectedItem as TreeViewItem;
			TreeViewItem parent = item.Parent as TreeViewItem;

			if (item != null && parent != null) {
				int index;
				TreeViewItem newParent = FindMoveLocation(item, 1, out index);
				parent.Items.Remove(item);
				newParent.Items.Insert(index, item);
				item.IsSelected = true;
				item.Focus();
				UpdateButtons();
				Modified = true;
			}
		}

		private void OnMoveUp(object sender, RoutedEventArgs e) {
			TreeViewItem item = treeView.SelectedItem as TreeViewItem;
			TreeViewItem parent = item.Parent as TreeViewItem;

			if (item != null && parent != null) {
				int index;
				TreeViewItem newParent = FindMoveLocation(item, -1, out index);
				parent.Items.Remove(item);
				newParent.Items.Insert(index, item);
				item.IsSelected = true;
				item.Focus();
				UpdateButtons();
				Modified = true;
			}
		}

		private void OnTreeViewSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			UpdateButtons();
		}

		private void UpdateButtons() {
			TreeViewItem item = treeView.SelectedItem as TreeViewItem;
			TreeViewItem parent = null;
			if (item != null)
				parent = item.Parent as TreeViewItem;

			if (item != null && parent != null) {
				int index;
				buttonMoveUp.IsEnabled = (FindMoveLocation(item, -1, out index) != null);
				buttonMoveDown.IsEnabled = (FindMoveLocation(item, 1, out index) != null);
				buttonRemove.IsEnabled = true;
				buttonEdit.IsEnabled = (item.Parent != null);
				if (item.Tag is SetupFolder) {
					buttonRemove.ToolTip = "Remove Folder";
					imageRemove.Source = iconRemoveFolder;
				}
				else {
					buttonRemove.ToolTip = "Remove " + setupType + " Setup";
					switch (setupType) {
					case SetupTypes.Game: imageRemove.Source = iconRemoveGame; break;
					case SetupTypes.Server: imageRemove.Source = iconRemoveServer; break;
					case SetupTypes.Tool: imageRemove.Source = iconRemoveTool; break;
					}
				}
			}
			else {
				buttonMoveUp.IsEnabled = false;
				buttonMoveDown.IsEnabled = false;
				buttonRemove.IsEnabled = false;
				buttonEdit.IsEnabled = false;
				imageRemove.Source = iconRemove;
			}
		}
		private TreeViewItem FindMoveItem(TreeViewItem item, int distance) {
			int index;
			TreeViewItem parent = FindMoveLocation(item, distance, out index);
			if (index == -1)
				return null;
			return parent.Items[index] as TreeViewItem;
		}
		private TreeViewItem FindMoveLocation(TreeViewItem item, int distance, out int index) {
			TreeViewItem parent = item.Parent as TreeViewItem;

			if (parent == null) {
				index = -1;
				return null;
			}
			int oldIndex = parent.Items.IndexOf(item);
			index = -1;
			while (distance != 0) {
				if ((distance < 0 && oldIndex == 0) ||
					(distance > 0 && oldIndex == parent.Items.Count - 1)) {
					TreeViewItem parentsParent = parent.Parent as TreeViewItem;
					if (parentsParent == null) {
						index = -1;
						return null;
					}
					else {
						oldIndex = parentsParent.Items.IndexOf(parent) + (distance > 0 ? 1 : 0);
						index = oldIndex;
						distance -= Math.Sign(distance);
						parent = parentsParent;
					}
				}
				else {
					oldIndex += Math.Sign(distance);
					TreeViewItem newItem = parent.Items[oldIndex] as TreeViewItem;
					if (newItem != null) {
						if (newItem.Tag is SetupFolder && newItem.IsExpanded) {
							if (distance < 0)
								oldIndex = newItem.Items.Count;
							else
								oldIndex = 0;
							parent = newItem;
						}
					}
					index = oldIndex;
					distance -= Math.Sign(distance);
				}
			}
			if (index == -1)
				return null;
			return parent;
		}


		private void OnTreeViewMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton == MouseButton.Left) {
				lastMouseDown = e.GetPosition(treeView);
			}
		}

		private void OnTreeViewMouseMove(object sender, MouseEventArgs e) {
			try {
				if (e.LeftButton == MouseButtonState.Pressed) {
					Point currentPosition = e.GetPosition(treeView);
					
					if ((Math.Abs(currentPosition.X - lastMouseDown.X) > 10.0) ||
						(Math.Abs(currentPosition.Y - lastMouseDown.Y) > 10.0)) {
						draggedItem = treeView.SelectedItem as TreeViewItem;
						if (draggedItem != null) {
							DragDropEffects finalDropEffect = DragDrop.DoDragDrop(treeView, treeView.SelectedValue,
								DragDropEffects.Move);
							//Checking target is not null and item is dragging(moving)
							if ((finalDropEffect == DragDropEffects.Move) && (dropTarget != null)) {
								// A Move drop was accepted
								if (IsValidDropTarget(draggedItem, dropTarget)) {
									int index = 0;
									if (dropTarget.Tag is Setup || !dropTarget.IsExpanded) {
										TreeViewItem child = dropTarget;
										TreeViewItem parent = child.Parent as TreeViewItem;
										if (parent != null) {
											dropTarget = parent;
											index = dropTarget.Items.IndexOf(child) + 1;
										}
									}
									if (dropTarget.Items.Contains(draggedItem)) {
										int oldIndex = dropTarget.Items.IndexOf(draggedItem);
										if (oldIndex < index)
											index--;
										dropTarget.Items.Remove(draggedItem);
										dropTarget.Items.Insert(index, draggedItem);
										Modified = true;
									}
									else {
										TreeViewItem parent = draggedItem.Parent as TreeViewItem;
										if (parent != null) {
											parent.Items.Remove(draggedItem);
											dropTarget.Items.Insert(index, draggedItem);
											Modified = true;
										}
									}
									//newSelection = draggedItem;
									draggedItem.IsSelected = true;
									dropTarget = null;
									draggedItem = null;
								}
							}
						}
					}
				}
			}
			catch { }
		}
		private void OnTreeViewDragOver(object sender, DragEventArgs e) {
			try {

				Point currentPosition = e.GetPosition(treeView);


				if ((Math.Abs(currentPosition.X - lastMouseDown.X) > 10.0) ||
					(Math.Abs(currentPosition.Y - lastMouseDown.Y) > 10.0)) {
					// Verify that this is a valid drop and then store the drop target
					TreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);
					if (IsValidDropTarget(draggedItem, item)) {
						e.Effects = DragDropEffects.Move;
					}
					else {
						e.Effects = DragDropEffects.None;
					}
				}
				e.Handled = true;
			}
			catch { }
		}


		private void OnTreeViewDrop(object sender, DragEventArgs e) {
			try {
				e.Effects = DragDropEffects.None;
				e.Handled = true;

				// Verify that this is a valid drop and then store the drop target
				TreeViewItem target = GetNearestContainer(e.OriginalSource as UIElement);
				//TreeViewItem target = ((TreeViewItem)e.OriginalSource).Parent as TreeViewItem;
				if (target != null && draggedItem != null) {
					dropTarget = target;
					e.Effects = DragDropEffects.Move;
				}
			}
			catch { }
		}
		private bool IsValidDropTarget(TreeViewItem source, TreeViewItem target) {
			return source.Tag != target.Tag && !(source.Tag is SetupFolder && InsideItself(source, target));
		}
		private bool InsideItself(TreeViewItem item, TreeViewItem target) {
			TreeViewItem parent = target as TreeViewItem;
			while (parent != null) {
				if (parent == item)
					return true;
				parent = parent.Parent as TreeViewItem;
			}
			return false;
		}

		private TreeViewItem GetNearestContainer(UIElement element) {
			// Walk up the element tree to the nearest tree view item.
			TreeViewItem container = element as TreeViewItem;
			while ((container == null) && (element != null)) {
				element = VisualTreeHelper.GetParent(element) as UIElement;
				container = element as TreeViewItem;
			}
			return container;
		}
	}
}
