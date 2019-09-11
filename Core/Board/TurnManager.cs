using Kurenaiz.Utilities.Events;
using Seega.Enums;
using UnityEngine;
using Zenject;

public class TurnManager : MonoBehaviour, ITurnManager
{
    int _turnIndex;
    bool _isWhiteTurn;
    
    private EventManager _eventManager;

    [Inject]
    private void Construct(EventManager eventManager)
    {
        _eventManager = eventManager;
    }

    public bool IsWhiteTurn()
    {
        return _isWhiteTurn;
    }

    public void NextTurn()
    {
        _turnIndex++;
        _isWhiteTurn = !_isWhiteTurn;
        if (_eventManager.OnTurnChange != null)
            _eventManager.OnTurnChange (_isWhiteTurn);
    }

    public ColorType GetCurrentTurn()
    {
        return _isWhiteTurn ? ColorType.WHITE : ColorType.BLACK;
    }

    public void Start(ColorType starter)
    {
        _isWhiteTurn = starter == ColorType.BLACK ? false : true;
    }
}