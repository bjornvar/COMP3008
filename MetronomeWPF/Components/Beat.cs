namespace MetronomeWPF.Components
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
