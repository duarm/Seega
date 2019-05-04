using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Kurenaiz.Utilities.Physics;
using Kurenaiz.Utilities.Types;
using System;

public enum GameState
{
    STARTING,
    POSITIONING,
    MOVEMENT,
    END
}

public enum VictoryType
{
    TOTAL,
    GREAT,
    MINOR
}

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

    [Header("Configuration")]
    public float fieldUpdateTime = .5f;     //Determines how fast the board will update

    bool m_IsUpdating;
    int m_PlacedCounter;                      //Counts the number of placed Pieces by the player during this turn in the positioning phase
    int m_TurnIndex;
    bool m_IsWhiteTurn;
    int m_PieceIndex;
    int m_WhiteKillCount;
    int m_BlackKillCount;

    WaitForSeconds m_FieldUpdateRate;
    TileField m_HighlightedTile;     //Keep track of the current selected piece
    GameState m_CurrentGameState;
    Safe2DArray m_Fields = new Safe2DArray(5,5);
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

    public delegate void OnTurnChange (bool isWhiteTurn);
    public OnTurnChange onTurnChangeCallback;

    public delegate void OnStateChange (GameState state);
    public OnStateChange onStateChangeCallback;

    public delegate void OnGameEnd (string winner, string winType);
    public OnGameEnd onGameEndCallback;

    void Awake ()
    {
        Debug.Log("Awaking");
        s_BoardInstance = this;
    }

    void Start ()
    {
        Debug.Log("Starting");
        m_CurrentGameState = GameState.STARTING;
        m_FieldUpdateRate = new WaitForSeconds (fieldUpdateTime);
        PopulateArrays();
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
                PhysicsHelper.TryGetTileField(hit.collider, out tile);
                if (m_CurrentGameState == GameState.MOVEMENT)
                {
                    //Two possibilities: the field can or not have a piece
                    if (tile.Piece != null)
                    {
                        // One possibility: If its a piece from the current turn, we try to highlight
                        if(ValidateHighlight (tile.Piece))
                        {
                            // Three possibilities: If the clicked tile is the current highlighted, we only dehighlight
                            // If the highlighted tile is null, we only highlight
                            // If the clicked tile isn't the current highlighted, we dehighlight and highlight the clicked one
                            if(tile == m_HighlightedTile)
                                Dehighlight();
                            else if(m_HighlightedTile == null)
                            {
                                HighlightEmptyAdjacents(tile);
                            }
                            else if(tile != m_HighlightedTile)
                            {
                                Dehighlight();
                                HighlightEmptyAdjacents(tile);
                            }
                        }
                    }
                    else
                    {
                        //One possibilities: if there is not a piece on the field we clicked, and we already have a highlighted field, we try to move
                        if (m_HighlightedTile != null)
                        {
                            //MOVE PIECE
                            if(tile.highlighting)
                            {
                                //If this tile is highlighted, we can move to this tile.
                                m_HighlightedTile.MovePieceTo(tile);
                                UpdateBoard(tile, m_HighlightedTile);
                                Dehighlight();
                                NextTurn();
                            }
                        }
                    }
                }
                else if (m_CurrentGameState == GameState.POSITIONING)
                {
                    //One Possibility: There's no piece on the field, we can only move to an empty space
                    if(tile.Piece == null)
                    {
                        //Debug.Log("Theres no a piece");
                        //No piece can be put in the middle on the positioning phase
                        if(tile.Row == 2 && tile.Column == 2)
                            return;

                        if (m_IsWhiteTurn)
                            tile.SetPiece(GetNonPlacedWhitePiece());
                        else
                            tile.SetPiece(GetNonPlacedBlackPiece());

                        m_TurnIndex++;

                        // Each player places 2 pieces before the next turn
                        if(m_TurnIndex == 2)
                        {
                            m_TurnIndex = 0;
                            NextTurn ();
                        }

                        //24 is the number of pieces
                        if(m_PieceIndex == 24)
                            StartMovementPhase();
                    }
                }
            }
        }
    }

    private void Dehighlight()
    {
        //Debug.Log("Dehighlighting");
        //Debug.Log(m_HighlightedTile);

        TileField field;

        field = m_Fields[m_HighlightedTile.Row + 1, m_HighlightedTile.Column];
        if(field?.highlighting == true)
        {
            //Debug.Log(field.ToString());
            field.Dehighlight();
        }

        field = m_Fields[m_HighlightedTile.Row - 1, m_HighlightedTile.Column];
        if(field?.highlighting == true)
        {
            //Debug.Log(field.ToString());
            field.Dehighlight();
        }

        field = m_Fields[m_HighlightedTile.Row, m_HighlightedTile.Column + 1];
        if(field?.highlighting == true)
        {
            //Debug.Log(field.ToString());
            field.Dehighlight();
        }

        field = m_Fields[m_HighlightedTile.Row, m_HighlightedTile.Column - 1];
        if(field?.highlighting == true)
        {
            //Debug.Log(field.ToString());
            field.Dehighlight();
        }

        m_HighlightedTile = null;
    }

    private void HighlightEmptyAdjacents(TileField tile)
    {
        TileField field;
        bool highlighted = false;

        field = m_Fields[tile.Row + 1, tile.Column];
        if(field != null)
        {
            if(field.Piece == null)
            {
                field.Highlight();
                highlighted = true;
            }
        }

        field = m_Fields[tile.Row - 1, tile.Column];
        if(field != null)
        {
            if(field.Piece == null)
            {
                field.Highlight();
                highlighted = true;
            }
        }

        field = m_Fields[tile.Row, tile.Column + 1];
        if(field != null)
        {
            if(field.Piece == null)
            {
                field.Highlight();
                highlighted = true;
            }
        }

        field = m_Fields[tile.Row, tile.Column - 1];
        if(field != null)
        {
            if(field.Piece == null)
            {
                field.Highlight();
                highlighted = true;
            }
        }

        if(highlighted)
            m_HighlightedTile = tile;

    }

    private void PopulateArrays()
    {
        int row = 0;
        int column = 0;
        bool isWhite = true;
        foreach (Transform field in tileFieldParent)
        {
            if(row == 5)
            {
                column++;
                row=0;
            }

            m_Fields[row,column] = field.GetComponent<TileField>().Initialize(row, column, isWhite);
            isWhite = !isWhite;
            row++;
        }

        m_Pieces = FindObjectsOfType<Piece>();
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
        bool movedVertically = currentField.Row > lastField.Row || currentField.Row < lastField.Row;

        VerifyWall(currentField, movedVertically);
        VerifyCapture(currentField);
        VerifyKills();
    }

    private void VerifyKills()
    {
        if(m_WhiteKillCount == 12)
            EndGame(true, VictoryType.TOTAL);
        else if(m_BlackKillCount == 12)
            EndGame(false, VictoryType.TOTAL);
    }

    private void VerifyCapture(TileField currentField)
    {
        //Pieces on the middle can't be captured
        if(currentField.Row == 2 && currentField.Column == 2)
            return;

        //TO IMPLEMENT: No need to check for capture in the direction the piece came from
        TryCapture(currentField);
    }

    private void TryCapture(TileField currentField)
    {
        PieceType currentPieceType = IsWhiteTurn ? PieceType.WHITE : PieceType.BLACK;
        int index;

        index = currentField.Row + 1;
        if(index <= 4)
        {
            TileField fieldToCheck = m_Fields[index, currentField.Column];
            //One Field above current field
            if(fieldToCheck.Piece != null)
            {
                bool captureConditionMatch = false;
                //Two Fields above current field
                if(m_Fields[index + 1, currentField.Column]?.Piece?.type == currentPieceType)
                {
                    captureConditionMatch = true;
                }

                if(fieldToCheck.Piece.type != currentPieceType && captureConditionMatch)
                {
                    fieldToCheck.Capture();

                    if(currentPieceType == PieceType.WHITE)
                        m_WhiteKillCount++;
                    else
                        m_BlackKillCount++;
                }
            }
        }

        index = currentField.Row - 1;
        if(index >= 0)
        {
            TileField fieldToCheck = m_Fields[index, currentField.Column];
            //One Field current field
            if(fieldToCheck.Piece != null)
            {
                bool captureConditionMatch = false;
                //Two Fields below current field
                if(m_Fields[index - 1, currentField.Column]?.Piece?.type == currentPieceType)
                {
                    captureConditionMatch = true;
                }

                if(fieldToCheck.Piece.type != currentPieceType  && captureConditionMatch)
                {
                    fieldToCheck.Capture();

                    if(currentPieceType == PieceType.WHITE)
                        m_WhiteKillCount++;
                    else
                        m_BlackKillCount++;
                }
            }
        }

        index = currentField.Column + 1;
        if(index <= 4)
        {
            TileField fieldToCheck = m_Fields[currentField.Row, index];
            //One Field to right of the current field
            if(fieldToCheck.Piece != null)
            {
                bool captureConditionMatch = false;
                //Two Fields to right of the current field
                if(m_Fields[currentField.Row, index + 1]?.Piece?.type == currentPieceType)
                {
                    captureConditionMatch = true;
                }

                if(fieldToCheck.Piece.type != currentPieceType  && captureConditionMatch)
                {
                    fieldToCheck.Capture();

                    if(currentPieceType == PieceType.WHITE)
                        m_WhiteKillCount++;
                    else
                        m_BlackKillCount++;
                }
            }
        }

        index = currentField.Column - 1;
        if(index >= 0)
        {
            TileField fieldToCheck = m_Fields[currentField.Row, index];
            //One Field to left of the current field
            if(fieldToCheck.Piece != null)
            {
                bool captureConditionMatch = false;
                //Two Fields to left of the current field
                if(m_Fields[currentField.Row, index - 1]?.Piece?.type == currentPieceType)
                {
                    captureConditionMatch = true;
                }

                if(fieldToCheck.Piece.type != currentPieceType  && captureConditionMatch)
                {
                    fieldToCheck.Capture();

                    if(currentPieceType == PieceType.WHITE)
                        m_WhiteKillCount++;
                    else
                        m_BlackKillCount++;
                }
            }
        }
    }

    private void VerifyWall(TileField currentField, bool movedVertically)
    {
        //If the piece moved vertically, no need to check for a vertical wall
        if(movedVertically)
        {
            if(HasHorizontalWall(currentField))
                EndGame(m_IsWhiteTurn, VictoryType.MINOR);
        }
        else
        {
            if(HasVerticalWall(currentField))
                EndGame(m_IsWhiteTurn, VictoryType.MINOR);
        }
    }

    private bool HasVerticalWall(TileField currentField)
    {
        PieceType currentPieceType = IsWhiteTurn ? PieceType.WHITE : PieceType.BLACK;
        for(int i = 0; i < 5; i++)
        {
            int positiveIndex = currentField.Row + i;
            if(positiveIndex <= 4)
            {
                if(!(currentPieceType == m_Fields[positiveIndex, currentField.Column].Piece?.type))
                {
                    return false;
                }
            }

            int negativeIndex = currentField.Row - i;
            if(negativeIndex >= 0)
            {
                if(!(currentPieceType == m_Fields[negativeIndex, currentField.Column].Piece?.type))
                {
                    return false;
                }
            }
        }

        Debug.Log("VERTICAL WIN");

        return true;
    }

    private bool HasHorizontalWall(TileField currentField)
    {
        PieceType currentPieceType = IsWhiteTurn ? PieceType.WHITE : PieceType.BLACK;
        for(int i = 0; i < 5; i++)
        {
            int positiveIndex = currentField.Column + i;
            if(positiveIndex <= 4)
            {
                if(!(currentPieceType == m_Fields[currentField.Row, positiveIndex].Piece?.type))
                {
                    return false;
                }
            }

            int negativeIndex = currentField.Column - i;
            if(negativeIndex >= 0)
            {
                if(!(currentPieceType == m_Fields[currentField.Row, negativeIndex].Piece?.type))
                {
                    return false;
                }
            }
        }

        Debug.Log("HORIZONTAL WIN");

        return true;
    }

    public void EndGame (bool isWhite, VictoryType victoryType)
    {
        CurrentState = GameState.END;

        if (isWhite)
        {
            if (victoryType == VictoryType.TOTAL)
            {
                onGameEndCallback.Invoke ("Brancas", "Total");
            }
            else if (victoryType == VictoryType.GREAT)
            {
                onGameEndCallback.Invoke ("Brancas", "Grande");
            }
            else if (victoryType == VictoryType.MINOR)
            {
                onGameEndCallback.Invoke ("Brancas", "Pequena");
            }
        }
        else
        {
            if (victoryType == VictoryType.TOTAL)
            {
                onGameEndCallback.Invoke ("Pretas", "Total");
            }
            else if (victoryType == VictoryType.GREAT)
            {
                onGameEndCallback.Invoke ("Pretas", "Grande");
            }
            else if (victoryType == VictoryType.MINOR)
            {
                onGameEndCallback.Invoke ("Pretas", "Pequena");
            }
        }
    }

    public void NextTurn ()
    {
        m_IsWhiteTurn = !m_IsWhiteTurn;
        m_HighlightedTile = null;

        if (onTurnChangeCallback != null)
            onTurnChangeCallback.Invoke (m_IsWhiteTurn);
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

        if (onStateChangeCallback != null)
            onStateChangeCallback.Invoke (m_CurrentGameState);

        if (onTurnChangeCallback != null)
            onTurnChangeCallback.Invoke (m_IsWhiteTurn);
    }

    public void StartMovementPhase ()
    {
        m_CurrentGameState = GameState.MOVEMENT;

        if (onStateChangeCallback != null)
            onStateChangeCallback.Invoke (m_CurrentGameState);
    }

    //Game Methods
    private Piece GetNonPlacedBlackPiece ()
    {
        Debug.Log("Getting Piece");
        foreach (Piece piece in m_Pieces)
        {
            if(piece.type == PieceType.BLACK && !piece.isPlaced)
            {
                m_PieceIndex++;
                return piece;
            }
        }

        return null;
    }

    private Piece GetNonPlacedWhitePiece ()
    {
        Debug.Log("Getting Piece");
        foreach (Piece piece in m_Pieces)
        {
            if(piece.type == PieceType.WHITE && !piece.isPlaced)
            {
                m_PieceIndex++;
                return piece;
            }
        }

        return null;
    }
}

//Implement Coordinates
/* public struct Coordinates
{
    int x;
    int y;
} */