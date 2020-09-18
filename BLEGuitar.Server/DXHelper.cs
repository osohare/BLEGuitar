using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLEGuitar.Server
{
    /// <summary>
    /// http://www.flint.jp/misc/?q=dik&lang=en
    /// https://community.bistudio.com/wiki/DIK_KeyCodes
    /// </summary>
    public enum DIKKeyCode : short
    {
        DIK_ESCAPE = 0x01,
        DIK_1 = 0x02,
        DIK_2 = 0x03,
        DIK_3 = 0x04,
        DIK_4 = 0x05,
        DIK_5 = 0x06,
        DIK_6 = 0x07,
        DIK_7 = 0x08,
        DIK_8 = 0x09,
        DIK_9 = 0x0A,
        DIK_0 = 0x0B,
        DIK_RETURN = 0x1C,
        DIK_DOWN = 0xD0,
        DIK_UP = 0xC8,
        DIK_LEFT = 0xCB,
        DIK_RIGHT = 0xCD
    }

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

            InputData[0].type = 1;
            InputData[0].ki.wScan = keycode;
            InputData[0].ki.dwFlags = (int)(flag | KeyFlag.ScanCode);
            InputData[0].ki.time = 0;
            InputData[0].ki.dwExtraInfo = IntPtr.Zero;

            SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
