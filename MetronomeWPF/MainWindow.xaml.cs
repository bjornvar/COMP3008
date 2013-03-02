using System;
using System.Collections.Generic;
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

        private List<Ellipse> lights;

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
            }
        }

        private void AdvanceTick()
        {
        }

        private void SetSound(SoundPlayer sound, BeatState beatType)
        {
            /* Remove if present */
            if (sounds.Keys.Contains(beatType))
            {
                sounds.Remove(beatType);
            }

            sounds.Add(beatType, sound);
        }

        private void SetLights()
        {
            stc_lights.Children.Clear();
            foreach (Beat b in metronome.beats)
            {
                Ellipse e = new Ellipse();
                switch (b.BeatState)
                {
                    case BeatState.Off:
                        e.Style = (Style)FindResource("OffLight");
                        break;
                    case BeatState.Emphasized:
                        e.Style = (Style)FindResource("EmphasizedLight");
                        break;
                    case BeatState.On:
                    default:
                        e.Style = (Style)FindResource("OnLight");
                        break;
                }
                stc_lights.Children.Add(e);
            }
        }
    }
}
