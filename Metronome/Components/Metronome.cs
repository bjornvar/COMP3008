using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.System.Threading;
using System.Threading;
using Windows.UI.Xaml.Controls;

namespace Metronome.Components
{
    class Metronome
    {
        public int Tempo { get; private set; }
        private Beat[] beats;
        private TimeSignature timeSignature;

        private ThreadPoolTimer trigger;
        private int currentBeat;
        private Mutex currentBeatMutex;

        public Metronome()
            : this(new TimeSignature(4, 4), 120) { }

        public Metronome(TimeSignature timeSignature, int tempo)
        {
            this.ChangeTimeSignature(timeSignature);
            if (tempo > 0)
            {
                Tempo = tempo;
            }
            else
            {
                throw new ArgumentOutOfRangeException("tempo", "Tempo cannot be negative");
            }
        }

        /**
         *  <summary>
         *      Created sound threads at given interval.
         *  </summary>
         */
        public void StartMetronome(Action<Beat> tick)
        {
            currentBeat = 0;
            currentBeatMutex = new Mutex(false);
            
            

            trigger = ThreadPoolTimer.CreatePeriodicTimer(
                (source) => 
                {
                    /* Get beat number (thread-safe) */
                    currentBeatMutex.WaitOne();
                    int beat = currentBeat++;
                    currentBeat %= beats.Length;
                    currentBeatMutex.ReleaseMutex();

                    /* Get beat info and set sound */
                    Beat b = beats[beat];
                    tick(b);
                }, 
                TimeSpan.FromMilliseconds(60000 / Tempo));
        }

        /**
         *  <summary>
         *      Stop sound thread creation.
         *  </summary>
         */
        public void StopMetronome()
        {
            trigger.Cancel();
        }

        public void ChangeTimeSignature(TimeSignature newTimeSignature)
        {
            this.timeSignature = newTimeSignature;

            beats = new Beat[this.timeSignature.TotalBeats];
            for (int i = 0; i < beats.Length; i++)
            {
                beats[i] = new Beat(BeatState.On);
            }
            beats[0].BeatState = BeatState.Emphasized;
        }
    }
}
