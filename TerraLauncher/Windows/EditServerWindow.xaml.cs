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
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using System.IO;
using Path = System.IO.Path;

namespace TerraLauncher.Windows {
	/// <summary>
	/// Interaction logic for EditGameWindow.xaml
	/// </summary>
	public partial class EditServerWindow : Window {

		string nonDefaultWorldFolder = "";

		public EditServerWindow(Server server) {
			InitializeComponent();

			// Init icon combo box

			int index = 0;
			foreach (var pair in Setup.SetupIcons) {
				comboBoxIcon.Items.Add(pair.Key);
				if (server.Icon == pair.Key)
					comboBoxIcon.SelectedIndex = index;
				index++;
			}
			if (comboBoxIcon.SelectedIndex == -1)
				comboBoxIcon.SelectedIndex = 0;

			textBoxName.Text = server.Name;
			textBoxDetails.Text = server.Details;
			if (Setup.SetupIcons.ContainsKey(server.Icon)) {
				comboBoxIcon.Visibility = Visibility.Visible;
				textBoxCustomIcon.Visibility = Visibility.Hidden;
				buttonBrowseCustomIcon.Visibility = Visibility.Hidden;

				checkBoxCustomIcon.IsChecked = false;
			}
			else {
				textBoxCustomIcon.Text = server.Icon;
				comboBoxIcon.Visibility = Visibility.Hidden;
				textBoxCustomIcon.Visibility = Visibility.Visible;
				buttonBrowseCustomIcon.Visibility = Visibility.Visible;

				checkBoxCustomIcon.IsChecked = true;
			}

			textBoxExe.Text = server.ExePath;
			textBoxArguments.Text = server.Arguments;
			textBoxWorldFolder.Text = server.WorldDirectory;
			checkBoxDefaultWorldFolder.IsChecked = (server.WorldDirectory == "Default");
			textBoxWorldFolder.IsEnabled = (server.WorldDirectory != "Default");
			UpdateIcon(server.Icon);

			checkBoxTMod.IsChecked = server.IsTMod;

			// Remove quotes from "Copy Path" command on paste
			DataObject.AddPastingHandler(textBoxCustomIcon, OnTextBoxQuotesPaste);
			DataObject.AddPastingHandler(textBoxExe, OnTextBoxQuotesPaste);
			DataObject.AddPastingHandler(textBoxWorldFolder, OnTextBoxQuotesPaste);

			// Disable drag/drop text in textboxes so you can scroll their contents easily
			DataObject.AddCopyingHandler(textBoxName, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxDetails, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxCustomIcon, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxExe, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxWorldFolder, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxArguments, OnTextBoxCancelDrag);
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

		private void OnBrowseExe(object sender, RoutedEventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Choose a Terraria Executable";
			dialog.FileName = textBoxExe.Text;
			dialog.Filter = "Executable Files|*.exe|All Files|*.*";
			dialog.FilterIndex = 0;
			dialog.CheckFileExists = true;
			try {
				dialog.InitialDirectory = Path.GetDirectoryName(textBoxExe.Text);
			}
			catch { }
			var result = dialog.ShowDialog(this);
			if (result.HasValue && result.Value) {
				textBoxExe.Text = dialog.FileName;
			}
		}

		private void OnBrowseSaveFolder(object sender, RoutedEventArgs e) {
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.ShowNewFolderButton = true;
			dialog.Description = "Choose a Save Folder";
			dialog.SelectedPath = textBoxWorldFolder.Text;
			var result = dialog.ShowFolderBrowser(this);
			if (result.HasValue && result.Value) {
				textBoxWorldFolder.Text = dialog.SelectedPath;
			}
			dialog.Dispose();
			dialog = null;
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

		private void OnDefaultWorldFolderChecked(object sender, RoutedEventArgs e) {
			if (checkBoxDefaultWorldFolder.IsChecked.Value) {
				textBoxWorldFolder.IsEnabled = false;
				nonDefaultWorldFolder = textBoxWorldFolder.Text;
				textBoxWorldFolder.Text = "Default";
			}
			else {
				textBoxWorldFolder.IsEnabled = true;
				textBoxWorldFolder.Text = nonDefaultWorldFolder;
			}
		}


		public static bool ShowDialog(Window owner, Server server) {
			EditServerWindow window = new EditServerWindow(server);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				server.Name = window.textBoxName.Text;
				server.Details = window.textBoxDetails.Text;
				server.ExePath = window.textBoxExe.Text;
				server.Arguments = window.textBoxArguments.Text;
				if (window.checkBoxDefaultWorldFolder.IsChecked.Value)
					server.WorldDirectory = "Default";
				else
					server.WorldDirectory = window.textBoxWorldFolder.Text;
				if (window.checkBoxCustomIcon.IsChecked.Value)
					server.Icon = window.textBoxCustomIcon.Text;
				else
					server.Icon = window.comboBoxIcon.SelectedItem as string;
				server.IsTMod = window.checkBoxTMod.IsChecked.Value;
				return true;
			}
			return false;
		}

		private void UpdateIcon(string icon) {
			BitmapSource bitmap = Setup.LoadIcon(icon, "Server");
			imageIcon.Source = bitmap;
			imageIcon.Width = Math.Min(68, bitmap.PixelWidth);
			imageIcon.Height = Math.Min(68, bitmap.PixelHeight);
		}
	}
}
