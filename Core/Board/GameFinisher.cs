using Kurenaiz.Utilities.Events;
using Seega.Enums;
using UnityEngine;
using Zenject;

public class GameFinisher : MonoBehaviour, IGameFinisher
{
    private EventManager _eventManager;

    [Inject]
    private void Construct (EventManager eventManager)
    {
        _eventManager = eventManager;
    }

    public void TotalVictory (ColorType winner)
    {
        _eventManager.OnGameEnd (winner, "Total");
    }

    public void GreatVictory (ColorType winner)
    {
        _eventManager.OnGameEnd (winner, "Grande");
    }

    public void MinorVictory (ColorType winner)
    {
        _eventManager.OnGameEnd (winner, "Pequena");
    }
}