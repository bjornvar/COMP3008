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

        public static uint left;      // Left Volume
        public static uint right;     // Right Volume
        public static uint total      // Total Volume
        {
            get
            {
                return (leftMuted ? 0 : (left << 16)) + (rightMuted ? 0 : right);
            }
        }

        public static bool rightMuted = false;  // True if the right volume is muted
        public static bool leftMuted = false;   // True if the left volume is muted
        public static bool soundMuted = false;  // true if the sound is currently muted

        [DllImport("winmm.dll", EntryPoint = "waveOutSetVolume")]
        public static extern int WaveOutSetVolume(IntPtr hwo, uint dwVolume);

        // Public functions
        public static void SetVolume(int value)
        {
            left = (uint)value;
            right = (uint)value;

            WaveOutSetVolume(IntPtr.Zero, total);
            Console.WriteLine("Setting up the sound volume to: " + total);
        }

        public static void Mute()
        {
            soundMuted = true;
            WaveOutSetVolume(IntPtr.Zero, 0);
        }

        public static void Unmute()
        {
            soundMuted = false;
            WaveOutSetVolume(IntPtr.Zero, total);
        }

        public static void MuteRight()
        {
            rightMuted = true; 
            WaveOutSetVolume(IntPtr.Zero, total);
                         
        }

        public static void UnmuteRight()
        {
            rightMuted = false;
            WaveOutSetVolume(IntPtr.Zero, total);
        }

        public static void MuteLeft()
        {
            leftMuted = true;
            WaveOutSetVolume(IntPtr.Zero, total);
        }

        public static void UnmuteLeft()
        {
            leftMuted = false;
            WaveOutSetVolume(IntPtr.Zero, total);
        }
    }
}
