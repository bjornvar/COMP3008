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
    using Views;
    using Helpers;

    /// <summary>
    ///     View / Controller
    /// </summary>
    public partial class MainWindow : Window
    {
        private Metronome metronome;
        private Dictionary<BeatState, SoundPlayer> sounds;

        private int TempoChangeIntent = 0;

        public MainWindow()
        {
            InitializeComponent();
            this.metronome = new Metronome(Tick);

            // Set sounds. Move code?
            sounds = new Dictionary<BeatState, SoundPlayer>();
            this.SetSound(new SoundPlayer("Assets/click.wav"), BeatState.On);
            this.SetSound(new SoundPlayer("Assets/cow-bell.wav"), BeatState.Emphasized);
            
            this.InitializeView();
        }

        /// <summary>
        ///     Model dependent initialization.
        /// </summary>
        private void InitializeView()
        {
            SetLights();
        }

        /// <summary>
        ///     Performs actions involved in a metronome tick.
        ///     <list type="bullet">
        ///         <item>Play sound</item>
        ///         <item>Change lights</item>
        ///     </list>
        /// </summary>
        /// <param name="beat">
        ///     The beat the tick is associated with
        /// </param>
        private void Tick(Beat beat)
        {
            if (TempoChangeIntent > 0)
            {
                metronome.ChangeTempoLive(TempoChangeIntent);
                TempoChangeIntent = 0;
            }
            else
            {
                SoundPlayer sound = this.GetBeatSound(beat);

                if (null != sound)
                {
                    sound.Play();
                }

                // Change lights (async)
                this.Dispatcher.BeginInvoke(new Action<Beat>(AdvanceLights), new object[] { beat });
            }
        }

        /// <summary>
        ///     Changes the Ellipse associtated with the <paramref name="beat"/> to be the current 
        ///     beat. Reverts the Ellipse associated with the previous beat (index - 1) to default.
        /// </summary>
        /// <param name="beat">
        ///     The current Beat (to be changed)
        /// </param>
        private void AdvanceLights(Beat beat)
        {
            // Change light
            (stc_lights.Children[beat.BeatNumber] as Ellipse).Style = (Style)FindResource("CurrentLight");

            // Change previous light back to its default Style
            int last = beat.BeatNumber - 1;
            if (last < 0)
            {
                last = stc_lights.Children.Count - 1;
            }
            (stc_lights.Children[last] as Ellipse).Style = GetBeatStyle(metronome.beats.ElementAt(last).BeatState);
        }

        /// <summary>
        ///     Set the sound associated with a certain BeatState.
        /// </summary>
        /// <param name="sound">
        ///     The SoundPlayer sound to be played
        /// </param>
        /// <param name="beatType">
        ///     The BeatState to play this sound for
        /// </param>
        private void SetSound(SoundPlayer sound, BeatState beatType)
        {
            // Remove if present
            if (sounds.Keys.Contains(beatType))
            {
                sounds.Remove(beatType);
            }

            sounds.Add(beatType, sound);
        }

        /// <summary>
        ///     Creates lights based on beats per bar.
        /// </summary>
        ///
        private void SetLights()
        {
            stc_lights.Children.Clear();
            foreach (Beat b in metronome.beats)
            {
                Ellipse e = new Ellipse();
                e.MouseLeftButtonUp += Light_Click;
                e.Style = GetBeatStyle(b.BeatState);
                stc_lights.Children.Add(e);
            }
        }

        /// <summary>
        ///     Resizes lights based on available space and number of lights.
        /// </summary>
        /// 
        private void ResizeLights()
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

        /// <summary>
        ///     Gets the Ellipse Style associated with a <typeparamref name="BeatState"/>
        /// </summary>
        /// <param name="b">
        ///     The BeatState to get Style for
        /// </param>
        /// <returns>
        ///     The Ellipse Style associated with the BeatState
        /// </returns>
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

        /// <summary>
        ///     Gets the Ellipse Style associated with a <typeparamref name="BeatState"/>
        /// </summary>
        /// <param name="b">
        ///     The BeatState to get Style for
        /// </param>
        /// <returns>
        ///     The Ellipse Style associated with the BeatState
        /// </returns>
        private SoundPlayer GetBeatSound(Beat b)
        {
            SoundPlayer sound = null;
            switch (b.BeatState)
            {
                case BeatState.Emphasized:
                    sounds.TryGetValue(BeatState.Emphasized, out sound);
                    break;
                case BeatState.On:
                    sounds.TryGetValue(BeatState.On, out sound);
                    break;
                case BeatState.Off:
                default:
                    sound = null;
                    break;
            }
            return sound;
        }

        private void btn_start_Checked(object sender, RoutedEventArgs e)
        {
            metronome.StartMetronome();
            (sender as ToggleButton).Content = "STOP";
            this.SetLights();
        }

        private void btn_start_Unchecked(object sender, RoutedEventArgs e)
        {
            metronome.StopMetronome();
            metronome.ResetMetronome();
            (sender as ToggleButton).Content = "START";
            this.SetLights();
        }

        // Settings Page
        // Need to finish
        private void btn_settings_Click(object sender, RoutedEventArgs e)
        {
            //frm_tapping.Content = new Settings();
            //frm_tapping.Visibility = System.Windows.Visibility.Visible;
        }

        // Help Page
        // Need to finish
        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            frm_tapping.Content = new Help();           
            frm_tapping.Visibility = System.Windows.Visibility.Visible;
        }


        private void btn_tapping_Click(object sender, RoutedEventArgs e)
        {
            frm_tapping.Content = new Tapping(frm_tapping);
            frm_tapping.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        ///     Resizes components based on window sizes.
        /// </summary>
        ///
        private void LayoutChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeLights();
        }

        /// <summary>
        ///     Changes system volume for this application based on the GUI slider.
        /// </summary>
        ///
        private void sld_volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            uint left = (uint) sld_volume.Value << 16; // Unsigned int with 16 MST bit being left and 16 LSB right
            uint right = (uint) sld_volume.Value;
            uint total = left + right;

            //uint left = (uint) sld_volume.Value << 16;
           // uint right = (uint) sld_volume.Value;
           // uint total = left + right;

           // SoundVolume.WaveOutSetVolume(IntPtr.Zero, total);
        }
        /// <summary>
        ///     Toggles system volume to on or off. 
        /// </summary>
        ///
        private void toggleMute(object sender, RoutedEventArgs e)
        {
            if (btn_mute.Content == FindResource("Mute"))
            {
                SoundVolume.WaveOutSetVolume(IntPtr.Zero, 0);
                btn_mute.Content = FindResource("Sound");
            }
            else
            {
                uint left = (uint)sld_volume.Value << 16; // Unsigned int with 16 MST bit being left and 16 LSB right
                uint right = (uint)sld_volume.Value;
                uint total = left + right;

                SoundVolume.WaveOutSetVolume(IntPtr.Zero, total);
                btn_mute.Content = FindResource("Mute");
            }
            
        }
        /// <summary>
        ///     Handles Light click. Advances Light to next BeatState.
        /// </summary>
        /// <param name="sender">
        ///     The Ellipse that was clicked
        /// </param>
        private void Light_Click(object sender, EventArgs e)
        {
            int beat = stc_lights.Children.IndexOf(sender as Ellipse);
            metronome.AdvanceBeatState(beat);
            SetLights();
        }

        private void txt_tempo_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int tempo = Int32.Parse((sender as TextBox).Text);
                //metronome.ChangeTempo(tempo);
                TempoChangeIntent = tempo;
            }
            catch (Exception) { }
        }

        private void sld_tempo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Determine whether to start the metronome again on mouseup
            /*
            bool active = metronome.active;
            (sender as Slider).PreviewMouseLeftButtonUp += (s, events) => 
            {
                if (active)
                {
                    metronome.StartMetronome();
                }
            };

            // Stop the metronome
            metronome.StopMetronome();
            SetLights();
            */
        }

        /// <summary>
        ///     Handles all tempo increment and decrement buttons.
        /// </summary>
        /// <param name="sender">
        ///     Inc/dec Button with Tag indicating value
        /// </param>
        private void TempoButton_Click(object sender, EventArgs e)
        {
            try
            {
                Button b = (sender as Button);
                int tempo = Int32.Parse(txt_tempo.Text);
                int change = Int32.Parse((string)b.Tag);
                txt_tempo.Text = (tempo + change) + "";

                txt_tempo.Focus();
                b.Focus();
            }
            catch (Exception) { }
        }
    }
}
