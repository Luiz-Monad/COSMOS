﻿using System;
using System.Collections.Generic;
using Encoding = System.Text.Encoding;
using Plug = Cosmos.IL2CPU.Plugs.PlugAttribute;

namespace Cosmos.System.Plugs.System.System {
	[Plug(Target = typeof(global::System.Console))]
	public static class ConsoleImpl {

		private static ConsoleColor mForeground = ConsoleColor.White;
		private static ConsoleColor mBackground = ConsoleColor.Black;

		public static ConsoleColor get_BackgroundColor() {
			return mBackground;
		}

		public static void set_BackgroundColor(ConsoleColor value) {
			mBackground = value;
			Cosmos.Hardware.Global.TextScreen.SetColors(mForeground, mBackground);
		}

		public static int get_BufferHeight() {
			WriteLine("Not implemented: get_BufferHeight");
			return -1;
		}

		public static void set_BufferHeight(int aHeight) {
			WriteLine("Not implemented: set_BufferHeight");
		}

		public static int get_BufferWidth() {
			WriteLine("Not implemented: get_BufferWidth");
			return -1;
		}

        public static void set_BufferWidth(int aWidth) {
			WriteLine("Not implemented: set_BufferWidth");
		}

		public static bool get_CapsLock() {
			WriteLine("Not implemented: get_CapsLock");
			return false;
		}

		public static int get_CursorLeft() {
			return Global.Console.X;
		}

		public static void set_CursorLeft(int x) {
			Global.Console.X = x;
		}

		public static int get_CursorSize() {
			WriteLine("Not implemented: get_CursorSize");
			return -1;
		}

		public static void set_CursorSize(int aSize) {
			WriteLine("Not implemented: set_CursorSize");
		}

		public static int get_CursorTop() {
			return Global.Console.Y;
		}

		public static void set_CursorTop(int y) {
			Global.Console.Y = y;
		}

		public static bool get_CursorVisible() {
			WriteLine("Not implemented: get_CursorVisible");
			return false;
		}

		public static void set_CursorVisible(bool value) {
			WriteLine("Not implemented: set_CursorVisible");
		}

		//public static TextWriter get_Error() {
		//    WriteLine("Not implemented: get_Error");
		//    return null;
		//}

		public static ConsoleColor get_ForegroundColor() {
			return mForeground;
		}

		public static void set_ForegroundColor(ConsoleColor value) {
			mForeground = value;
			Cosmos.Hardware.Global.TextScreen.SetColors(mForeground, mBackground);
		}

		//public static TextReader get_In()
		//{
		//    WriteLine("Not implemented: get_In");
		//    return null;
		//}

		public static Encoding get_InputEncoding()
		{
			WriteLine("Not implemented: get_InputEncoding");
			return null;
		}

		public static void set_InputEncoding(Encoding value)
		{
			WriteLine("Not implemented: set_InputEncoding");
		}

		public static bool get_KeyAvailable()
		{
			WriteLine("Not implemented: get_KeyAvailable");
			return false;
		}

		public static int get_LargestWindowHeight()
		{
			WriteLine("Not implemented: get_LargestWindowHeight");
			return -1;
		}

		public static int get_LargestWindowWidth() {
			WriteLine("Not implemented: get_LargestWindowWidth");
			return -1;
		}

		public static bool get_NumberLock() {
			WriteLine("Not implemented: get_NumberLock");
			return false;
		}

		//public static TextWriter get_Out() {
		//    WriteLine("Not implemented: get_Out");
		//    return null;
		//}

		public static Encoding get_OutputEncoding() {
			WriteLine("Not implemented: get_OutputEncoding");
			return null;
		}

		public static void set_OutputEncoding(Encoding value) {
			WriteLine("Not implemented: set_OutputEncoding");
		}

		public static string get_Title() {
			WriteLine("Not implemented: get_Title");
			return string.Empty;
		}

		public static void set_Title(string value) {
			WriteLine("Not implemented: set_Title");
		}

		public static bool get_TreatControlCAsInput() {
			WriteLine("Not implemented: get_TreatControlCAsInput");
			return false;
		}

		public static void set_TreatControlCAsInput(bool value) {
			WriteLine("Not implemented: set_TreatControlCAsInput");
		}

		public static int get_WindowHeight() {
			return Global.Console.Rows;
		}

		public static void set_WindowHeight(int value) {
			WriteLine("Not implemented: set_WindowHeight");
		}

		public static int get_WindowLeft() {
			WriteLine("Not implemented: get_WindowLeft");
			return -1;
		}

		public static void set_WindowLeft(int value) {
			WriteLine("Not implemented: set_WindowLeft");
		}

		public static int get_WindowTop() {
			WriteLine("Not implemented: get_WindowTop");
			return -1;
		}

		public static void set_WindowTop(int value) {
			WriteLine("Not implemented: set_WindowTop");
		}

		public static int get_WindowWidth() {
			return Global.Console.Cols;
		}

		public static void set_WindowWidth(int value) {
			WriteLine("Not implemented: set_WindowWidth");
		}

