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
	public partial class EditToolWindow : Window {

		public EditToolWindow(Tool tool) {
			InitializeComponent();

			// Init icon combo box

			int index = 0;
			foreach (var pair in Setup.SetupIcons) {
				comboBoxIcon.Items.Add(pair.Key);
				if (tool.Icon == pair.Key)
					comboBoxIcon.SelectedIndex = index;
				index++;
			}
			if (comboBoxIcon.SelectedIndex == -1)
				comboBoxIcon.SelectedIndex = 0;

			textBoxName.Text = tool.Name;
			textBoxDetails.Text = tool.Details;
			if (Setup.SetupIcons.ContainsKey(tool.Icon)) {
				comboBoxIcon.Visibility = Visibility.Visible;
				textBoxCustomIcon.Visibility = Visibility.Hidden;
				buttonBrowseCustomIcon.Visibility = Visibility.Hidden;

				checkBoxCustomIcon.IsChecked = false;
			}
			else {
				textBoxCustomIcon.Text = tool.Icon;
				comboBoxIcon.Visibility = Visibility.Hidden;
				textBoxCustomIcon.Visibility = Visibility.Visible;
				buttonBrowseCustomIcon.Visibility = Visibility.Visible;

				checkBoxCustomIcon.IsChecked = true;
			}

			textBoxExe.Text = tool.ExePath;
			textBoxArguments.Text = tool.Arguments;
			textBoxProject.Text = tool.ProjectPath;
			checkBoxDeveloper.IsChecked = !string.IsNullOrWhiteSpace(tool.ProjectPath);
			textBoxProject.IsEnabled = !string.IsNullOrWhiteSpace(tool.ProjectPath);
			UpdateIcon(tool.Icon);

			// Remove quotes from "Copy Path" command on paste
			DataObject.AddPastingHandler(textBoxCustomIcon, OnTextBoxQuotesPaste);
			DataObject.AddPastingHandler(textBoxExe, OnTextBoxQuotesPaste);
			DataObject.AddPastingHandler(textBoxProject, OnTextBoxQuotesPaste);

			// Disable drag/drop text in textboxes so you can scroll their contents easily
			DataObject.AddCopyingHandler(textBoxName, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxDetails, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxCustomIcon, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxExe, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxProject, OnTextBoxCancelDrag);
			DataObject.AddCopyingHandler(textBoxArguments, OnTextBoxCancelDrag);
		}

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			// Make text boxes lose focus on click away
			FocusManager.SetFocusedElement(this, this);
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

		private void OnExeChanged(object sender, TextChangedEventArgs e) {

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

		private void OnBrowseProject(object sender, RoutedEventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Choose a Project File";
			dialog.FileName = textBoxProject.Text;
			dialog.Filter = "Solution Files|*.sln|All Files|*.*";
			dialog.FilterIndex = 0;
			dialog.CheckFileExists = true;
			try {
				dialog.InitialDirectory = Path.GetDirectoryName(textBoxProject.Text);
			}
			catch { }
			var result = dialog.ShowDialog(this);
			if (result.HasValue && result.Value) {
				textBoxProject.Text = dialog.FileName;
			}
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

		private void OnDeveloperChecked(object sender, RoutedEventArgs e) {
			textBoxProject.IsEnabled = checkBoxDeveloper.IsChecked.Value;
		}


		public static bool ShowDialog(Window owner, Tool tool) {
			EditToolWindow window = new EditToolWindow(tool);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				tool.Name = window.textBoxName.Text;
				tool.Details = window.textBoxDetails.Text;
				tool.ExePath = window.textBoxExe.Text;
				tool.Arguments = window.textBoxArguments.Text;
				if (window.checkBoxDeveloper.IsChecked.Value)
					tool.ProjectPath = window.textBoxProject.Text;
				else
					tool.ProjectPath = "";
				if (window.checkBoxCustomIcon.IsChecked.Value)
					tool.Icon = window.textBoxCustomIcon.Text;
				else
					tool.Icon = window.comboBoxIcon.SelectedItem as string;
				return true;
			}
			return false;
		}

		private void UpdateIcon(string icon) {
			BitmapSource bitmap = Setup.LoadIcon(icon, "Tool");
			imageIcon.Source = bitmap;
			imageIcon.Width = Math.Min(68, bitmap.PixelWidth);
			imageIcon.Height = Math.Min(68, bitmap.PixelHeight);
		}
		
		private void OnCustomIconLostFocus(object sender, RoutedEventArgs e) {
			if (checkBoxCustomIcon.IsChecked.Value)
				UpdateIcon(textBoxCustomIcon.Text);
		}
	}
}
