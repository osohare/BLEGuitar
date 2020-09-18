using BLEGuitar.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ButtonState = BLEGuitar.Commons.ButtonState;

namespace BLEGuitar.Server
{

    public partial class Form1 : Form
    {
        private readonly IBLEGuitarDevice guitar = new BLEGuitar();
        private readonly DXHelper simulator = new DXHelper();

        private readonly Action<ButtonState, ButtonState, DIKKeyCode> ProcessButtonState;
        private GuitarButtonsState PreviousSnapshot = new GuitarButtonsState();

        private readonly Dictionary<string, DIKKeyCode> KeyMap = new Dictionary<string, DIKKeyCode>();

        public Form1()
        {
            InitializeComponent();

            foreach (var property in PreviousSnapshot.GetType().GetProperties())
            {
                string keyValue = ConfigurationManager.AppSettings[property.Name];
                if (Enum.TryParse(keyValue, true, out DIKKeyCode keyMap))
                {
                    KeyMap.Add(property.Name, keyMap);
                }
            }

            ProcessButtonState = (oldState, newState, keycode) =>
            {
                if (newState == ButtonState.Pressed)
                {
                    simulator.KeyDown(keycode);
                }
                else
                {
                    if (oldState != newState)
                    {
                        simulator.KeyUp(keycode);
                    }
                }
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
            ProcessButtonState(PreviousSnapshot.Fret1, e.Snapshot.Fret1, KeyMap[nameof(GuitarButtonsState.Fret1)]);
            ProcessButtonState(PreviousSnapshot.Fret2, e.Snapshot.Fret2, KeyMap[nameof(GuitarButtonsState.Fret2)]);
            ProcessButtonState(PreviousSnapshot.Fret3, e.Snapshot.Fret3, KeyMap[nameof(GuitarButtonsState.Fret3)]);
            ProcessButtonState(PreviousSnapshot.Fret4, e.Snapshot.Fret4, KeyMap[nameof(GuitarButtonsState.Fret4)]);
            ProcessButtonState(PreviousSnapshot.Fret5, e.Snapshot.Fret5, KeyMap[nameof(GuitarButtonsState.Fret5)]);
            ProcessButtonState(PreviousSnapshot.Fret6, e.Snapshot.Fret6, KeyMap[nameof(GuitarButtonsState.Fret6)]);
            ProcessButtonState(PreviousSnapshot.Pause, e.Snapshot.Pause, KeyMap[nameof(GuitarButtonsState.Pause)]);
            ProcessButtonState(PreviousSnapshot.LiveTV, e.Snapshot.LiveTV, KeyMap[nameof(GuitarButtonsState.LiveTV)]);
            ProcessButtonState(PreviousSnapshot.HeroPower, e.Snapshot.HeroPower, KeyMap[nameof(GuitarButtonsState.HeroPower)]);
            ProcessButtonState(PreviousSnapshot.Power, e.Snapshot.Power, KeyMap[nameof(GuitarButtonsState.Power)]);

            switch (e.Snapshot.StrumBar)
            {
                case StrumBarState.StrumBarNeutral:
                    if (PreviousSnapshot.StrumBar == StrumBarState.StrumBarUp)
                    {
                        ProcessButtonState(ButtonState.Pressed, ButtonState.NotPressed, DIKKeyCode.DIK_8);
                    }

                    if (PreviousSnapshot.StrumBar == StrumBarState.StrumBarDown)
                    {
                        ProcessButtonState(ButtonState.Pressed, ButtonState.NotPressed, DIKKeyCode.DIK_9);
                    }
                    break;
                case StrumBarState.StrumBarUp:
                    ProcessButtonState(ButtonState.NotPressed, ButtonState.Pressed, DIKKeyCode.DIK_8);
                    break;
                case StrumBarState.StrumBarDown:
                    ProcessButtonState(ButtonState.NotPressed, ButtonState.Pressed, DIKKeyCode.DIK_9);
                    break;
                default:
                    break;
            }

            //switch (e.Snapshot.Navigator)
            //{
            //    case 0:
            //        simulator.KeyDown(DIKKeyCode.DIK_W);
            //        break;
            //    case 1:
            //        simulator.KeyDown(DIKKeyCode.DIK_W);
            //        break;
            //    case 2:
            //        simulator.KeyDown(DIKKeyCode.DIK_A);
            //        break;
            //    case 3:
            //        simulator.KeyDown(DIKKeyCode.DIK_A);
            //        break;
            //    case 4:
            //        simulator.KeyDown(DIKKeyCode.DIK_S);
            //        break;
            //    case 5:
            //        simulator.KeyDown(DIKKeyCode.DIK_S);
            //        break;
            //    case 6:
            //        simulator.KeyDown(DIKKeyCode.DIK_D);
            //        break;
            //    case 7:
            //        simulator.KeyDown(DIKKeyCode.DIK_D);
            //        break;
            //    case 15:
            //    default:
            //        break;
            //}

            //e.Snapshot.WhammyBar

            PreviousSnapshot = e.Snapshot;
        }
    }
}
