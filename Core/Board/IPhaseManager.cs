using Seega.Enums;

public interface IPhaseManager
{
    Phase GetCurrentPhase();
    void StartPositioningPhaseAsBlack();
    void StartPositioningPhaseAsWhite();
    void StartMovementPhase();
}