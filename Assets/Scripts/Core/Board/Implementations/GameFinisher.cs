using Kurenaiz.Utilities.Events;
using Seega.Enums;
using Seega.Types;
using UnityEngine;
using Zenject;

public class GameFinisher : MonoBehaviour, IGameFinisher
{
    private EventManager _eventManager;
    private IWallVerifier _wallVerifier;
    private ICaptureVerifier _captureVerifier;
    private IPieceProvider _pieceProvider;

    private int _whiteKills = 0;
    private int _blackKills = 0;

    [Inject]
    private void Construct (EventManager eventManager,
        IWallVerifier wallVerifier,
        ICaptureVerifier captureVerifier,
        IPieceProvider pieceProvider)
    {
        _eventManager = eventManager;
        _wallVerifier = wallVerifier;
        _captureVerifier = captureVerifier;
        _pieceProvider = pieceProvider;
    }

    public void VerifyCaptures (TileField currentField, Movement movement)
    {
        if (currentField.Piece.type == ColorType.BLACK)
            _blackKills += _captureVerifier.VerifyCapture (currentField, movement);
        else
            _whiteKills += _captureVerifier.VerifyCapture (currentField, movement);

        if (_blackKills == (_pieceProvider.PieceCount / 2))
            _eventManager.OnGameEnd (ColorType.BLACK, "Total");
        else if (_whiteKills == (_pieceProvider.PieceCount / 2))
            _eventManager.OnGameEnd (ColorType.WHITE, "Total");
    }

    public void Surrender (ColorType winner)
    {
        _eventManager.OnGameEnd (winner, "Grande");
    }

    //checks if the current field formed a wall
    public void VerifyWall (TileField currentField, Movement movement)
    {
        if (_wallVerifier.HasWall (currentField, movement))
            _eventManager.OnGameEnd (currentField.Piece.type, "Pequena");
    }

    //searchs through the whole field for a wall
    public void VerifyWall ()
    {
        if (_wallVerifier.HasWallOnField (out ColorType? winner))
        {
            if (winner != null)
                _eventManager.OnGameEnd ((ColorType) winner, "Pequena");
        }
    }
}