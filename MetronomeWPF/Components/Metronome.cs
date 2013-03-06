using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace MetronomeWPF.Components
{
    public delegate void StopEventHandler(object sender, EventArgs e);

    /// <summary>
    ///     Metronome model with trigger.
    /// </summary>
    public class Metronome
    {
        // Metronome settings
        public int Tempo { get; private set; }
        public Beat[] beats { get; private set; }
        private TimeSignature TimeSignature;
        public int counts { get; set; }        //number of bars to count if the countin. 0 means that the feature is OFF
        private int counter = -1;

        // Control related
        private Timer trigger;
        public bool active { get; private set; }
        private int currentBeat;
        private Mutex currentBeatMutex;

        public event StopEventHandler stopped;

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

            counts = 0;

            // Initialize inactive trigger (infinite wait time)
            trigger = new Timer(
                (source) => 
                {
                    if (counter > 0 || counter == -1)
                    {
                        // Get beat number (thread-safe)
                        currentBeatMutex.WaitOne();
                        int beat = currentBeat++;
                        currentBeat %= beats.Length;
                        currentBeatMutex.ReleaseMutex();

                        // Get beat info and set sound
                        Beat b = beats[beat];
                        tick(b);
                    }

                    if (counter == 0)
                        StopMetronome(true);

                    if (counter > 0)
                        counter--;
                },
                null,
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan
            );
        }
        
        /// <summary>
        ///     Calling this function will make the metronome start from beat one next time it is
        ///     started. If the metronome is active, it will restart from the beat one.
        /// </summary>
        public void ResetMetronome()
        {
            currentBeat = 0;
        }

        /// <summary>
        ///     Created sound threads at given interval.
        /// </summary>
        public void StartMetronome()
        {
            active = true;

            counter = (counts > 0) ? (counts * 4) + 1 : -1;

            // Activate timer
            trigger.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(60000 / Tempo));
        }

        /// <summary>
        ///     Deactivates metronome trigger
        /// </summary>
        public void StopMetronome(bool sig = false)
        {
            active = false;
            trigger.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            //sig boolean is to state whether to send the stop signal (only used for count-in feature)
            if (sig) onStopped(EventArgs.Empty);
        }

        //Signal for stopping the metronome within the class
        public void onStopped(EventArgs e)
        {
            if (stopped != null)
                stopped(this, e);
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

        /// <summary>
        ///     Enables smooth live tempo change. This function should be called by the Tick
        ///     function if there is a change in tempo. The trigger will be reset and will
        ///     therefore send a new Tick.
        ///     This use of this function for a Tick should be mutually exclusive with the standard
        ///     handling of tick (sound etc.).
        /// </summary>
        /// <param name="tempo">
        ///     The tempo to change to.
        /// </param>
        public void ChangeTempoLive(int tempo)
        {
            // Step back one beat
            if (--currentBeat < 0)
            {
                currentBeat = beats.Length - 1;
            }
            ChangeTempo(tempo);
        }
    }
}
