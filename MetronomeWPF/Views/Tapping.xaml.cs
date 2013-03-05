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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetronomeWPF.Views
{
    /// <summary>
    /// Interaction logic for Tapping.xaml
    /// </summary>
    public partial class Tapping : Page
    {
        private Frame f = null;
        System.DateTime time;
        int bpm = 0;
        double[] beats = null;
        int index = 0;
        int beatNum = 0;
        DateTime recent;

        public Tapping(Frame frame)
        {
            InitializeComponent();
            f = frame;
            time = new DateTime(DateTime.MaxValue.Ticks);
            beatNum = 4;
            beats = new double[beatNum];
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            f.Visibility = System.Windows.Visibility.Hidden;
              //  (Parent as UIElement).Visibility = System.Windows.Visibility.Hidden;
            //Visibility = System.Windows.Visibility.Hidden;
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
            
            int sec = temp.Second;
            double mil = (double)temp.Millisecond / 1000;

            beats[index % beatNum] = (sec + mil) % 60;

            
            
            bpm = calculateBPM(index % beatNum);

            index++;     

            MessageBox.Show(""+bpm);
            (sender as Rectangle).Opacity = 0.8;
        }

        private void Rectangle_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            (sender as Rectangle).Opacity = 0.0;
        }

        private int calculateBPM(int startIn)
        {
            double total = 0;
            double cur = beats[startIn];
            int taps = 0;
            for (int x = 1; x < beats.Length; x++)
            {
                int ind = startIn - x;
                if (ind < 0) ind = beatNum + ind;
                double prev = beats[ind];
                if(prev > 0 && cur > 0)
                {
                    total += cur - prev;
                    taps++;
                }
                cur = prev;
            }
            if (taps == 0)
                return 0;
            else
                return Convert.ToInt32(total / taps);
        }
    }
}