		// Beep() is pure CIL

		public static void Beep(int aFrequency, int aDuration) {
			if (aFrequency < 37 || aFrequency > 32767) {
				throw new ArgumentOutOfRangeException("Frequency must be between 37 and 32767Hz");
			}

			if (aDuration <= 0) {
				throw new ArgumentOutOfRangeException("Duration must be more than 0");
			}

			WriteLine("Not implemented: Beep");

			//var xPIT = Hardware.Global.PIT;
			//xPIT.EnableSound();
			//xPIT.T2Frequency = (uint)aFrequency;
			//xPIT.Wait((uint)aDuration);
			//xPIT.DisableSound();
		}

		//TODO: Console uses TextWriter - intercept and plug it instead
		public static void Clear() {
			Global.Console.Clear();
		}

		//  MoveBufferArea(int, int, int, int, int, int) is pure CIL

		public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, Char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor) {
			WriteLine("Not implemented: MoveBufferArea");
		}

		//public static Stream OpenStandardError() {
		//    WriteLine("Not implemented: OpenStandardError");
		//}

		//public static Stream OpenStandardError(int bufferSize) {
		//    WriteLine("Not implemented: OpenStandardError");
		//}

		//public static Stream OpenStandardInput(int bufferSize) {
		//    WriteLine("Not implemented: OpenStandardInput");
		//}

		//public static Stream OpenStandardInput() {
		//    WriteLine("Not implemented: OpenStandardInput");
		//}

		//public static Stream OpenStandardOutput(int bufferSize) {
		//    WriteLine("Not implemented: OpenStandardOutput");
		//}

		//public static Stream OpenStandardOutput() {
		//    WriteLine("Not implemented: OpenStandardOutput");
		//}

		public static int Read() {
			// TODO special cases, if needed, that returns -1
			return Hardware.Global.Keyboard.ReadChar();
		}
		
		// ReadKey() pure CIL

		public static ConsoleKeyInfo ReadKey(Boolean intercept) {
			var key = Cosmos.Hardware.Global.Keyboard.ReadMapping();
			var returnValue = new ConsoleKeyInfo(
				key.Value,
				key.Key,
				Cosmos.Hardware.Global.Keyboard.ShiftPressed,
				Cosmos.Hardware.Global.Keyboard.AltPressed,
				Cosmos.Hardware.Global.Keyboard.CtrlPressed);

			if (false == intercept)
			{
				Write(returnValue.KeyChar);
			}
			return returnValue;
		}

		public static String ReadLine() {
			List<char> chars = new List<char>(32);
			char current;
			int currentCount = 0;

			while ((current = Hardware.Global.Keyboard.ReadChar()) != '\n')
			{
				//Check for "special" keys
				if (current == '\u0968') // Backspace   
				{
					if (currentCount > 0)
					{
						int curCharTemp = Global.Console.X;
						chars.RemoveAt(currentCount - 1);
						Global.Console.X = Global.Console.X - 1;

						//Move characters to the left
						for (int x = currentCount - 1; x < chars.Count; x++)
						{
							Write(chars[x]);
						}

						Write(' ');

						Global.Console.X = curCharTemp - 1;

						currentCount--;
					}
					continue;
				}
				else if (current == '\u2190') // Arrow Left
				{
					if (currentCount > 0)
					{
						Global.Console.X = Global.Console.X - 1;
						currentCount--;
					}
					continue;
				}
				else if (current == '\u2192') // Arrow Right
				{
					if (currentCount < chars.Count)
					{
						Global.Console.X = Global.Console.X + 1;
						currentCount++;
					}
					continue;
				}

				//Write the character to the screen
				if (currentCount == chars.Count)
				{
					chars.Add(current);
					Write(current);
					currentCount++;
				}
				else
				{
					//Insert the new character in the correct location
					//For some reason, List.Insert() doesn't work properly
					//so the character has to be inserted manually
					List<char> temp = new List<char>();

					for (int x = 0; x < chars.Count; x++)
					{
						if (x == currentCount)
						{
							temp.Add(current);
						}

						temp.Add(chars[x]);
					}

					chars = temp;

					//Shift the characters to the right
					for (int x = currentCount; x < chars.Count; x++)
					{
						Write(chars[x]);
					}

					Global.Console.X -= (chars.Count - currentCount) - 1;
					currentCount++;
				}
			}
			WriteLine();

			char[] final = chars.ToArray();
			return new string(final);
		}

		public static void ResetColor() {
			WriteLine("Not implemented: ResetColor");
		}

		public static void SetBufferSize(int width, int height) {
			WriteLine("Not implemented: SetBufferSize");
		}

		public static void SetCursorPosition(int left, int top) {
			WriteLine("Not implemented: SetCursorPosition");
		}

		//public static void SetError(TextWriter newError) {
		//    WriteLine("Not implemented: SetError");
		//}

		//public static void SetIn(TextReader newIn) {
		//    WriteLine("Not implemented: SetIn");
		//}

