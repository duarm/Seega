using System;
using Kurenaiz.Utilities.Physics;
using Kurenaiz.Utilities.Types;
using Seega.GlobalEnums;
using Seega.Scripts.Types;
using UnityEngine;
using Zenject;

namespace Seega.Scripts.Core
{
    public class Board : MonoBehaviour
    {
        public Transform tileFieldParent;

        bool m_IsUpdating;
        int m_TurnIndex;
        bool _isWhiteTurn;
        int m_PieceIndex;
        int m_WhiteKillCount;
        int m_BlackKillCount;

        TileField m_HighlightedTile; //Keep track of the current selected piece
        GameState _currentGameState = GameState.STARTING;
        Safe2DArray m_Fields = new Safe2DArray(5, 5);
        Piece[] m_Pieces;


        public Action<GameState> OnStateChange;
        public Action<bool> OnTurnChange;
        public Action<string, string> OnGameEnd;

        public IRayProvider _rayProvider;
        public ISelector _selector;

        private PhysicsCache _physicsCache;

        [Inject]
        private void Construct(PhysicsCache physicsCache)
        {
            _physicsCache = physicsCache;
        }

        void Awake() => SolveDependencies();

        void Start() => PopulateArrays();

        private void SolveDependencies()
        {
            _rayProvider = GetComponent<IRayProvider>();
            _selector = GetComponent<ISelector>();
        }

        private void PopulateArrays()
        {
            var row = 0;
            var column = 0;
            var isWhite = true;
            foreach (Transform field in tileFieldParent)
            {
                if (row == 5)
                {
                    column++;
                    row = 0;
                }

                m_Fields[row, column] = field.GetComponent<TileField>().Initialize(row, column, isWhite);
                isWhite = !isWhite;
                row++;
            }

            m_Pieces = FindObjectsOfType<Piece>();
        }

        public void OnMouseClick()
        {
            //CREATING THE RAY
            var ray = _rayProvider.CreateRay();

            //SHOTING THE RAY
            //shooting a raycast to get the tile that the player clicked
            if (_selector.Check(ray))
            {
                TileField tile;
                _physicsCache.TryGetTileField(_selector.GetSelection(), out tile);
                if (_currentGameState == GameState.MOVEMENT)
                {
                    //Two possibilities: the field can or not have a piece
                    if (tile.Piece != null)
                    {
                        // One possibility: If its a piece from the current turn, we try to highlight
                        if (ValidateHighlight(tile.Piece))
                        {
                            // Three possibilities: 
                            // If the clicked tile is the current highlighted, we only dehighlight
                            // If the highlighted tile is null, we only highlight
                            // If the clicked tile isn't the current highlighted, we dehighlight and highlight the clicked one
                            if (tile == m_HighlightedTile)
                                Deselect();
                            else if (m_HighlightedTile == null)
                                TrySelectEmptyAdjacents(tile);
                            else if (tile != m_HighlightedTile)
                            {
                                Deselect();
                                TrySelectEmptyAdjacents(tile);
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
                                m_HighlightedTile.MovePieceTo(tile);
                                UpdateBoard(tile, m_HighlightedTile);
                                Deselect();
                                NextTurn();
                            }
                        }
                    }
                }
                else if (_currentGameState == GameState.POSITIONING)
                {
                    //One Possibility: There's no piece on the field, we can only move to an empty space
                    if (tile.Piece == null)
                    {
                        //No piece can be put in the middle on the positioning phase
                        if (tile.Coordinates.x == 2 && tile.Coordinates.y == 2)
                            return;

                        if (_isWhiteTurn)
                            tile.SetPiece(GetNonPlacedWhitePiece());
                        else
                            tile.SetPiece(GetNonPlacedBlackPiece());

                        m_TurnIndex++;

                        // Each player places 2 pieces before the next turn
                        if (m_TurnIndex == 2)
                        {
                            m_TurnIndex = 0;
                            NextTurn();
                        }

                        //24 is the number of pieces
                        if (m_PieceIndex == 24)
                            StartMovementPhase();
                    }
                }
            }
        }

