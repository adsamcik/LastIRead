﻿using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace LastIRead.Extensions {
	/// <summary>
	///     Extension methods for Key enum.
	/// </summary>
	public static class KeyExtensions {
		[DllImport("user32.dll")]
		private static extern int ToUnicode(
			uint wVirtKey,
			uint wScanCode,
			byte[] lpKeyState,
			[Out] [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
			StringBuilder pwszBuff,
			int cchBuff,
			uint wFlags
		);

		[DllImport("user32.dll")]
		private static extern bool GetKeyboardState(byte[] lpKeyState);

		[DllImport("user32.dll")]
		private static extern uint MapVirtualKey(uint uCode, MapType uMapType);

		/// <summary>
		///     Converts enum Key to unicode char.
		///     https://stackoverflow.com/a/5826175/2422905
		/// </summary>
		/// <param name="key">Pressed key</param>
		/// <returns>Unicode character</returns>
		public static char ToChar(this Key key) {
			var ch = ' ';

			var virtualKey = KeyInterop.VirtualKeyFromKey(key);
			var keyboardState = new byte[256];
			GetKeyboardState(keyboardState);

			var scanCode = MapVirtualKey((uint) virtualKey, MapType.MAPVK_VK_TO_VSC);
			var stringBuilder = new StringBuilder(2);

			var result = ToUnicode(
				(uint) virtualKey,
				scanCode,
				keyboardState,
				stringBuilder,
				stringBuilder.Capacity,
				0
			);
			switch (result) {
				case -1:
					break;
				case 0:
					break;
				case 1: {
					ch = stringBuilder[0];
					break;
				}
				default: {
					ch = stringBuilder[0];
					break;
				}
			}

			return ch;
		}

		private enum MapType : uint {
			MAPVK_VK_TO_VSC = 0x0,
			MAPVK_VSC_TO_VK = 0x1,
			MAPVK_VK_TO_CHAR = 0x2,
			MAPVK_VSC_TO_VK_EX = 0x3
		}
	}
}