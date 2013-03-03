using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace MetronomeWPF.Components
{
    /// <summary>
    ///     Metronome model with trigger.
    /// </summary>
    class Metronome
    {
        // Metronome settings
        public int Tempo { get; private set; }
        public Beat[] beats { get; private set; }
        private TimeSignature TimeSignature;

        // Control related
        private Timer trigger;
        private bool active;
        private int currentBeat;
        private Mutex currentBeatMutex;

        /// <summary>
        ///     Default constructor - creates a time signature 4,4 and tempo 120 bpm metronome.
        /// </summary>
        public Metronome(Action<Beat> tick)
            : this(tick, new TimeSignature(4, 4)) { }

        /// <summary>
        ///     Creates a metronome based on the given <paramref name="timeSignature"/> and <paramref name="tempo"/>.
        /// </summary>
        /// <param name="timeSignature">
        ///     The metronome's time signature
        /// </param>
        /// <param name="tempo">
        ///     The metronome's tempo
        /// </param>
        public Metronome(Action<Beat> tick, TimeSignature timeSignature, int tempo = 120)
        {
            this.ChangeTimeSignature(timeSignature);
            this.ChangeTempo(tempo);

            currentBeatMutex = new Mutex(false);

            // Initialize inactive trigger (infinite wait time)
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
                null,
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan
            );
        }

        /// <summary>
        ///     Created sound threads at given interval.
        /// </summary>
        /// <param name="tick">
        ///     The function to call for each tick
        /// </param>
        public void StartMetronome()
        {      
            currentBeat = 0;
            active = true;

            // Activate timer
            trigger.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(60000 / Tempo));
        }

        /// <summary>
        ///     Deactivates metronome trigger
        /// </summary>
        public void StopMetronome()
        {
            active = false;
            trigger.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        ///     Change TimeSignature to of the metronome.
        /// </summary>
        /// <param name="newTimeSignature">
        ///     The new TimeSignature
        /// </param>
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

        /// <summary>
        ///     Changes the BeatState of a given Beat.
        /// </summary>
        /// <param name="beat">
        ///     The index of the Beat to change
        /// </param>
        /// <param name="state">
        ///     The BeatState to change to.
        /// </param>
        public void ChangeBeatState(int beat, BeatState state)
        {
            beats[beat].BeatState = state;
        }

        /// <summary>
        ///     Changes the BeatState of a given Beat to the next BeatState (cycles).
        /// </summary>
        /// <param name="beat">
        ///     The index of the Beat to change
        /// </param>
        public void AdvanceBeatState(int beat)
        {
            Beat b = beats[beat];
            b.BeatState++;
            if ((int)b.BeatState == (Enum.GetValues(typeof(BeatState)).Length))
            {
                b.BeatState = (BeatState)0;
            }
        }

        /// <summary>
        ///     Changes the tempo of the metronome.
        /// </summary>
        /// <param name="tempo">
        ///     The new tempo of the metronome
        /// </param>
        public void ChangeTempo(int tempo)
        {
            if (tempo > 0)
            {
                Tempo = tempo;
            }
            else
            {
                throw new ArgumentOutOfRangeException("tempo", "Tempo cannot be negative");
            }

            if (active)
            {
                StartMetronome();
            }
        }
    }
}
