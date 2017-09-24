using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TerraLauncher.Util {
	public static class Extensions {
		/**<summary>Trims whitespace off of a string.</summary>*/
		public static string TrimWhitespace(this string s) {
			return s.Trim(' ', '\t', '\r', '\n');
		}
		/**<summary>Fills an array with a value.</summary>*/
		public static void Fill<T>(this T[] array, T with) {
			for (int i = 0; i < array.Length; i++) {
				array[i] = with;
			}
		}
		/**<summary>Swaps two items within an array.</summary>*/
		public static void Swap<T>(this T[] array, int indexA, int indexB) {
			T swap = array[indexA];
			array[indexA] = array[indexB];
			array[indexB] = swap;
		}
		/**<summary>Moves an item within a list.</summary>*/
		public static void Move<T>(this List<T> list, int oldIndex, int newIndex) {
			T t = list[oldIndex];
			list.RemoveAt(oldIndex);
			list.Insert(newIndex, t);
		}
		/**<summary>Swaps two items within a list.</summary>*/
		public static void Swap<T>(this List<T> list, int indexA, int indexB) {
			T swap = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = swap;
		}
		/**<summary>Tests if a collection is empty.</summary>*/
		public static bool IsEmpty<T>(this ICollection<T> collection) {
			return (collection.Count == 0);
		}
		/**<summary>Shows a process's window.</summary>*/
		public static void Show(this Process process) {
			ShowWindow(process.MainWindowHandle, SW_RESTORE);
		}
		/**<summary>Shows a process's window.</summary>*/
		public static void Hide(this Process process) {
			ShowWindow(process.MainWindowHandle, SW_HIDE);
		}

		public static void SelectItem(this TreeView treeView, object item) {
			var tvItem = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
			if (tvItem != null) {
				tvItem.IsSelected = true;
			}
		}

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		private const int SW_RESTORE = 9;
		private const int SW_HIDE = 0;

		/**<summary>Sets an enum's flag.</summary>*/
		public static TEnum SetFlag<TEnum>(this Enum enumValue, TEnum flag, bool set = true)
			where TEnum : struct, IComparable, IFormattable, IConvertible {
			Type underlyingType = Enum.GetUnderlyingType(enumValue.GetType());

			// note: AsInt mean: math integer vs enum (not the c# int type)
			dynamic valueAsInt = Convert.ChangeType(enumValue, underlyingType);
			dynamic flagAsInt = Convert.ChangeType(flag, underlyingType);
			if (set)
				valueAsInt |= flagAsInt;
			else
				valueAsInt &= ~flagAsInt;
			return (TEnum)valueAsInt;
		}
		/**<summary>Unsets an enum's flag.</summary>*/
		public static TEnum UnsetFlag<TEnum>(this Enum enumValue, TEnum flag)
			where TEnum : struct, IComparable, IFormattable, IConvertible {
			Type underlyingType = Enum.GetUnderlyingType(enumValue.GetType());

			// note: AsInt mean: math integer vs enum (not the c# int type)
			dynamic valueAsInt = Convert.ChangeType(enumValue, underlyingType);
			dynamic flagAsInt = Convert.ChangeType(flag, underlyingType);
			valueAsInt &= ~flagAsInt;
			return (TEnum)valueAsInt;
		}
	}
}
