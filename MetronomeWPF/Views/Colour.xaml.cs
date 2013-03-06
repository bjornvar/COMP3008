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
    /// Interaction logic for Colour.xaml
    /// </summary>
    public partial class ColourSelector : Page
    {
        public Color On { get; private set; }
        public Color Off { get; private set; }
        public Color Emphasized { get; private set; }

        public Frame Frame { get; private set; }

        public ColourSelector(Frame frame)
        {
            Frame = frame;
            InitializeComponent();
        }

        public void Show()
        {
            Frame.Content = this;
            this.Visibility = System.Windows.Visibility.Visible;
            Frame.Visibility = System.Windows.Visibility.Visible;
        }

        private void ColourSelected(object sender, EventArgs e)
        {
            RadioButton r = sender as RadioButton;
            Color c = (Color)ColorConverter.ConvertFromString(r.Tag.ToString());
            if (r.Tag.ToString().Equals("Gray"))
            {
                c = Color.FromRgb(128, 128, 128);
            }

            switch (r.GroupName)
            {
                case "On":
                    On = c;
                    break;
                case "Off":
                    Off = c;
                    break;
                case "Emphasized":
                    Emphasized = c;
                    break;
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
            Frame.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
