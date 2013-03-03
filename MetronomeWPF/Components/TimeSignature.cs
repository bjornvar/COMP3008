namespace MetronomeWPF.Components
{
    class TimeSignature
    {
        public int BeatsPerBar { get; private set; }
        public int BeatLength { get; private set; }

        public TimeSignature(int beatsPerBar, int beatLength)
        {
            BeatsPerBar = beatsPerBar;
            BeatLength = beatLength;
        }
    }
}