        private void Deselect()
        {
            TileField field;
            var coor = new Coordinates(m_HighlightedTile);

            field = m_Fields[coor.up];
            if (field?.IsHighlighting == true)
                field.OnDeselect();

            field = m_Fields[coor.right];
            if (field?.IsHighlighting == true)
                field.OnDeselect();

            field = m_Fields[coor.down];
            if (field?.IsHighlighting == true)
                field.OnDeselect();

            field = m_Fields[coor.left];
            if (field?.IsHighlighting == true)
                field.OnDeselect();

            m_HighlightedTile = null;
        }

        private void TrySelectEmptyAdjacents(TileField tile)
        {
            TileField field;
            var coor = new Coordinates(tile);
            var highlighted = false;

            field = m_Fields[coor.up];
            if (TrySelect(field))
                highlighted = true;

            field = m_Fields[coor.right];
            if (TrySelect(field))
                highlighted = true;

            field = m_Fields[coor.down];
            if (TrySelect(field))
                highlighted = true;

            field = m_Fields[coor.left];
            if (TrySelect(field))
                highlighted = true;

            if (highlighted)
                m_HighlightedTile = tile;
        }

        private bool TrySelect(TileField tile)
        {
            if (tile != null && tile.Piece == null)
            {
                tile.OnSelect();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the clicked piece corresponds to this turn
        /// </summary>
        public bool ValidateHighlight(Piece piece)
        {
            if (piece.type == PieceType.WHITE)
            {
                if (_isWhiteTurn)
                    return true;
            }
            else
            {
                if (!_isWhiteTurn)
                    return true;
            }

            return false;
        }

        public void UpdateBoard(TileField currentField, TileField lastField)
        {
            bool movedVertically = currentField.Coordinates.x > lastField.Coordinates.x || currentField.Coordinates.x < lastField.Coordinates.x;

            VerifyWall(currentField, movedVertically);
            VerifyCapture(currentField);
            VerifyKills();
        }

        private void VerifyKills()
        {
            if (m_WhiteKillCount == 12)
                EndGame(true, VictoryType.TOTAL);
            else if (m_BlackKillCount == 12)
                EndGame(false, VictoryType.TOTAL);
        }

        private void VerifyCapture(TileField currentField)
        {
            var currentPieceType = _isWhiteTurn ? PieceType.WHITE : PieceType.BLACK;
            var coordinates = new Coordinates(currentField);
            CheckForCaptureOnSide(currentField, currentPieceType, coordinates.up, coordinates.right, coordinates.down, coordinates.left);
        }

        private void CheckForCaptureOnSide(TileField currentTile, PieceType pieceType, params Coordinates[] coords)
        {
            for (int i = 0; i < coords.Length; i++)
            {
                //Verifying the piece adjacent to the currentField
                //[farTile][inBetweenTile][currentTile]
                var farCoordinates = coords[i];
                var inBetweenTile = m_Fields[coords[i]];

                if (currentTile.Coordinates.x > coords[i].x)
                    farCoordinates.x--;
                else if (currentTile.Coordinates.x < coords[i].x)
                    farCoordinates.x++;
                else if (currentTile.Coordinates.y < coords[i].y)
                    farCoordinates.y++;
                else if (currentTile.Coordinates.y > coords[i].y)
                    farCoordinates.y--;

                var farTile = m_Fields[farCoordinates];

                //Verifying the adjacent tile is an enemy
                if (inBetweenTile != null &&
                    inBetweenTile.Piece != null &&
                    inBetweenTile.Piece.type != pieceType)
                {
                    //Veryfying if the far tile is an ally
                    if (farTile != null &&
                        farTile.Piece != null &&
                        farTile.Piece.type == pieceType)
                    {
                        //if the inBetweenPiece is on the middle, it can't be captured
                        if (inBetweenTile.Coordinates.x != 2 ||
                            inBetweenTile.Coordinates.y != 2)
                        {
                            inBetweenTile.Capture();
                            if (pieceType == PieceType.WHITE)
                                m_WhiteKillCount++;
                            else
                                m_BlackKillCount++;
                        }
                    }
                }
            }
        }

        private void VerifyWall(TileField currentField, bool movedVertically)
        {
            //If the piece moved vertically, no need to check for a vertical wall
            if (movedVertically)
            {
                if (HasHorizontalWall(currentField))
                    EndGame(_isWhiteTurn, VictoryType.MINOR);
            }
            else
            {
                if (HasVerticalWall(currentField))
                    EndGame(_isWhiteTurn, VictoryType.MINOR);
            }
        }

        private bool HasVerticalWall(TileField currentField)
        {
            var currentPieceType = _isWhiteTurn ? PieceType.WHITE : PieceType.BLACK;
            for (int i = 0; i < 5; i++)
            {
                int positiveIndex = currentField.Coordinates.x + i;
                if (positiveIndex <= 4)
                {
                    if (!(currentPieceType == m_Fields[positiveIndex, currentField.Coordinates.y].Piece?.type))
                    {
                        return false;
                    }
                }

                int negativeIndex = currentField.Coordinates.x - i;
                if (negativeIndex >= 0)
                {
                    if (!(currentPieceType == m_Fields[negativeIndex, currentField.Coordinates.y].Piece?.type))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool HasHorizontalWall(TileField currentField)
        {
            var currentPieceType = _isWhiteTurn ? PieceType.WHITE : PieceType.BLACK;
            for (int i = 0; i < 5; i++)
            {
                int positiveIndex = currentField.Coordinates.y + i;
                if (positiveIndex <= 4)
                {
                    if (!(currentPieceType == m_Fields[currentField.Coordinates.x, positiveIndex].Piece?.type))
                    {
                        return false;
                    }
                }

                int negativeIndex = currentField.Coordinates.y - i;
                if (negativeIndex >= 0)
                {
                    if (!(currentPieceType == m_Fields[currentField.Coordinates.x, negativeIndex].Piece?.type))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void EndGame(bool isWhite, VictoryType victoryType)
        {
            _currentGameState = GameState.END;

            if (isWhite)
            {
                if (victoryType == VictoryType.TOTAL)
                {
                    OnGameEnd("Brancas", "Total");
                }
                else if (victoryType == VictoryType.GREAT)
                {
                    OnGameEnd("Brancas", "Grande");
                }
                else if (victoryType == VictoryType.MINOR)
                {
                    OnGameEnd("Brancas", "Pequena");
                }
            }
            else
            {
                if (victoryType == VictoryType.TOTAL)
                {
                    OnGameEnd("Pretas", "Total");
                }
                else if (victoryType == VictoryType.GREAT)
                {
                    OnGameEnd("Pretas", "Grande");
                }
                else if (victoryType == VictoryType.MINOR)
                {
                    OnGameEnd("Pretas", "Pequena");
                }
            }
        }

        public void NextTurn()
        {
            _isWhiteTurn = !_isWhiteTurn;
            m_HighlightedTile = null;

            if (OnTurnChange != null)
                OnTurnChange(_isWhiteTurn);
        }

        //Called by the Surrender Button
        public void Surrender()
        {
            //who surrenders is who loses
            EndGame(!_isWhiteTurn, VictoryType.GREAT);
        }

        //Called by the buttons while choosing who starts
        public void StartPositioningPhase(bool isWhite)
        {
            _currentGameState = GameState.POSITIONING;
            _isWhiteTurn = isWhite;

            if (OnStateChange != null)
                OnStateChange(_currentGameState);

            if (OnStateChange != null)
                OnTurnChange(_isWhiteTurn);
        }

        public void StartMovementPhase()
        {
            _currentGameState = GameState.MOVEMENT;

            if (OnStateChange != null)
                OnStateChange(_currentGameState);
        }

        //Game Methods
        private Piece GetNonPlacedBlackPiece()
        {
            foreach (Piece piece in m_Pieces)
            {
                if (piece.type == PieceType.BLACK && !piece.isPlaced)
                {
                    m_PieceIndex++;
                    return piece;
                }
            }

            return null;
        }

        private Piece GetNonPlacedWhitePiece()
        {
            foreach (Piece piece in m_Pieces)
            {
                if (piece.type == PieceType.WHITE && !piece.isPlaced)
                {
                    m_PieceIndex++;
                    return piece;
                }
            }

            return null;
        }
    }
}