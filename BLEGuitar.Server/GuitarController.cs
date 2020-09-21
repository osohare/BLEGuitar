using BLEGuitar.Commons;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BLEGuitar.Server.DXHelper;

namespace BLEGuitar.Server
{
    public class GuitarController
    {
        private readonly DXHelper simulator = new DXHelper();
        private readonly ConcurrentQueue<GuitarButtonsState> queue = new ConcurrentQueue<GuitarButtonsState>();
        private Dictionary<string, DIKKeyCode> KeyMap;

        public void LoadMaps()
        {
            KeyMap = new Dictionary<string, DIKKeyCode>();

            foreach (var property in typeof(GuitarButtonsState).GetProperties())
            {
                string keyValue = ConfigurationManager.AppSettings[property.Name];
                if (Enum.TryParse(keyValue, true, out DIKKeyCode keyMap))
                {
                    KeyMap.Add(property.Name, keyMap);
                }
            }
        }

        private Tuple<KeyFlag, short> ProcessButtonState(ButtonState state, DIKKeyCode keycode)
        {
            return Tuple.Create(state == ButtonState.Pressed ? KeyFlag.KeyDown : KeyFlag.KeyUp, (short)keycode);
        }

        public void Guitar_DataReceived(object sender, DataReceivedArgs e)
        {
            var inputs = GetInputs(e.Snapshot);
            simulator.SendInput(inputs);
            simulator.SendMouseInput(e.Snapshot.WhammyBar, e.Snapshot.WhammyBar);
        }

        private List<Tuple<KeyFlag, short>> GetInputs(GuitarButtonsState e)
        {
            List<Tuple<KeyFlag, short>> inputs = new List<Tuple<KeyFlag, short>>()
            {
                ProcessButtonState(e.Fret1, KeyMap[nameof(GuitarButtonsState.Fret1)]),
                ProcessButtonState(e.Fret2, KeyMap[nameof(GuitarButtonsState.Fret2)]),
                ProcessButtonState(e.Fret3, KeyMap[nameof(GuitarButtonsState.Fret3)]),
                ProcessButtonState(e.Fret4, KeyMap[nameof(GuitarButtonsState.Fret4)]),
                ProcessButtonState(e.Fret5, KeyMap[nameof(GuitarButtonsState.Fret5)]),
                ProcessButtonState(e.Fret6, KeyMap[nameof(GuitarButtonsState.Fret6)]),
                ProcessButtonState(e.Pause, KeyMap[nameof(GuitarButtonsState.Pause)]),
                ProcessButtonState(e.LiveTV, KeyMap[nameof(GuitarButtonsState.LiveTV)]),
                ProcessButtonState(e.HeroPower, KeyMap[nameof(GuitarButtonsState.HeroPower)]),
                ProcessButtonState(e.Power, KeyMap[nameof(GuitarButtonsState.Power)])
            };

            switch (e.StrumBar)
            {
                case StrumBarState.StrumBarNeutral:
                    inputs.Add(ProcessButtonState(ButtonState.NotPressed, DIKKeyCode.DIK_8));
                    inputs.Add(ProcessButtonState(ButtonState.NotPressed, DIKKeyCode.DIK_9));
                    break;
                case StrumBarState.StrumBarUp:
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_8));
                    inputs.Add(ProcessButtonState(ButtonState.NotPressed, DIKKeyCode.DIK_9));
                    break;
                case StrumBarState.StrumBarDown:
                    inputs.Add(ProcessButtonState(ButtonState.NotPressed, DIKKeyCode.DIK_8));
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_9));
                    break;
                default:
                    break;
            }

            switch (e.Navigator)
            {
                case 0:
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_W));
                    break;
                case 1:
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_W));
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_A));
                    break;
                case 2:
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_A));
                    break;
                case 3:
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_A));
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_S));
                    break;
                case 4:
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_S));
                    break;
                case 5:
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_S));
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_D));
                    break;
                case 6:
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_D));
                    break;
                case 7:
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_D));
                    inputs.Add(ProcessButtonState(ButtonState.Pressed, DIKKeyCode.DIK_W));
                    break;
                case 15:
                default:
                    inputs.Add(ProcessButtonState(ButtonState.NotPressed, DIKKeyCode.DIK_W));
                    inputs.Add(ProcessButtonState(ButtonState.NotPressed, DIKKeyCode.DIK_A));
                    inputs.Add(ProcessButtonState(ButtonState.NotPressed, DIKKeyCode.DIK_S));
                    inputs.Add(ProcessButtonState(ButtonState.NotPressed, DIKKeyCode.DIK_D));
                    break;
            }

            return inputs;
        }
    }
}
