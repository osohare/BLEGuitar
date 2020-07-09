using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLEGuitar.Commons
{
    public class EventDecoder
    {
        public GuitarButtonsState CurrentState { get; private set; }
        public GuitarButtonsState SetValues(byte[] values)
        {
            if (CurrentState == null) CurrentState = new GuitarButtonsState();

            /// values[0] = "buttons"; //00 - 02 04 08 01 10 20
            CurrentState.Fret1 = IsPressed(values[0], ButtonConstants.BUTTON1_PRESSED) ? ButtonState.Pressed : ButtonState.NotPressed;
            CurrentState.Fret2 = IsPressed(values[0], ButtonConstants.BUTTON2_PRESSED) ? ButtonState.Pressed : ButtonState.NotPressed;
            CurrentState.Fret3 = IsPressed(values[0], ButtonConstants.BUTTON3_PRESSED) ? ButtonState.Pressed : ButtonState.NotPressed;
            CurrentState.Fret4 = IsPressed(values[0], ButtonConstants.BUTTON4_PRESSED) ? ButtonState.Pressed : ButtonState.NotPressed;
            CurrentState.Fret5 = IsPressed(values[0], ButtonConstants.BUTTON5_PRESSED) ? ButtonState.Pressed : ButtonState.NotPressed;
            CurrentState.Fret6 = IsPressed(values[0], ButtonConstants.BUTTON6_PRESSED) ? ButtonState.Pressed : ButtonState.NotPressed;

            /// values[1] = "pause"; // 00 - 02 pause 04 live tv 08 hero power 10 power button
            CurrentState.Pause = IsPressed(values[1], ButtonConstants.BUTTON_PAUSE_PRESSED) ? ButtonState.Pressed : ButtonState.NotPressed;
            CurrentState.LiveTV = IsPressed(values[1], ButtonConstants.BUTTON_LIVETV_PRESSED) ? ButtonState.Pressed : ButtonState.NotPressed;
            CurrentState.HeroPower = IsPressed(values[1], ButtonConstants.BUTTON_HERO_PRESSED) ? ButtonState.Pressed : ButtonState.NotPressed;
            CurrentState.Power = IsPressed(values[1], ButtonConstants.BUTTON_POWER_PRESSED) ? ButtonState.Pressed : ButtonState.NotPressed;

            /// values[2] = "navigator"; //0f - 00 01 02 03 04 05 06 07
            CurrentState.Navigator = values[2];

            /// values[4] = "strumbar"; //80 - ff down 00 up
            switch (values[4])
            {
                case ButtonConstants.STRUM_BAR_DOWN:
                    CurrentState.StrumBar = StrumBarState.StrumBarDown;
                    break;
                case ButtonConstants.STRUM_BAR_UP:
                    CurrentState.StrumBar = StrumBarState.StrumBarUp;
                    break;
                case ButtonConstants.STRUM_BAR_NEUTRAL:
                    CurrentState.StrumBar = StrumBarState.StrumBarNeutral;
                    break;
                default:
                    break;
            }

            /// values[6] = "whammy bar"; //80 to FF
            CurrentState.WhammyBar = (short)(values[6] - ButtonConstants.WHAMMY_BAR_MIN);

            return CurrentState;
        }

        private bool IsPressed(byte value, byte constant)
        {
            return ((value & constant) != 0);
        }
    }
}
