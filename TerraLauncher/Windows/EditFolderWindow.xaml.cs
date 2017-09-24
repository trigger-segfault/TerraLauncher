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
using System.Windows.Shapes;
using Microsoft.Win32;
using TerraLauncher.Setups;
using Path = System.IO.Path;

namespace TerraLauncher.Windows {
	/// <summary>
	/// Interaction logic for EditFolderWindow.xaml
	/// </summary>
	public partial class EditFolderWindow : Window {
		public EditFolderWindow(SetupFolder folder) {
			InitializeComponent();

			// Init icon combo box

			int index = 0;
			int folderIndex = -1;
			foreach (var pair in Setup.SetupIcons) {
				comboBoxIcon.Items.Add(pair.Key);
				if (folder.Icon == pair.Key)
					comboBoxIcon.SelectedIndex = index;
				if (pair.Key == "Folder")
					folderIndex = index;
				index++;
			}
			if (comboBoxIcon.SelectedIndex == -1 && folderIndex != -1)
				comboBoxIcon.SelectedIndex = folderIndex;

			textBoxName.Text = folder.Name;
			if (Setup.SetupIcons.ContainsKey(folder.Icon)) {
				comboBoxIcon.Visibility = Visibility.Visible;
				textBoxCustomIcon.Visibility = Visibility.Hidden;
				buttonBrowseCustomIcon.Visibility = Visibility.Hidden;

				checkBoxCustomIcon.IsChecked = false;
			}
			else {
				textBoxCustomIcon.Text = folder.Icon;
				comboBoxIcon.Visibility = Visibility.Hidden;
				textBoxCustomIcon.Visibility = Visibility.Visible;
				buttonBrowseCustomIcon.Visibility = Visibility.Visible;

				checkBoxCustomIcon.IsChecked = true;
			}

			UpdateIcon(folder.Icon);

			// Remove quotes from "Copy Path" command on paste
			DataObject.AddPastingHandler(textBoxCustomIcon, OnTextBoxQuotesPaste);

			// Disable drag/drop text in textboxes so you can scroll their contents easily
			DataObject.AddCopyingHandler(textBoxName, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxCustomIcon, OnTextBoxCancelDrag);
		}

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			// Make text boxes lose focus on click away
			FocusManager.SetFocusedElement(this, this);
		}
		private void OnCustomIconLostFocus(object sender, RoutedEventArgs e) {
			if (checkBoxCustomIcon.IsChecked.Value)
				UpdateIcon(textBoxCustomIcon.Text);
		}

		private void OnTextBoxCancelDrag(object sender, DataObjectCopyingEventArgs e) {
			if (e.IsDragDrop)
				e.CancelCommand();
		}

		private void OnTextBoxQuotesPaste(object sender, DataObjectPastingEventArgs e) {
			var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
			if (!isText) return;

			var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
			if (text.StartsWith("\"") || text.EndsWith("\"")) {
				text = text.Trim('"');
				Clipboard.SetText(text);
			}
		}

		private void OnOKClicked(object sender, RoutedEventArgs e) {
			DialogResult = true;
		}

		private void OnIconSelectionChanged(object sender, SelectionChangedEventArgs e) {
			UpdateIcon(comboBoxIcon.SelectedItem as string);
		}

		private void OnBrowseCustomIcon(object sender, RoutedEventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Choose an Icon";
			dialog.FileName = textBoxCustomIcon.Text;
			dialog.Filter = "Image Files|*.png;*.bmp;*.jpg|Files with Icons|*.ico;*.exe|All Files|*.*";
			dialog.FilterIndex = 0;
			dialog.CheckFileExists = true;
			try {
				dialog.InitialDirectory = Path.GetDirectoryName(textBoxCustomIcon.Text);
			}
			catch { }
			var result = dialog.ShowDialog(this);
			if (result.HasValue && result.Value) {
				textBoxCustomIcon.Text = dialog.FileName;
				UpdateIcon(textBoxCustomIcon.Text);
			}
		}

		private void OnCustomIconChecked(object sender, RoutedEventArgs e) {
			if (!checkBoxCustomIcon.IsChecked.Value) {
				comboBoxIcon.Visibility = Visibility.Visible;
				textBoxCustomIcon.Visibility = Visibility.Hidden;
				buttonBrowseCustomIcon.Visibility = Visibility.Hidden;
				UpdateIcon(comboBoxIcon.SelectedItem as string);
			}
			else {
				comboBoxIcon.Visibility = Visibility.Hidden;
				textBoxCustomIcon.Visibility = Visibility.Visible;
				buttonBrowseCustomIcon.Visibility = Visibility.Visible;
				UpdateIcon(textBoxCustomIcon.Text);
			}
		}


		public static bool ShowDialog(Window owner, SetupFolder folder) {
			EditFolderWindow window = new EditFolderWindow(folder);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				folder.Name = window.textBoxName.Text;
				if (window.checkBoxCustomIcon.IsChecked.Value)
					folder.Icon = window.textBoxCustomIcon.Text;
				else
					folder.Icon = window.comboBoxIcon.SelectedItem as string;
				return true;
			}
			return false;
		}

		private void UpdateIcon(string icon) {
			BitmapSource bitmap = Setup.LoadFolderIcon(icon);
			imageIcon.Source = bitmap;
			imageIcon.Width = Math.Min(68, bitmap.PixelWidth);
			imageIcon.Height = Math.Min(68, bitmap.PixelHeight);
		}
	}
}
