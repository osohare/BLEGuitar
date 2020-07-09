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
using WindowsInput.Native;

namespace BLEGuitar.Server
{

    public partial class Form1 : Form
    {
        private readonly IBLEGuitarDevice guitar = new BLEGuitar();
        private readonly WindowsInput.InputSimulator simulator = new WindowsInput.InputSimulator();

        private readonly Action<Commons.ButtonState, VirtualKeyCode> ProcessButtonState;

        public Form1()
        {
            InitializeComponent();

            ProcessButtonState = (state, keycode) => 
            {
                if (state == Commons.ButtonState.Pressed)
                    simulator.Keyboard.KeyDown(keycode);
                else
                    simulator.Keyboard.KeyUp(keycode);
            };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            guitar.FindAndConnect();
            guitar.DataReceived += Guitar_DataReceived;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            guitar.DataReceived -= Guitar_DataReceived;
            var t = guitar.Disconnect();
            t.Wait();
            button2.Enabled = true;
        }

        private void Guitar_DataReceived(object sender, DataReceivedArgs e)
        {
            ProcessButtonState(e.Snapshot.Fret1, VirtualKeyCode.VK_1);
            ProcessButtonState(e.Snapshot.Fret2, VirtualKeyCode.VK_2);
            ProcessButtonState(e.Snapshot.Fret3, VirtualKeyCode.VK_3);
            ProcessButtonState(e.Snapshot.Fret4, VirtualKeyCode.VK_4);
            ProcessButtonState(e.Snapshot.Fret5, VirtualKeyCode.VK_5);
            ProcessButtonState(e.Snapshot.Fret6, VirtualKeyCode.VK_6);

            ProcessButtonState(e.Snapshot.Pause, VirtualKeyCode.VK_P);
            ProcessButtonState(e.Snapshot.LiveTV, VirtualKeyCode.VK_P);
            ProcessButtonState(e.Snapshot.HeroPower, VirtualKeyCode.VK_P);
            ProcessButtonState(e.Snapshot.Power, VirtualKeyCode.VK_P);

            switch (e.Snapshot.StrumBar)
            {
                case Commons.StrumBarState.StrumBarNeutral:
                    simulator.Keyboard.KeyUp(VirtualKeyCode.DOWN);
                    simulator.Keyboard.KeyUp(VirtualKeyCode.UP);
                    break;
                case Commons.StrumBarState.StrumBarUp:
                    simulator.Keyboard.KeyDown(VirtualKeyCode.UP);
                    break;
                case Commons.StrumBarState.StrumBarDown:
                    simulator.Keyboard.KeyDown(VirtualKeyCode.DOWN);
                    break;
                default:
                    break;
            }

            switch (e.Snapshot.Navigator)
            {
                case 0:
                    simulator.Keyboard.KeyPress(VirtualKeyCode.VK_W);
                    break;
                case 1:
                    simulator.Keyboard.KeyPress(VirtualKeyCode.VK_W);
                    break;
                case 2:
                    simulator.Keyboard.KeyPress(VirtualKeyCode.VK_A);
                    break;
                case 3:
                    simulator.Keyboard.KeyPress(VirtualKeyCode.VK_A);
                    break;
                case 4:
                    simulator.Keyboard.KeyPress(VirtualKeyCode.VK_S);
                    break;
                case 5:
                    simulator.Keyboard.KeyPress(VirtualKeyCode.VK_S);
                    break;
                case 6:
                    simulator.Keyboard.KeyPress(VirtualKeyCode.VK_D);
                    break;
                case 7:
                    simulator.Keyboard.KeyPress(VirtualKeyCode.VK_D);
                    break;
                case 15:
                default:
                    break;
            }

            //e.Snapshot.WhammyBar
        }
    }
}
