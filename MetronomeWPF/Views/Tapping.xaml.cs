using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetronomeWPF.Views
{
    using Components;

    /// <summary>
    /// Interaction logic for Tapping.xaml
    /// </summary>
    public partial class Tapping : Page
    {
        private Frame frame = null;
        private Metronome metro;
        private Slider tempoSlider;
        private SoundPlayer sound;

        System.DateTime time;
        int bpm = 0;
        double[] beats = null;
        int index = 0;
        int beatNum = 0;
        DateTime lastTap;

        public Tapping(Frame f, Metronome m, Slider t)
        {
            InitializeComponent();
            
            frame = f;
            metro = m;
            tempoSlider = t;

            time = new DateTime(DateTime.MaxValue.Ticks);
            sound = new SoundPlayer("Assets/cow-bell.wav");

            beatNum = 4;
            beats = new double[beatNum];
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            frame.Visibility = System.Windows.Visibility.Hidden;
              //  (Parent as UIElement).Visibility = System.Windows.Visibility.Hidden;
            //Visibility = System.Windows.Visibility.Hidden;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int newValue = (bpm > 240) ? 240 : bpm;
            newValue = (bpm < 20) ? 20 : bpm;

            metro.ChangeTempo(newValue);
            tempoSlider.Value = newValue;
            frame.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Page_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("TAP");
        }

        private void Rectangle_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            // Need to come back to this, but should try keeping a DateTime object to store the most recent, and subtract it with a new DateTime on every click,
            //refreshing the recent on every click.
            // Need to store the differences in time, not the times themselves, then calculate the bpm
            DateTime temp = DateTime.Now;

            if (index > 0)
            {
                TimeSpan diff = temp.Subtract(lastTap);
                double total = diff.TotalMilliseconds;
                if (total > 5000)
                {
                    beats = new double[beatNum];
                    index = 0;
                }
                else
                    beats[(index - 1) % beatNum] = total;
            }

            lastTap = temp;
            index++;

            bpm = calculateBPM();

            String printValue = (bpm > 240) ? "240+" : "" + bpm;

            tempoBox.Text = printValue;
            (sender as Rectangle).Opacity = 0.8;

            sound.Play();
        }

        private void Rectangle_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            (sender as Rectangle).Opacity = 0.0;
        }

        private int calculateBPM()
        {
            double total = 0;
            int taps = 0;
            for (int x = 0; x < beats.Length; x++)
            {
                if (beats[x] > 0)
                {
                    total += beats[x] / (1000 * 60);
                    taps++;
                }
            }
            if (taps == 0)
                return 0;
            else
                return Convert.ToInt32(taps / total);
        }
    }
}
