using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLEGuitar.Server
{
    public class DXHelper
    {
        [Flags]
        private enum KeyFlag
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            UniCode = 0x0004,
            ScanCode = 0x0008
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mouseinput
        /// </summary>
        [Flags]
        private enum MouseFlag
        {
            MOUSEEVENTF_ABSOLUTE = 0x8000,
            MOUSEEVENTF_HWHEEL = 0x01000,
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-input
        /// </summary>
        private enum InputType : int
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }

        [DllImport("user32.dll")]
        static extern UInt32 SendInput(UInt32 nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] pInputs, Int32 cbSize);

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        public void KeyUp(DIKKeyCode keycode)
        {
            SendInput(KeyFlag.KeyUp, (short)keycode);
        }

        public void KeyDown(DIKKeyCode keycode)
        {
            SendInput(KeyFlag.KeyDown, (short)keycode);
        }

        private void SendInput(KeyFlag flag, short keycode)
        {
            INPUT[] InputData = new INPUT[1];

            InputData[0].type = (int)InputType.INPUT_KEYBOARD;
            InputData[0].ki.wScan = keycode;
            InputData[0].ki.dwFlags = (int)(flag | KeyFlag.ScanCode);
            InputData[0].ki.time = 0;
            InputData[0].ki.dwExtraInfo = IntPtr.Zero;

            SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)));
        }

        private void SendMouseInput(int x, int y)
        {
            INPUT[] InputData = new INPUT[1];
            InputData[0].type = (int)InputType.INPUT_MOUSE;
            InputData[0].mi.mouseData = 0;
            InputData[0].mi.time = 0;
            InputData[0].mi.dx = x;
            InputData[0].mi.dy = y;
            InputData[0].mi.dwFlags = (int)MouseFlag.MOUSEEVENTF_MOVE;
            SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
