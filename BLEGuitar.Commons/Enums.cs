using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLEGuitar.Commons
{
    public enum ButtonState
    {
        NotPressed,
        Pressed
    }

    public enum StrumBarState
    {
        StrumBarNeutral,
        StrumBarUp,
        StrumBarDown       
    }

    public class GuitarButtonsState
    {
        /// <summary>
        /// values[0] = "buttons"; //00 - 02 04 08 01 10 20
        /// </summary>
        public ButtonState Fret1 { get; internal set; }
        public ButtonState Fret2 { get; internal set; }
        public ButtonState Fret3 { get; internal set; }
        public ButtonState Fret4 { get; internal set; }
        public ButtonState Fret5 { get; internal set; }
        public ButtonState Fret6 { get; internal set; }
        /// <summary>
        /// values[1] = "pause"; // 00 - 02 pause 04 live tv 08 hero power 10 power button
        /// </summary>
        public ButtonState Pause { get; internal set; }
        public ButtonState LiveTV { get; internal set; }
        public ButtonState HeroPower { get; internal set; }
        public ButtonState Power { get; internal set; }
        /// <summary>
        /// values[2] = "navigator"; //0f - 00 01 02 03 04 05 06 07
        /// </summary>
        public int Navigator { get; internal set; }
        /// <summary>
        /// values[4] = "strumbar"; //80 - ff down 00 up
        /// </summary>
        public StrumBarState StrumBar { get; internal set; }
        // values[5] = "vertical position"; //80 horizontal FF arm up 00 amr down

        /// <summary>
        /// values[6] = "whammy bar"; //80 to FF
        /// </summary>
        public short WhammyBar { get; internal set; }

        /// values[18] = ""; //DD default DC ???
        /// values[19] = ""; //00 arm down through FF arm up
    }
}
