using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLEGuitar.Commons
{
    /// <summary>
    /// values[0] = "buttons"; //00 - 02 04 08 01 10 20
    /// values[1] = "pause"; // 00 - 02 pause 04 live tv 08 hero power 10 power button
    /// values[2] = "navigator"; //0f - 00 01 02 03 04 05 06 07
    /// values[3] = "neutral"; //80
    /// values[4] = "strumbar"; //80 - ff down 00 up
    /// values[5] = "vertical position"; //80 horizontal FF arm up 00 amr down
    /// values[6] = "whammy bar"; //80 to FF
    /// values[7][8][9][10][11][12][13][14][15]=00 [16]=01 [17]=08
    /// values[18] = ""; //DD default DC ???
    /// values[19] = ""; //00 arm down through FF arm up
    /// </summary>
    public class ButtonConstants
    {
        public const byte BUTTON_NOT_PRESSED = 0x00;

        public const byte BUTTON1_PRESSED = 0x02;
        public const byte BUTTON2_PRESSED = 0x04;
        public const byte BUTTON3_PRESSED = 0x08;
        public const byte BUTTON4_PRESSED = 0x01;
        public const byte BUTTON5_PRESSED = 0x10;
        public const byte BUTTON6_PRESSED = 0x20;

        public const byte BUTTON_PAUSE_PRESSED = 0x02;
        public const byte BUTTON_LIVETV_PRESSED = 0x04;
        public const byte BUTTON_HERO_PRESSED = 0x08;
        public const byte BUTTON_POWER_PRESSED = 0x10;

        public const byte NAVIGATOR_NEUTRAL = 0x0F;
        public const byte NAVIGATOR_0 = 0x00;
        public const byte NAVIGATOR_1 = 0x01;
        public const byte NAVIGATOR_2 = 0x02;
        public const byte NAVIGATOR_3 = 0x03;
        public const byte NAVIGATOR_4 = 0x04;
        public const byte NAVIGATOR_5 = 0x05;
        public const byte NAVIGATOR_6 = 0x06;
        public const byte NAVIGATOR_7 = 0x07;

        public const byte STRUM_BAR_NEUTRAL = 0x80;
        public const byte STRUM_BAR_UP = 0x00;
        public const byte STRUM_BAR_DOWN = 0xFF;

        public const byte POSITION_HORIZONTAL = 0x80;
        public const byte POSITION_ARM_UP = 0xFF;
        public const byte POSITION_ARM_DOWN = 0x00;

        public const byte WHAMMY_BAR_MIN = 0x80;
        public const byte WHAMMY_BAR_MAX = 0xFF;

        public const byte ANGLE_ARM_MIN = 0x00;
        public const byte ANGLE_ARM_MAX = 0xFF;
    }
}
