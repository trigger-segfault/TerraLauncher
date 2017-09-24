using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Timer = System.Timers.Timer;

namespace TerraLauncher.Controls.Terraria {
	public class TerrariaWindow : ContentControl {
		static TerrariaWindow() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TerrariaWindow),
					   new FrameworkPropertyMetadata(typeof(TerrariaWindow)));
		}
		
		bool resizing = false;

		public TerrariaWindow() {

		}


		protected override void OnRender(DrawingContext d) {
			DrawCropped.DrawFrame(d, CroppedFrames.WindowFrame, ActualWidth, ActualHeight);
			base.OnRender(d);
		}

		private void OnDragWindow(object sender, MouseButtonEventArgs e) {
			Window.GetWindow(this).DragMove();
		}
		private void OnCloseWindow(object sender, MouseButtonEventArgs e) {
			Sounds.PlayClose();
			Window.GetWindow(this).Close();
		}
		private void OnMinimizeWindow(object sender, MouseButtonEventArgs e) {
			Sounds.PlayClose();
			Window.GetWindow(this).WindowState = WindowState.Minimized;
		}

		public override void OnApplyTemplate() {
			((TerrariaButton)GetTemplateChild("closeButton")).MouseUp += OnCloseWindow;
			((TerrariaButton)GetTemplateChild("minimizeButton")).MouseUp += OnMinimizeWindow;
			((Grid)GetTemplateChild("titleBar")).MouseLeftButtonDown += OnDragWindow;
			Grid grid = (Grid)GetTemplateChild("windowGrid");
			foreach (var child in grid.Children) {
				Rectangle rect = child as Rectangle;
				if (rect != null && rect.Name.EndsWith("SizeGrip")) {
					rect.MouseLeftButtonDown += OnResizeBegin;
					rect.MouseLeftButtonUp += OnResizeEnd;
					rect.MouseMove += OnResizeMove;
				}
			}
		}
		private void OnResizeBegin(object sender, MouseButtonEventArgs e) {
			Rectangle senderRect = sender as Rectangle;
			if (senderRect != null) {
				resizing = true;
				senderRect.CaptureMouse();
			}
		}

		private void OnResizeEnd(object sender, MouseButtonEventArgs e) {
			Rectangle senderRect = sender as Rectangle;
			if (senderRect != null) {
				resizing = false; ;
				senderRect.ReleaseMouseCapture();
			}
		}

		private void OnResizeMove(object sender, MouseEventArgs e) {
			if (resizing) {
				Rectangle senderRect = sender as Rectangle;
				Window mainWindow = senderRect.Tag as Window;
				if (senderRect != null) {
					double width = e.GetPosition(mainWindow).X;
					double height = e.GetPosition(mainWindow).Y;
					senderRect.CaptureMouse();
					if (senderRect.Name.ToLower().Contains("right")) {
						width += 5;
						if (width > 0)
							mainWindow.Width = width;
					}
					if (senderRect.Name.ToLower().Contains("left")) {
						width -= 5;
						double width2 = mainWindow.MinWidth - (mainWindow.Width - width);
						if (width2 > 0)
							width -= width2;
						mainWindow.Left += width;
						width = mainWindow.Width - width;
						if (width > 0) {
							mainWindow.Width = width;
						}
					}
					if (senderRect.Name.ToLower().Contains("bottom")) {
						height += 5;
						if (height > 0)
							mainWindow.Height = height;
					}
					if (senderRect.Name.ToLower().Contains("top")) {
						height -= 5;
						double height2 = mainWindow.MinHeight - (mainWindow.Height - height);
						if (height2 > 0)
							height -= height2;
						mainWindow.Top += height;
						height = mainWindow.Height - height;
						if (height > 0) {
							mainWindow.Height = height;
						}
					}
				}
			}
		}
	}
}