		//public static void SetOut(TextWriter newOut) {
		//    WriteLine("Not implemented: SetOut");
		//}

		public static void SetWindowPosition(int left, int top) {
			WriteLine("Not implemented: SetWindowPosition");
		}

		public static void SetWindowSize(int width, int height) {
			WriteLine("Not implemented: SetWindowSize");
		}

        #region Write

		public static void Write(bool aBool) {
			Write(aBool.ToString());
		}

		public static void Write(char aChar) {
			Global.Console.WriteChar(aChar);
		}

        public static void Write(char[] aBuffer) {
            Write(aBuffer, 0, aBuffer.Length);
        }

		//public static void Write(decimal aBuffer) {
		//    Write("No Decimal.ToString()");
		//}

		public static void Write(double aDouble) {
			Write(aDouble.ToString());
		}

		public static void Write(float aFloat) {
			Write(aFloat.ToString());
		}

		public static void Write(int aInt) {
			Write(aInt.ToString());
		}

		public static void Write(long aLong) {
			Write(aLong.ToString());
		}

		public static void Write(object value) {
			if (value != null) {
				Write(value.ToString());
			}
		}

		public static void Write(string aText) {
			Global.Console.Write(aText);
		}

		public static void Write(uint aInt) {
			Write(aInt.ToString());
		}

		public static void Write(ulong aLong) {
			Write(aLong.ToString());
		}

		public static void Write(string format, object arg0) {
			WriteLine("Not implemented: Write");
		}

		public static void Write(string format, params object[] arg) {
			WriteLine("Not implemented: Write");
		}

		public static void Write(char[] aBuffer, int aIndex, int aCount) {
			if (aBuffer == null) {
				throw new ArgumentNullException("aBuffer");
			}
			if (aIndex < 0) {
				throw new ArgumentOutOfRangeException("aIndex");
			}
			if (aCount < 0) {
				throw new ArgumentOutOfRangeException("aCount");
			}
			if ((aBuffer.Length - aIndex) < aCount) {
				throw new ArgumentException();
			}
			for (int i = 0; i < aCount; i++) {
				Write(aBuffer[aIndex + i]);
			}
		}

		public static void Write(string format, object arg0, object arg1) {
			WriteLine("Not implemented: Write");
		}

		public static void Write(string format, object arg0, object arg1, object arg2) {
			WriteLine("Not implemented: Write");
		}

		public static void Write(string format, object arg0, object arg1, object arg2, object arg3) {
			WriteLine("Not implemented: Write");
		}

        //You'd expect this to be on System.Console wouldn't you? Well, it ain't so we just rely on Write(object value)
        //public static void Write(byte aByte) {
        //    Write(aByte.ToString());
        //}

        #endregion

        #region WriteLine

        public static void WriteLine() {
            Global.Console.NewLine();
        }

		public static void WriteLine(bool aBool) {
			Write(aBool.ToString());
			Global.Console.NewLine();
		}

		public static void WriteLine(char aChar) {
			Write(aChar);
			Global.Console.NewLine();
		}

		public static void WriteLine(char[] aBuffer) {
			Write(aBuffer, 0, aBuffer.Length);
			Global.Console.NewLine();
		}

		//public static void WriteLine(decimal aDecimal) {
		//    Write(aDecimal);
		//    Global.Console.NewLine();
		//}

		public static void WriteLine(double aDouble) {
			Write(aDouble.ToString());
			Global.Console.NewLine();
		}

		public static void WriteLine(float aFloat) {
			Write(aFloat.ToString());
			Global.Console.NewLine();
		}

		public static void WriteLine(int aInt) {
			Write(aInt.ToString());
			Global.Console.NewLine();
		}

		public static void WriteLine(long aLong) {
			Write(aLong.ToString());
			Global.Console.NewLine();
		}

		public static void WriteLine(object value) {
			if (value != null)
			{
				Write(value.ToString());
				Global.Console.NewLine();
			}
		}

		public static void WriteLine(string aText) {
			Global.Console.Write(aText);
			Global.Console.NewLine();
		}

		public static void WriteLine(uint aInt) {
			Write(aInt.ToString());
			Global.Console.NewLine();
		}

		public static void WriteLine(ulong aLong) {
			Write(aLong.ToString());
			Global.Console.NewLine();
		}

		public static void WriteLine(string format, object arg0) {
			WriteLine("Not implemented: WriteLine");
		}

		public static void WriteLine(string format, params object[] arg) {
			WriteLine("Not implemented: WriteLine");
		}

		public static void WriteLine(char[] aBuffer, int aIndex, int aCount) {
			Write(aBuffer, aIndex, aCount);
			Global.Console.NewLine();
		}

		public static void WriteLine(string format, object arg0, object arg1) {
			WriteLine("Not implemented: WriteLine");
		}

		public static void WriteLine(string format, object arg0, object arg1, object arg2) {
			WriteLine("Not implemented: WriteLine");
		}

		public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3) {
			WriteLine("Not implemented: WriteLine");
		}
        #endregion
    }
}