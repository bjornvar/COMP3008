using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace MetronomeWPF.Components
{
    class Metronome
    {
        public int Tempo { get; private set; }
        public Beat[] beats { get; private set; }
        private TimeSignature TimeSignature;

        Timer trigger;
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
            
            trigger = new Timer(
                (source) => 
                {
                    // Get beat number (thread-safe)
                    currentBeatMutex.WaitOne();
                    int beat = currentBeat++;
                    currentBeat %= beats.Length;
                    currentBeatMutex.ReleaseMutex();

                    // Get beat info and set sound
                    Beat b = beats[beat];
                    tick(b);
                },
                null,Timeout.InfiniteTimeSpan,Timeout.InfiniteTimeSpan);
            trigger.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(60000 / Tempo));
        }

        /**
         *  <summary>
         *      Stop sound thread creation.
         *  </summary>
         */
        public void StopMetronome()
        {
            trigger.Dispose();
        }

        public void ChangeTimeSignature(TimeSignature newTimeSignature)
        {
            this.TimeSignature = newTimeSignature;

            beats = new Beat[this.TimeSignature.BeatsPerBar];
            for (int i = 0; i < beats.Length; i++)
            {
                beats[i] = new Beat(i, BeatState.On);
            }
            beats[0].BeatState = BeatState.Emphasized;
        }

        public void ChangeBeatState(int beat, BeatState state)
        {
            beats[beat].BeatState = state;
        }
    }
}
