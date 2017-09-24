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
	public partial class EditGameWindow : Window {

		string nonDefaultSaveFolder = "";

		public EditGameWindow(Game game) {
			InitializeComponent();

			// Init icon combo box

			int index = 0;
			foreach (var pair in Setup.SetupIcons) {
				comboBoxIcon.Items.Add(pair.Key);
				if (game.Icon == pair.Key)
					comboBoxIcon.SelectedIndex = index;
				index++;
			}
			if (comboBoxIcon.SelectedIndex == -1)
				comboBoxIcon.SelectedIndex = 0;

			textBoxName.Text = game.Name;
			textBoxDetails.Text = game.Details;
			if (Setup.SetupIcons.ContainsKey(game.Icon)) {
				comboBoxIcon.Visibility = Visibility.Visible;
				textBoxCustomIcon.Visibility = Visibility.Hidden;
				buttonBrowseCustomIcon.Visibility = Visibility.Hidden;

				checkBoxCustomIcon.IsChecked = false;
			}
			else {
				textBoxCustomIcon.Text = game.Icon;
				comboBoxIcon.Visibility = Visibility.Hidden;
				textBoxCustomIcon.Visibility = Visibility.Visible;
				buttonBrowseCustomIcon.Visibility = Visibility.Visible;

				checkBoxCustomIcon.IsChecked = true;
			}

			textBoxExe.Text = game.ExePath;
			textBoxSaveFolder.Text = game.SaveDirectory;
			checkBoxDefaultSaveFolder.IsChecked = (game.SaveDirectory == "Default");
			textBoxSaveFolder.IsEnabled = (game.SaveDirectory != "Default");
			UpdateIcon(game.Icon);

			checkBoxTMod.IsChecked = game.IsTMod;

			// Remove quotes from "Copy Path" command on paste
			DataObject.AddPastingHandler(textBoxCustomIcon, OnTextBoxQuotesPaste);
			DataObject.AddPastingHandler(textBoxExe, OnTextBoxQuotesPaste);
			DataObject.AddPastingHandler(textBoxSaveFolder, OnTextBoxQuotesPaste);

			// Disable drag/drop text in textboxes so you can scroll their contents easily
			DataObject.AddCopyingHandler(textBoxName, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxDetails, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxCustomIcon, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxExe, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxSaveFolder, OnTextBoxCancelDrag);
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
			dialog.SelectedPath = textBoxSaveFolder.Text;
			var result = dialog.ShowFolderBrowser(this);
			if (result.HasValue && result.Value) {
				textBoxSaveFolder.Text = dialog.SelectedPath;
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

		private void OnDefaultSaveFolderChecked(object sender, RoutedEventArgs e) {
			if (checkBoxDefaultSaveFolder.IsChecked.Value) {
				textBoxSaveFolder.IsEnabled = false;
				nonDefaultSaveFolder = textBoxSaveFolder.Text;
				textBoxSaveFolder.Text = "Default";
			}
			else {
				textBoxSaveFolder.IsEnabled = true;
				textBoxSaveFolder.Text = nonDefaultSaveFolder;
			}
		}


		public static bool ShowDialog(Window owner, Game game) {
			EditGameWindow window = new EditGameWindow(game);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				game.Name = window.textBoxName.Text;
				game.Details = window.textBoxDetails.Text;
				game.ExePath = window.textBoxExe.Text;
				if (window.checkBoxDefaultSaveFolder.IsChecked.Value)
					game.SaveDirectory = "Default";
				else
					game.SaveDirectory = window.textBoxSaveFolder.Text;
				if (window.checkBoxCustomIcon.IsChecked.Value)
					game.Icon = window.textBoxCustomIcon.Text;
				else
					game.Icon = window.comboBoxIcon.SelectedItem as string;
				game.IsTMod = window.checkBoxTMod.IsChecked.Value;
				return true;
			}
			return false;
		}

		private void UpdateIcon(string icon) {
			BitmapSource bitmap = Setup.LoadIcon(icon, "Tree");
			imageIcon.Source = bitmap;
			imageIcon.Width = Math.Min(68, bitmap.PixelWidth);
			imageIcon.Height = Math.Min(68, bitmap.PixelHeight);
		}
	}
}
