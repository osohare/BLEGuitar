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
        private readonly IBLEGuitarDevice guitar = new BLEGuitar();
        private readonly DXHelper simulator = new DXHelper();

        private readonly Action<Commons.ButtonState, DIKKeyCode> ProcessButtonState;

        public Form1()
        {
            InitializeComponent();

            ProcessButtonState = (state, keycode) => 
            {
                if (state == Commons.ButtonState.Pressed)
                    simulator.KeyDown(keycode);
                else
                    simulator.KeyUp(keycode);
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
            ProcessButtonState(e.Snapshot.Fret1, DIKKeyCode.DIK_1);
            ProcessButtonState(e.Snapshot.Fret2, DIKKeyCode.DIK_2);
            ProcessButtonState(e.Snapshot.Fret3, DIKKeyCode.DIK_3);
            ProcessButtonState(e.Snapshot.Fret4, DIKKeyCode.DIK_4);
            ProcessButtonState(e.Snapshot.Fret5, DIKKeyCode.DIK_5);
            ProcessButtonState(e.Snapshot.Fret6, DIKKeyCode.DIK_6);

            ProcessButtonState(e.Snapshot.Pause, DIKKeyCode.DIK_RETURN);
            ProcessButtonState(e.Snapshot.LiveTV, DIKKeyCode.DIK_RETURN);
            ProcessButtonState(e.Snapshot.HeroPower, DIKKeyCode.DIK_RETURN);
            ProcessButtonState(e.Snapshot.Power, DIKKeyCode.DIK_RETURN);

            switch (e.Snapshot.StrumBar)
            {
                case Commons.StrumBarState.StrumBarNeutral:
                    simulator.KeyUp(DIKKeyCode.DIK_DOWN);
                    simulator.KeyUp(DIKKeyCode.DIK_UP);
                    break;
                case Commons.StrumBarState.StrumBarUp:
                    simulator.KeyDown(DIKKeyCode.DIK_UP);
                    break;
                case Commons.StrumBarState.StrumBarDown:
                    simulator.KeyDown(DIKKeyCode.DIK_DOWN);
                    break;
                default:
                    break;
            }

            //switch (e.Snapshot.Navigator)
            //{
            //    case 0:
            //        simulator.Keyboard.KeyPress(VirtualKeyCode.VK_W);
            //        break;
            //    case 1:
            //        simulator.Keyboard.KeyPress(VirtualKeyCode.VK_W);
            //        break;
            //    case 2:
            //        simulator.Keyboard.KeyPress(VirtualKeyCode.VK_A);
            //        break;
            //    case 3:
            //        simulator.Keyboard.KeyPress(VirtualKeyCode.VK_A);
            //        break;
            //    case 4:
            //        simulator.Keyboard.KeyPress(VirtualKeyCode.VK_S);
            //        break;
            //    case 5:
            //        simulator.Keyboard.KeyPress(VirtualKeyCode.VK_S);
            //        break;
            //    case 6:
            //        simulator.Keyboard.KeyPress(VirtualKeyCode.VK_D);
            //        break;
            //    case 7:
            //        simulator.Keyboard.KeyPress(VirtualKeyCode.VK_D);
            //        break;
            //    case 15:
            //    default:
            //        break;
            //}

            //e.Snapshot.WhammyBar
        }
    }
}
