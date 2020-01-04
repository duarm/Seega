using Seega.Enums;

public interface IPhaseManager
{
    Phase Phase { get; }
    void StartPositioningPhaseAsBlack ();
    void StartPositioningPhaseAsWhite ();
    void StartMovementPhase ();
}