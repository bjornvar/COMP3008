using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metronome.Components
{
    class Beat
    {
        public BeatState BeatState { get; set; }

        public Beat(BeatState beatState)
        {
            BeatState = beatState;
        }
    }

    enum BeatState
    {
        On,
        Off,
        Emphasized
    }
}
