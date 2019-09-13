using Kurenaiz.Utilities.Events;
using Seega.Enums;
using UnityEngine;
using Zenject;

public class PhaseManager : MonoBehaviour, IPhaseManager
{
    Phase _currentPhase = Phase.STARTING;

    private EventManager _eventManager;
    private ITurnManager _turnManager;

    [Inject]
    private void Construct (ITurnManager turnManager, EventManager eventManager)
    {
        _eventManager = eventManager;
        _turnManager = turnManager;
    }

    public void StartPositioningPhaseAsBlack ()
    {
        _currentPhase = Phase.POSITIONING;
        _turnManager.Initialize (ColorType.BLACK);

        if (_eventManager.OnStateChange != null)
            _eventManager.OnStateChange (_currentPhase);
    }

    public void StartPositioningPhaseAsWhite ()
    {
        _currentPhase = Phase.POSITIONING;
        _turnManager.Initialize (ColorType.WHITE);

        if (_eventManager.OnStateChange != null)
            _eventManager.OnStateChange (_currentPhase);
    }

    public void StartMovementPhase ()
    {
        _currentPhase = Phase.MOVEMENT;

        if (_eventManager.OnStateChange != null)
            _eventManager.OnStateChange (_currentPhase);
    }

    public Phase GetCurrentPhase ()
    {
        return _currentPhase;
    }
}