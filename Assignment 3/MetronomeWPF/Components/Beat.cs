namespace MetronomeWPF.Components
{
    public class Beat
    {
        public BeatState BeatState { get; set; }
        public int BeatNumber { get; private set; }

        public Beat(int beatNumber, BeatState beatState = BeatState.On)
        {
            BeatNumber = beatNumber;
            BeatState = beatState;
        }
    }

    public enum BeatState
    {
        On,
        Off,
        Emphasized
    }
}
