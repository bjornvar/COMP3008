﻿using System;
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
<<<<<<< HEAD


        public Tapping()
=======
        private Frame f = null;

        public Tapping(Frame frame)
>>>>>>> origin/Cowan
        {
            InitializeComponent();
            f = frame;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("No");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            f.Visibility = System.Windows.Visibility.Hidden;
              //  (Parent as UIElement).Visibility = System.Windows.Visibility.Hidden;
            //Visibility = System.Windows.Visibility.Hidden;
        }

        private void Page_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("TAP");
        }
    }
}
