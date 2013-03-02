using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Metronome
{
    using Components;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Metronome metronome;
        private Dictionary<BeatState, MediaElement> sounds;

        public MainPage()
        {
            this.InitializeComponent();
            this.metronome = new Metronome();

            /* Move? */
            sounds = new Dictionary<BeatState, MediaElement>();
            this.SetSound(snd_on, BeatState.On);
            this.SetSound(snd_emphasized, BeatState.Emphasized);

            

            this.InitializeView();
        }

        private void InitializeView()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            metronome.StartMetronome(Tick);
        }

        private async void Tick(Beat beat)
        {
            MediaElement sound = null;
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
                await sound.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, sound.Play);
            }
        }

        private void SetSound(MediaElement sound, BeatState beatType)
        {
            /* Remove if present */
            if (sounds.Keys.Contains(beatType))
            {
                sounds.Remove(beatType);
            }

            sounds.Add(beatType, sound);
        }
    }
}
