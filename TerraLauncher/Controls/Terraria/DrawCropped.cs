using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TerraLauncher.Controls.Terraria {

	public class CroppedFrame {
		public CroppedBitmap[] Crops;

		public int Corner;
		public int Side;
	}

	public static class CroppedFrames {

		public static CroppedFrame WindowFrame = null;
		public static CroppedFrame SetupFrame = null;
		public static CroppedFrame SetupFrameFocused = null;
		public static CroppedFrame ButtonFrame = null;
		public static CroppedFrame ButtonFrameLight = null;
		public static CroppedFrame ButtonFrameDark = null;

		static CroppedFrames() {
			string uri = "pack://application:,,,/TerraLauncher;component/Resources/Terraria/Controls/";
			WindowFrame = DrawCropped.GetCroppedFrame(10, 2, uri + "WindowFrame.png");

			SetupFrame = DrawCropped.GetCroppedFrame(6, 2, uri + "SetupFrame.png");
			SetupFrameFocused = DrawCropped.GetCroppedFrame(6, 2, uri + "SetupFrameFocused.png");

			ButtonFrame = DrawCropped.GetCroppedFrame(4, 2, uri + "ButtonFrame.png");
			ButtonFrameLight = DrawCropped.GetCroppedFrame(4, 2, uri + "ButtonFrameLight.png");
			ButtonFrameDark = DrawCropped.GetCroppedFrame(4, 2, uri + "ButtonFrameDark.png");
		}
	}

	public static class DrawCropped {

		private static void DrawFrame(DrawingContext d, CroppedBitmap[] crops, int c, int s, double w, double h) {
			if (w - c * 2 < 0 || h - c * 2 < 0)
				return;

			d.DrawImage(crops[0], new Rect(0, 0, c, c));
			d.DrawImage(crops[1], new Rect(w - c, 0, c, c));
			d.DrawImage(crops[2], new Rect(0, h - c, c, c));
			d.DrawImage(crops[3], new Rect(w - c, h - c, c, c));

			d.DrawImage(crops[4], new Rect(c, 0, w - c * 2, c));
			d.DrawImage(crops[5], new Rect(0, c, c, h - c * 2));
			d.DrawImage(crops[6], new Rect(c, h - c, w - c * 2, c));
			d.DrawImage(crops[7], new Rect(w - c, c, c, h - c * 2));

			d.DrawImage(crops[8], new Rect(c, c, w - c * 2, h - c * 2));
		}
		public static void DrawFrame(DrawingContext d, CroppedFrame frame, double width, double height) {
			DrawFrame(d, frame.Crops, frame.Corner, frame.Side, width, height);
		}

		private static CroppedBitmap[] GetCroppedBitmaps(BitmapSource source, int c, int s) {
			return new CroppedBitmap[] {
				new CroppedBitmap(source, new Int32Rect(0, 0, c, c)),
				new CroppedBitmap(source, new Int32Rect(c+s, 0, c, c)),
				new CroppedBitmap(source, new Int32Rect(0, c+s, c, c)),
				new CroppedBitmap(source, new Int32Rect(c+s, c+s, c, c)),

				new CroppedBitmap(source, new Int32Rect(c, 0, s, c)),
				new CroppedBitmap(source, new Int32Rect(0, c, c, s)),
				new CroppedBitmap(source, new Int32Rect(c, c+s, s, c)),
				new CroppedBitmap(source, new Int32Rect(c+s, c, c, s)),

				new CroppedBitmap(source, new Int32Rect(c, c, s, s))
			};
		}
		public static CroppedFrame GetCroppedFrame(BitmapSource source, int corner, int side) {
			CroppedFrame frame = new CroppedFrame();
			frame.Crops = GetCroppedBitmaps(source, corner, side);
			frame.Corner = corner;
			frame.Side = side;
			return frame;
		}
		public static CroppedFrame GetCroppedFrame(int corner, int side, string uri) {
			BitmapImage source = new BitmapImage(new Uri(uri));
			CroppedFrame frame = new CroppedFrame();
			frame.Crops = GetCroppedBitmaps(source, corner, side);
			frame.Corner = corner;
			frame.Side = side;
			return frame;
		}
	}
}
