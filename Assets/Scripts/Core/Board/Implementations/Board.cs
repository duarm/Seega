using Kurenaiz.Utilities.Events;
using Kurenaiz.Utilities.Physics;
using Seega.Enums;
using Seega.Types;
using UnityEngine;
using Zenject;

public class Board : MonoBehaviour
{
    private int _turnIndex = 0;

    private PhysicsCache _physicsCache;
    private EventManager _eventManager;
    private IRayProvider _rayProvider;
    private ISelector2D _selector;
    private IPieceProvider _pieceProvider;
    private IGameFinisher _gameFinisher;
    private IPhaseManager _phaseManager;
    private ITurnManager _turnManager;
    private IMovementValidator _movementValidator;

    [Inject]
    private void Construct (PhysicsCache physicsCache,
        EventManager eventManager,
        IRayProvider rayProvider,
        ISelector2D selector,
        IPieceProvider pieceProvider,
        IGameFinisher gameFinisher,
        IPhaseManager phaseManager,
        ITurnManager turnManager,
        IMovementValidator movementValidator)
    {
        _physicsCache = physicsCache;
        _eventManager = eventManager;
        _rayProvider = rayProvider;
        _selector = selector;
        _pieceProvider = pieceProvider;
        _gameFinisher = gameFinisher;
        _phaseManager = phaseManager;
        _turnManager = turnManager;
        _movementValidator = movementValidator;
    }

    //On click in the game
    public void OnMouseClick ()
    {
        //Create a ray from the camera to the mouse position
        var ray = _rayProvider.CreateRay ();

        //check if we hit some collider
        if (_selector.Check (ray))
        {
            // if so, we get the correspoding cached tile associated with that collider
            _physicsCache.TryGetTileField (_selector.GetSelection (), out TileField tile);
            if (_phaseManager.Phase == Phase.MOVEMENT)
            {
                //Two possibilities: the field we clicked can/not have a piece
                if (tile.Piece != null) // does have a piece
                {
                    // If its a piece from the current turn, we try to select
                    if (_movementValidator.CanSelect (tile.Piece.type, _turnManager.Turn))
                    {
                        // Three possibilities: 
                        // If the clicked tile is the current selected, we deselect
                        // If there's no selected tile, we select
                        // If the clicked tile isn't the current selected, we deselect then select the clicked one
                        if (tile == _movementValidator.SelectedField)
                            _movementValidator.Deselect ();
                        else if (_movementValidator.SelectedField == null)
                            _movementValidator.Select (tile);
                        else if (tile != _movementValidator.SelectedField)
                        {
                            _movementValidator.Deselect ();
                            _movementValidator.Select (tile);
                        }
                    }
                }
                else // does not have a piece
                {
                    //One possibility: if there's no piece in the tile clicked, and we already have a tile selected, we try to move
                    if (_movementValidator.SelectedField != null)
                    {
                        //MOVE PIECE
                        if (tile.IsSelected)
                        {
                            //If this tile is highlighted, we can move to this tile.
                            _movementValidator.SelectedField.MovePieceTo (tile);
                            UpdateBoard (tile, _movementValidator.SelectedField);
                            _movementValidator.Deselect ();
                            _turnManager.NextTurn ();
                        }
                    }
                }
            }
            else if (_phaseManager.Phase == Phase.POSITIONING)
            {
                // If there's no piece on the clicked tile , we can only move to an empty space
                if (tile.Piece == null)
                {
                    //No piece can be put in the middle in the positioning phase
                    if (tile.Coordinates.x == 2 && tile.Coordinates.y == 2)
                        return;

                    if (_turnManager.IsWhiteTurn ())
                        tile.SetPiece (_pieceProvider.GetNonPlacedWhitePiece ());
                    else
                        tile.SetPiece (_pieceProvider.GetNonPlacedBlackPiece ());

                    _turnIndex++;

                    // Each player places 2 pieces before the next turn
                    if (_turnIndex % 2 == 0)
                        _turnManager.NextTurn ();

                    //when all pieces are placed, we start he movement phase
                    if (_turnIndex == _pieceProvider.PieceCount)
                    {
                        _phaseManager.StartMovementPhase ();
                        _gameFinisher.VerifyWall ();
                    }
                }
            }
        }
    }

    public void UpdateBoard (TileField currentField, TileField lastField)
    {
        var movement = new Movement (currentField, lastField);

        _gameFinisher.VerifyWall (currentField, movement);
        _gameFinisher.VerifyCaptures (currentField, movement);
    }

    //Called by the Surrender Button
    public void Surrender ()
    {
        //who surrenders is who loses
        _gameFinisher.Surrender (_turnManager.Turn);
    }
}