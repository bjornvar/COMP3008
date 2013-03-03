using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetronomeWPF
{
    using Components;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Metronome metronome;
        private Dictionary<BeatState, SoundPlayer> sounds;

        public MainWindow()
        {
            InitializeComponent();
            this.metronome = new Metronome();

            /* Move? */
            sounds = new Dictionary<BeatState, SoundPlayer>();
            this.SetSound(new SoundPlayer("Assets/click.wav"), BeatState.On);
            this.SetSound(new SoundPlayer("Assets/click.wav"), BeatState.Emphasized);
            
            this.InitializeView();
        }

        private void InitializeView()
        {
            SetLights();
        }

        private void btn_start_Checked(object sender, RoutedEventArgs e)
        {
            metronome.StartMetronome(Tick);
            (sender as ToggleButton).Content = "STOP";
        }

        private void btn_start_Unchecked(object sender, RoutedEventArgs e)
        {
            metronome.StopMetronome();
            (sender as ToggleButton).Content = "START";
        }

        private void Tick(Beat beat)
        {
            SoundPlayer sound = null;
            switch (beat.BeatState)
            {
                case BeatState.Emphasized:
                    sounds.TryGetValue(BeatState.Emphasized, out sound);
                    break;
                case BeatState.On:
                    sounds.TryGetValue(BeatState.On, out sound);
                    break;
                case BeatState.Off:
                    sound = null;
                    break;
            }
            if (null != sound)
            {
                sound.Play();
                this.Dispatcher.InvokeAsync(() => 
                {
                    // Change light
                    (stc_lights.Children[beat.BeatNumber] as Ellipse).Style = (Style)FindResource("CurrentLight");

                    // Change back last 
                    int last = beat.BeatNumber - 1;
                    Console.WriteLine(beat.BeatNumber);
                    if (last < 0)
                    {
                        last = stc_lights.Children.Count - 1;
                    }
                    (stc_lights.Children[last] as Ellipse).Style = GetBeatStyle(metronome.beats.ElementAt(last).BeatState);
                });
            }
        }

        /**
         *  <summary>
         *      Set the sound associated with a certain BeatState.
         *  </summary>
         */
        private void SetSound(SoundPlayer sound, BeatState beatType)
        {
            /* Remove if present */
            if (sounds.Keys.Contains(beatType))
            {
                sounds.Remove(beatType);
            }

            sounds.Add(beatType, sound);
        }

        /**
         *  <summary>
         *      Creates lights based on beats per bar.
         *  </summary>
         */
        private void SetLights()
        {
            stc_lights.Children.Clear();
            foreach (Beat b in metronome.beats)
            {
                Ellipse e = new Ellipse();
                e.Style = GetBeatStyle(b.BeatState);
                stc_lights.Children.Add(e);
            }
        }

        private Style GetBeatStyle(BeatState b)
        {
            switch (b)
            {
                case BeatState.Off:
                    return (Style)FindResource("OffLight");
                case BeatState.Emphasized:
                    return (Style)FindResource("EmphasizedLight");
                case BeatState.On:
                default:
                    return (Style)FindResource("OnLight");
            }
        }

        /**
         *  <summary>
         *      Resizes components based on window sizes.
         *  </summary>
         */
        private void LayoutChanged(object sender, SizeChangedEventArgs e)
        {
            int newHeight;
            int newWidth;

            newHeight = (int)stc_lights.ActualHeight;
            newWidth = (int)((this.ActualWidth - 110) / stc_lights.Children.Count);

            foreach (Ellipse ellipse in stc_lights.Children)
            {
                ellipse.Height = Math.Min(newHeight, newWidth) - 10;
                ellipse.Width = Math.Min(newHeight, newWidth) - 10;
            }
        }

        /**
         *  <summary>
         *      Changes system volume for this application based on the GUI slider.
         *  </summary>
         */
        private void sld_volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            uint left = (uint) sld_volume.Value << 16;
            uint right = (uint) sld_volume.Value;
            uint total = left + right;

            NativeMethods.WaveOutSetVolume(IntPtr.Zero, total);
        }
    }

    /**
     *  <summary>
     *      Volume Control
     *  </summary>
     */
    static class NativeMethods
    {
        [DllImport("winmm.dll", EntryPoint = "waveOutSetVolume")]
        public static extern int WaveOutSetVolume(IntPtr hwo, uint dwVolume);
    }
}
