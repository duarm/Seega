using System;
using System.Collections.Generic;
using System.Linq;
using Kurenaiz.Utilities.Physics;
using Kurenaiz.Utilities.Types;
using Seega.GlobalEnums;
using UnityEngine;

public class Board : MonoBehaviour
{
    #region Singleton
    static protected Board s_BoardInstance;
    static public Board Instance { get { return s_BoardInstance; } }
    #endregion

    //Layers Masks
    public LayerMask whatIsTile;
    public Transform tileFieldParent;

    //Colors
    [Header ("Materials")]
    public Material blackHighlightMaterial;
    public Material whiteHighlightMaterial;
    public Material blackNormalMaterial;
    public Material whiteNormalMaterial;

    [Header ("Configuration")]
    public float fieldUpdateTime = .5f; //Determines how fast the board will update

    bool m_IsUpdating;
    int m_TurnIndex;
    bool m_IsWhiteTurn;
    int m_PieceIndex;
    int m_WhiteKillCount;
    int m_BlackKillCount;

    WaitForSeconds m_FieldUpdateRate;
    TileField m_HighlightedTile; //Keep track of the current selected piece
    GameState m_CurrentGameState;
    Safe2DArray m_Fields = new Safe2DArray (5, 5);
    Piece[] m_Pieces;

    public bool IsUpdating
    {
        get { return m_IsUpdating; }
        set { m_IsUpdating = value; }
    }

    public bool IsWhiteTurn
    {
        get { return m_IsWhiteTurn; }
        set { m_IsWhiteTurn = value; }
    }

    public GameState CurrentState
    {
        get { return m_CurrentGameState; }
        set { m_CurrentGameState = value; }
    }

    public Action<GameState> OnStateChange;
    public Action<bool> OnTurnChange;
    public Action<string, string> OnGameEnd;

    void Awake ()
    {
        s_BoardInstance = this;
    }

