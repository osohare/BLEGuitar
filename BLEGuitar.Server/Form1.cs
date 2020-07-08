using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLEGuitar.Server
{

    public partial class Form1 : Form
    {
        private readonly BLEGuitarFinder guitar = new BLEGuitarFinder();
        private readonly WindowsInput.InputSimulator simulator = new WindowsInput.InputSimulator();

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            guitar.FindAndConnect();
            guitar.DataReceived += Guitar_DataReceived;
        }

        private void Guitar_DataReceived(object sender, BLEGuitarFinder.DataReceivedArgs e)
        {
            if (e.Snapshot.Fret1 == Commons.ButtonState.Pressed)
                simulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_1);
            else
                simulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_1);

            if (e.Snapshot.Fret2 == Commons.ButtonState.Pressed)
                simulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_2);
            else
                simulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_2);

            if (e.Snapshot.Fret3 == Commons.ButtonState.Pressed)
                simulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_3);
            else
                simulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_3);

            if (e.Snapshot.Fret4 == Commons.ButtonState.Pressed)
                simulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_4);
            else
                simulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_4);

            if (e.Snapshot.Fret5 == Commons.ButtonState.Pressed)
                simulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_5);
            else
                simulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_5);

            if (e.Snapshot.Fret6 == Commons.ButtonState.Pressed)
                simulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_6);
            else
                simulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_6);

            switch (e.Snapshot.StrumBar)
            {
                case Commons.StrumBarState.StrumBarNeutral:
                    simulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.DOWN);
                    simulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.UP);
                    break;
                case Commons.StrumBarState.StrumBarUp:
                    simulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.UP);
                    break;
                case Commons.StrumBarState.StrumBarDown:
                    simulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.DOWN);
                    break;
                default:
                    break;
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await guitar.Disconnect();
            guitar.DataReceived -= Guitar_DataReceived;
        }
    }
}
