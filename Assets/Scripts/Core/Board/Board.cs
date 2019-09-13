using Kurenaiz.Utilities.Events;
using Kurenaiz.Utilities.Physics;
using Kurenaiz.Utilities.Types;
using Seega.Enums;
using Seega.Types;
using UnityEngine;
using Zenject;

public class Board : MonoBehaviour
{
    private int _turnIndex = 0;

    TileField m_HighlightedTile; //Keep track of the current selected piece

    private PhysicsCache _physicsCache;
    private EventManager _eventManager;
    private IRayProvider _rayProvider;
    private ISelector _selector;
    private IFieldProvider _fieldProvider;
    private IPieceProvider _pieceProvider;
    private ICaptureVerifier _captureVerifier;
    private IGameFinisher _gameFinisher;
    private IPhaseManager _phaseManager;
    private ITurnManager _turnManager;
    private IWallVerifier _wallVerifier;

    [Inject]
    private void Construct (PhysicsCache physicsCache,
        EventManager eventManager,
        IRayProvider rayProvider,
        ISelector selector,
        IFieldProvider fieldProvider,
        IPieceProvider pieceProvider,
        ICaptureVerifier captureVerifier,
        IGameFinisher gameFinisher,
        IPhaseManager phaseManager,
        ITurnManager turnManager,
        IWallVerifier wallVerifier)
    {
        _physicsCache = physicsCache;
        _eventManager = eventManager;
        _rayProvider = rayProvider;
        _selector = selector;
        _fieldProvider = fieldProvider;
        _pieceProvider = pieceProvider;
        _captureVerifier = captureVerifier;
        _gameFinisher = gameFinisher;
        _phaseManager = phaseManager;
        _turnManager = turnManager;
        _wallVerifier = wallVerifier;
    }

    public void OnMouseClick ()
    {
        //CREATING THE RAY
        var ray = _rayProvider.CreateRay ();

        //SHOTING THE RAY
        //shooting a raycast to get the tile that the player clicked
        if (_selector.Check (ray))
        {
            TileField tile;
            _physicsCache.TryGetTileField (_selector.GetSelection (), out tile);
            if (_phaseManager.GetCurrentPhase () == Phase.MOVEMENT)
            {
                //Two possibilities: the field can or not have a piece
                if (tile.Piece != null)
                {
                    // One possibility: If its a piece from the current turn, we try to highlight
                    if (ValidateHighlight (tile.Piece))
                    {
                        // Three possibilities: 
                        // If the clicked tile is the current highlighted, we only dehighlight
                        // If the highlighted tile is null, we only highlight
                        // If the clicked tile isn't the current highlighted, we dehighlight and highlight the clicked one
                        if (tile == m_HighlightedTile)
                            Deselect ();
                        else if (m_HighlightedTile == null)
                            TrySelectEmptyAdjacents (tile);
                        else if (tile != m_HighlightedTile)
                        {
                            Deselect ();
                            TrySelectEmptyAdjacents (tile);
                        }
                    }
                }
                else
                {
                    //One possibilities: if there is not a piece on the field we clicked, and we already have a highlighted field, we try to move
                    if (m_HighlightedTile != null)
                    {
                        //MOVE PIECE
                        if (tile.IsHighlighting)
                        {
                            //If this tile is highlighted, we can move to this tile.
                            m_HighlightedTile.MovePieceTo (tile);
                            UpdateBoard (tile, m_HighlightedTile);
                            Deselect ();
                            _turnManager.NextTurn ();
                        }
                    }
                }
            }
            else if (_phaseManager.GetCurrentPhase () == Phase.POSITIONING)
            {
                //One Possibility: There's no piece on the field, we can only move to an empty space
                if (tile.Piece == null)
                {
                    //No piece can be put in the middle on the positioning phase
                    if (tile.Coordinates.x == 2 && tile.Coordinates.y == 2)
                        return;

                    if (_turnManager.IsWhiteTurn ())
                        tile.SetPiece (_pieceProvider.GetNonPlacedWhitePiece ());
                    else
                        tile.SetPiece (_pieceProvider.GetNonPlacedBlackPiece ());

                    _turnIndex++;

                    // Each player places 2 pieces before the next turn
                    if (_turnIndex % 2 == 0)
                    {
                        _turnManager.NextTurn ();
                    }

                    //when all pieces are placed
                    if (_turnIndex == 24)
                        _phaseManager.StartMovementPhase ();
                }
            }
        }
    }

    private void Deselect ()
    {
        TileField field;
        var coor = new Coordinates (m_HighlightedTile);
        var fields = _fieldProvider.GetField ();

        field = fields[coor.up];
        if (field?.IsHighlighting == true)
            field.OnDeselect ();

        field = fields[coor.right];
        if (field?.IsHighlighting == true)
            field.OnDeselect ();

        field = fields[coor.down];
        if (field?.IsHighlighting == true)
            field.OnDeselect ();

        field = fields[coor.left];
        if (field?.IsHighlighting == true)
            field.OnDeselect ();

        m_HighlightedTile = null;
    }

    private void TrySelectEmptyAdjacents (TileField tile)
    {
        TileField field;
        var coor = new Coordinates (tile);
        var fields = _fieldProvider.GetField ();
        var highlighted = false;

        field = fields[coor.up];
        if (TrySelect (field))
            highlighted = true;

        field = fields[coor.right];
        if (TrySelect (field))
            highlighted = true;

        field = fields[coor.down];
        if (TrySelect (field))
            highlighted = true;

        field = fields[coor.left];
        if (TrySelect (field))
            highlighted = true;

        if (highlighted)
            m_HighlightedTile = tile;
    }

    private bool TrySelect (TileField tile)
    {
        if (tile != null && tile.Piece == null)
        {
            tile.OnSelect ();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if the clicked piece corresponds to this turn
    /// </summary>
    public bool ValidateHighlight (Piece piece)
    {
        if (piece.type == ColorType.WHITE)
        {
            if (_turnManager.IsWhiteTurn ())
                return true;
        }
        else
        {
            if (!_turnManager.IsWhiteTurn ())
                return true;
        }

        return false;
    }

    public void UpdateBoard (TileField currentField, TileField lastField)
    {
        bool movedVertically = currentField.Coordinates.x > lastField.Coordinates.x || currentField.Coordinates.x < lastField.Coordinates.x;

        VerifyWall (currentField, movedVertically);
        _captureVerifier.VerifyCapture (_fieldProvider.GetField (), currentField, _turnManager.GetCurrentTurn ());
        //vERIFY kILLS
    }

    private void VerifyWall (TileField currentField, bool movedVertically)
    {
        //If the piece moved vertically, no need to check for a vertical wall
        if (movedVertically)
            if (_wallVerifier.HasHorizontalWall (_fieldProvider.GetField (), currentField, _turnManager.IsWhiteTurn ()))
                _gameFinisher.MinorVictory (_turnManager.GetCurrentTurn ());
            else
        if (_wallVerifier.HasVerticalWall (_fieldProvider.GetField (), currentField, _turnManager.IsWhiteTurn ()))
            _gameFinisher.MinorVictory (_turnManager.GetCurrentTurn ());
    }

    //Called by the Surrender Button
    public void Surrender ()
    {
        //who surrenders is who loses
        _gameFinisher.GreatVictory (_turnManager.GetCurrentTurn ());
    }
}