    void Start ()
    {
        m_CurrentGameState = GameState.STARTING;
        m_FieldUpdateRate = new WaitForSeconds (fieldUpdateTime);
        PopulateArrays ();
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown (0) && !m_IsUpdating)
        {
            //shooting a raycast to get the tile that the player clicked
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit, 20, whatIsTile))
            {
                TileField tile;
                PhysicsHelper.TryGetTileField (hit.collider, out tile);
                Debug.Log(tile.coordinates);
                if (m_CurrentGameState == GameState.MOVEMENT)
                {
                    //Two possibilities: the field can or not have a piece
                    if (tile.Piece != null)
                    {
                        // One possibility: If its a piece from the current turn, we try to highlight
                        if (ValidateHighlight (tile.Piece))
                        {
                            // Three possibilities: If the clicked tile is the current highlighted, we only dehighlight
                            // If the highlighted tile is null, we only highlight
                            // If the clicked tile isn't the current highlighted, we dehighlight and highlight the clicked one
                            if (tile == m_HighlightedTile)
                                Dehighlight ();
                            else if (m_HighlightedTile == null)
                            {
                                HighlightEmptyAdjacents (tile);
                            }
                            else if (tile != m_HighlightedTile)
                            {
                                Dehighlight ();
                                HighlightEmptyAdjacents (tile);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log ("No Piece");
                        //One possibilities: if there is not a piece on the field we clicked, and we already have a highlighted field, we try to move
                        if (m_HighlightedTile != null)
                        {
                            Debug.Log ("Has a tile Highlighted");
                            //MOVE PIECE
                            if (tile.highlighting)
                            {
                                //If this tile is highlighted, we can move to this tile.
                                m_HighlightedTile.MovePieceTo (tile);
                                UpdateBoard (tile, m_HighlightedTile);
                                Dehighlight ();
                                NextTurn ();
                            }
                        }
                    }
                }
                else if (m_CurrentGameState == GameState.POSITIONING)
                {
                    //One Possibility: There's no piece on the field, we can only move to an empty space
                    if (tile.Piece == null)
                    {
                        //No piece can be put in the middle on the positioning phase
                        if (tile.coordinates.x == 2 && tile.coordinates.y == 2)
                            return;

                        if (m_IsWhiteTurn)
                            tile.SetPiece (GetNonPlacedWhitePiece ());
                        else
                            tile.SetPiece (GetNonPlacedBlackPiece ());

                        m_TurnIndex++;

                        // Each player places 2 pieces before the next turn
                        if (m_TurnIndex == 2)
                        {
                            m_TurnIndex = 0;
                            NextTurn ();
                        }

                        //24 is the number of pieces
                        if (m_PieceIndex == 24)
                            StartMovementPhase ();
                    }
                }
            }
        }
    }

    private void Dehighlight ()
    {
        TileField field;
        var coor = new Coordinates (m_HighlightedTile);

        field = m_Fields[coor.up];
        if (field?.highlighting == true)
            field.Dehighlight ();

        field = m_Fields[coor.right];
        if (field?.highlighting == true)
            field.Dehighlight ();

        field = m_Fields[coor.down];
        if (field?.highlighting == true)
            field.Dehighlight ();

        field = m_Fields[coor.left];
        if (field?.highlighting == true)
            field.Dehighlight ();

        m_HighlightedTile = null;
    }

    private void HighlightEmptyAdjacents (TileField tile)
    {
        TileField field;
        var coor = new Coordinates (tile);
        var highlighted = false;

        field = m_Fields[coor.up];
        if (TryHighlight (field))
            highlighted = true;

        field = m_Fields[coor.right];
        if (TryHighlight (field))
            highlighted = true;

        field = m_Fields[coor.down];
        if (TryHighlight (field))
            highlighted = true;

        field = m_Fields[coor.left];
        if (TryHighlight (field))
            highlighted = true;

        if (highlighted)
            m_HighlightedTile = tile;
    }

    private bool TryHighlight (TileField tile)
    {
        if (tile != null && tile.Piece == null)
        {
            tile.Highlight ();
            return true;
        }
        return false;
    }

    private void PopulateArrays ()
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

            m_Fields[row, column] = field.GetComponent<TileField> ().Initialize (row, column, isWhite);
            isWhite = !isWhite;
            row++;
        }

        m_Pieces = FindObjectsOfType<Piece> ();
    }

    /// <summary>
    /// Check if the clicked piece corresponds to this turn
    /// </summary>
    public bool ValidateHighlight (Piece piece)
    {
        if (piece.type == PieceType.WHITE)
        {
            if (m_IsWhiteTurn)
                return true;
        }
        else
        {
            if (!m_IsWhiteTurn)
                return true;
        }

        return false;
    }

    public void UpdateBoard (TileField currentField, TileField lastField)
    {
        bool movedVertically = currentField.coordinates.x > lastField.coordinates.x || currentField.coordinates.x < lastField.coordinates.x;

        VerifyWall (currentField, movedVertically);
        VerifyCapture (currentField);
        VerifyKills ();
    }

    private void VerifyKills ()
    {
        if (m_WhiteKillCount == 12)
            EndGame (true, VictoryType.TOTAL);
        else if (m_BlackKillCount == 12)
            EndGame (false, VictoryType.TOTAL);
    }

    private void VerifyCapture (TileField currentField)
    {
        var currentPieceType = IsWhiteTurn ? PieceType.WHITE : PieceType.BLACK;
        var coordinates = new Coordinates (currentField);
        CheckForCaptureOnSide (currentField, currentPieceType, coordinates.up, coordinates.right, coordinates.down, coordinates.left);
    }

    private void CheckForCaptureOnSide (TileField currentTile, PieceType pieceType, params Coordinates[] coords)
    {
        for (int i = 0; i < coords.Length; i++)
        {
            //Verifying the piece adjacent to the currentField
            //[farTile][inBetweenTile][currentTile]
            var farCoordinates = coords[i];
            var inBetweenTile = m_Fields[coords[i]];

            if (currentTile.coordinates.x > coords[i].x)
                farCoordinates.x--;
            else if (currentTile.coordinates.x < coords[i].x)
                farCoordinates.x++;
            else if (currentTile.coordinates.y < coords[i].y)
                farCoordinates.y++;
            else if (currentTile.coordinates.y > coords[i].y)
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
                    if (inBetweenTile.coordinates.x != 2 ||
                        inBetweenTile.coordinates.y != 2)
                    {
                        inBetweenTile.Capture ();
                        if (pieceType == PieceType.WHITE)
                            m_WhiteKillCount++;
                        else
                            m_BlackKillCount++;
                    }
                }
            }
        }
    }

    private void VerifyWall (TileField currentField, bool movedVertically)
    {
        //If the piece moved vertically, no need to check for a vertical wall
        if (movedVertically)
        {
            if (HasHorizontalWall (currentField))
                EndGame (m_IsWhiteTurn, VictoryType.MINOR);
        }
        else
        {
            if (HasVerticalWall (currentField))
                EndGame (m_IsWhiteTurn, VictoryType.MINOR);
        }
    }

    private bool HasVerticalWall (TileField currentField)
    {
        var currentPieceType = IsWhiteTurn ? PieceType.WHITE : PieceType.BLACK;
        for (int i = 0; i < 5; i++)
        {
            int positiveIndex = currentField.coordinates.x + i;
            if (positiveIndex <= 4)
            {
                if (!(currentPieceType == m_Fields[positiveIndex, currentField.coordinates.y].Piece?.type))
                {
                    return false;
                }
            }

            int negativeIndex = currentField.coordinates.x - i;
            if (negativeIndex >= 0)
            {
                if (!(currentPieceType == m_Fields[negativeIndex, currentField.coordinates.y].Piece?.type))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool HasHorizontalWall (TileField currentField)
    {
        var currentPieceType = IsWhiteTurn ? PieceType.WHITE : PieceType.BLACK;
        for (int i = 0; i < 5; i++)
        {
            int positiveIndex = currentField.coordinates.y + i;
            if (positiveIndex <= 4)
            {
                if (!(currentPieceType == m_Fields[currentField.coordinates.x, positiveIndex].Piece?.type))
                {
                    return false;
                }
            }

            int negativeIndex = currentField.coordinates.y - i;
            if (negativeIndex >= 0)
            {
                if (!(currentPieceType == m_Fields[currentField.coordinates.x, negativeIndex].Piece?.type))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void EndGame (bool isWhite, VictoryType victoryType)
    {
        CurrentState = GameState.END;

        if (isWhite)
        {
            if (victoryType == VictoryType.TOTAL)
            {
                OnGameEnd ("Brancas", "Total");
            }
            else if (victoryType == VictoryType.GREAT)
            {
                OnGameEnd ("Brancas", "Grande");
            }
            else if (victoryType == VictoryType.MINOR)
            {
                OnGameEnd ("Brancas", "Pequena");
            }
        }
        else
        {
            if (victoryType == VictoryType.TOTAL)
            {
                OnGameEnd ("Pretas", "Total");
            }
            else if (victoryType == VictoryType.GREAT)
            {
                OnGameEnd ("Pretas", "Grande");
            }
            else if (victoryType == VictoryType.MINOR)
            {
                OnGameEnd ("Pretas", "Pequena");
            }
        }
    }

    public void NextTurn ()
    {
        m_IsWhiteTurn = !m_IsWhiteTurn;
        m_HighlightedTile = null;

        if (OnTurnChange != null)
            OnTurnChange (m_IsWhiteTurn);
    }

    //Called by the Surrender Button
    public void Surrender ()
    {
        //who surrenders is who loses
        EndGame (!m_IsWhiteTurn, VictoryType.GREAT);
    }

    //Called by the buttons while choosing who starts
    public void StartPositioningPhase (bool isWhite)
    {
        m_CurrentGameState = GameState.POSITIONING;
        m_IsWhiteTurn = isWhite;

        if (OnStateChange != null)
            OnStateChange (m_CurrentGameState);

        if (OnStateChange != null)
            OnTurnChange (m_IsWhiteTurn);
    }

    public void StartMovementPhase ()
    {
        m_CurrentGameState = GameState.MOVEMENT;

        if (OnStateChange != null)
            OnStateChange (m_CurrentGameState);
    }

    //Game Methods
    private Piece GetNonPlacedBlackPiece ()
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

    private Piece GetNonPlacedWhitePiece ()
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