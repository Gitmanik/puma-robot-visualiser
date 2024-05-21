namespace Puma_Visualiser.Objects;

public class Timestamp
{
    public readonly float Time;
    public readonly int TrackNumber;
    public readonly float Value;

    public Timestamp(float time, int trackNumber, float value)
    {
        Time = time;
        TrackNumber = trackNumber;
        Value = value;
    }

    public override string ToString() => $"TS: {Time}, {TrackNumber} -> {Value}";
}