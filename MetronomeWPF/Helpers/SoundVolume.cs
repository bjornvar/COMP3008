using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MetronomeWPF.Helpers
{
    public static class SoundVolume
    {
        private static uint left;       // Left Volume
        private static uint right;      // Right Volume
        private static uint total;      // Total Volume

        private static bool rightMuted = false; // True if the right volume is muted
        private static bool leftMuted = false;  // True if the left volume is muted

        [DllImport("winmm.dll", EntryPoint = "waveOutSetVolume")]
        public static extern int WaveOutSetVolume(IntPtr hwo, uint dwVolume);

        // Public functions
        public static void SetVolume(int value)
        {
            SetLeft(value);
            SetRight(value);

            WaveOutSetVolume(IntPtr.Zero, total);
        }

        public static void Mute()
        {
            WaveOutSetVolume(IntPtr.Zero, 0);
        }

        public static int Unmute()
        {
            WaveOutSetVolume(IntPtr.Zero, total);
            return (int)total;
        }

        public static int MuteRight()
        {
            uint temp = right;

            if (rightMuted == false)
            {
                SetRight(0);
                WaveOutSetVolume(IntPtr.Zero, total);

                rightMuted = true;
                SetRight((int)temp);                
                return (int)total;
            }
            else
                return 0;
        }

        public static int UnmuteRight()
        {
            if (rightMuted == true && leftMuted == true)
            {
                uint temp = left;
                uint value;

                SetLeft(0);
                WaveOutSetVolume(IntPtr.Zero, total);
                return (int)total;
            }
            return 0;
        }

        public static int MuteLeft()
        {
            uint temp = left;

            if (leftMuted == false)
            {
                SetLeft(0);
                WaveOutSetVolume(IntPtr.Zero, total);

                leftMuted = true;
                SetLeft((int)temp);
                return (int)total;
            }
            else
                return 0;
        }

        public static int UnmuteLeft()
        {

            return 0;
        }

        //public static 
        // Private Functions setting the left and right volume
        private static void SetRight(int value)
        {
            right = (uint)value;
            total = left + right;
        }

        private static void SetLeft(int value)
        {
            left = (uint)value << 16;
            total = left + right;
        }
    }
}
