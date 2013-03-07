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
    using Components;
    using System.Media;
    using Views;
   
    /// <summary>
    /// Interaction logic for Sound_Settings.xaml
    /// </summary>
    public partial class Sound_Settings : Page
    {
        
        private MainWindow window = null;
        
        public string selectedL, selectedR;
        public Frame Frame { get; private set; }

        public Sound_Settings(Frame frame, MainWindow w)
        {
            Frame = frame;
            window = w; 
            InitializeComponent();
            
            //selectedL = window.selectedOnSound;
            //selectedR = window.selectedEmphasizedSound;
            
            
        }

        public void Show()
        {
            Frame.Content = this;
            this.Visibility = System.Windows.Visibility.Visible;
            Frame.Visibility = System.Windows.Visibility.Visible;
        }

        private void initializeHighlights()
        {
            Button senderButton = null;  

            highlight_rectangleL.Margin = new Thickness(highlight_rectangleL.Margin.Left, senderButton.Margin.Top,
                                                        highlight_rectangleL.Margin.Right, senderButton.Margin.Bottom);
            selectedL = senderButton.Uid;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
            Frame.Visibility = System.Windows.Visibility.Hidden;  
        }

        private void btnClickL(object sender, RoutedEventArgs e)
        {
            Button senderButton = (sender as Button);

            highlight_rectangleL.Margin = new Thickness(highlight_rectangleL.Margin.Left, senderButton.Margin.Top,
                                                        highlight_rectangleL.Margin.Right, senderButton.Margin.Bottom);
            highlight_rectangleL.Visibility = System.Windows.Visibility.Visible;
            selectedL = senderButton.Uid;
            window.setSound(selectedL, BeatState.On);
            window.playsound(BeatState.On);
            
        }

        private void btnClickR(object sender, RoutedEventArgs e)
        {
            Button senderButton = (sender as Button);

            highlight_rectangleR.Margin = new Thickness(highlight_rectangleR.Margin.Left, senderButton.Margin.Top,
                                                        highlight_rectangleR.Margin.Right, senderButton.Margin.Bottom);
            highlight_rectangleR.Visibility = System.Windows.Visibility.Visible;
            selectedR = senderButton.Uid;
            window.setSound(selectedR, BeatState.Emphasized);
            window.playsound(BeatState.Emphasized);
        }

    }
}
