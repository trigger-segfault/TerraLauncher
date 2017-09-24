using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TerraLauncher.Controls.Terraria {
	public class TerrariaButton : ContentControl {
		static TerrariaButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TerrariaButton),
					   new FrameworkPropertyMetadata(typeof(TerrariaButton)));
		}


		/*static BitmapSource light;
		static BitmapSource dark;
		static BitmapSource normal;*/

		bool inside = false;
		bool down = false;


		public TerrariaButton() {

		}

		protected override void OnRender(DrawingContext d) {
			CroppedFrame frame = CroppedFrames.ButtonFrame;
			if (down)
				frame = CroppedFrames.ButtonFrameDark;
			else if (inside)
				frame = CroppedFrames.ButtonFrameLight;
			
			DrawCropped.DrawFrame(d, frame, ActualWidth, ActualHeight);
			base.OnRender(d);
		}

		private void OnMouseEnter(object sender, MouseEventArgs e) {
			inside = true;
			Sounds.PlayTick();
			InvalidateVisual();
		}

		private void OnMouseLeave(object sender, MouseEventArgs e) {
			inside = false;
			if (!down)
				InvalidateVisual();
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			down = true;
			this.CaptureMouse();
			InvalidateVisual();
		}

		private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			down = false;
			this.ReleaseMouseCapture();
			InvalidateVisual();
		}
		public override void OnApplyTemplate() {
			this.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
			this.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
			this.MouseEnter += OnMouseEnter;
			this.MouseLeave += OnMouseLeave;
		}
	}
}
