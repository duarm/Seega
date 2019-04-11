using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GameState
{
    STARTING,
    POSITIONING,
    MOVEMENT,
    END
}

public enum Turn
{
    WHITE,
    BLACK
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

    public bool IsUpdating
    {
        get { return isUpdating; }
        set { isUpdating = value; }
    }

    //Layers Masks
    [Header ("Layer Masks")]
    public LayerMask whatIsPiece;
    public LayerMask whatIsTile;

    //Colors
    [Header ("Materials")]
    public Material blackHighlightMaterial;
    public Material whiteHighlightMaterial;
    public Material blackNormalMaterial;
    public Material whiteNormalMaterial;
    //public Color removeTokenColor;

    //public bool movementLocked;
    public float fieldUpdateTime = .5f; //Determines how fast the board will update

    bool isUpdating;
    int placedCounter; //Counts the number of placed Pieces by the player during this turn in the positioning phase
    WaitForSeconds fieldUpdateRate;
    public TileField m_HighlightedTile;     //Keep track of the current selected piece
    Turn m_CurrentTurn;
    GameState m_CurrentGameState;
    List<Piece> m_Pieces;
    List<Piece> m_WhitePieces;
    List<Piece> m_BlackPieces;
    List<TileField> m_Fields;

    public bool IsHighlighting
    {
        get { return m_HighlightedTile != null; }
    }
    public TileField HighlightedField
    {
        get{ return m_HighlightedTile; }
        set{ m_HighlightedTile = value; }
    }

    public Turn CurrentTurn
    {
        get { return m_CurrentTurn; }
        set { m_CurrentTurn = value; }
    }

    public GameState CurrentState
    {
        get { return m_CurrentGameState; }
        set { m_CurrentGameState = value; }
    }

    //WORKING   
    public bool HasWhitePieces
    {
        get;
        private set;
    }

    public bool HasBlackPieces
    {
        get;
        private set;
    }
    //END WORKING

    public delegate void OnTurnChange ();
    public OnTurnChange onTurnChangeCallback;

    public delegate void OnStateChange ();
    public OnStateChange onStateChangeCallback;

    public delegate void OnGameEnd (string winner, string winType);
    public OnGameEnd onGameEndCallback;

    //Unity Methods
    void Reset ()
    {
        m_Pieces.Clear ();
        m_WhitePieces.Clear ();
        m_BlackPieces.Clear ();
    }

    void Awake ()
    {
        s_BoardInstance = this;
        HasWhitePieces = true;
        HasBlackPieces = true;
    }

    void Start ()
    {
        m_CurrentGameState = GameState.STARTING;
        fieldUpdateRate = new WaitForSeconds (fieldUpdateTime);
        m_WhitePieces = new List<Piece>();
        m_BlackPieces = new List<Piece>();

        m_Fields = FindObjectsOfType<TileField>().ToList();
        m_Pieces = FindObjectsOfType<Piece>().ToList();

        foreach(Piece piece in m_Pieces)
        {
            if(piece.type == PieceType.WHITE)
                m_WhitePieces.Add(piece);
            else
                m_BlackPieces.Add(piece);
        }
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown (0) && !isUpdating)
        {
            if (m_CurrentGameState == GameState.MOVEMENT)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                //shooting a raycast to get the tile that the player clicked
                if (Physics.Raycast (ray, out hit, 20, whatIsTile))
                {
                    var tile = hit.collider.GetComponent<TileField> ();
                    Debug.Log("Clicked  Tiled: " + tile.name);

                    //if theres a piece on this tile
                    if (tile.piece != null)
                    {
                        if(ValidateHighlight (tile.piece))
                        {
                            if(tile != m_HighlightedTile)
                                tile.HighlightEmptyAdjacents(); 
                            else
                                tile.DehighlightAdjacents();
                        }
                        else
                        {
                            if(IsHighlighting)
                                m_HighlightedTile.DehighlightAdjacents();
                        }
                    }
                    else if (m_HighlightedTile != null) //if there is not a piece on the field we clicked, and we already have a highlighted field
                    {
                        //we validate the movement and move if its possible
                        if (ValidateMovement (tile))
                        {
                            Debug.Log(m_HighlightedTile);
                            Debug.Log("Calling Piece Moved of :" + tile.gameObject.name);
                            StartCoroutine (tile.PieceMoved ());
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check if the clicked piece corresponds to this turn
    /// </summary>
    public bool ValidateHighlight (Piece piece)
    {
        if (piece.type == PieceType.WHITE)
        {
            if (m_CurrentTurn == Turn.WHITE)
                return true;
        }
        else
        {
            if (m_CurrentTurn == Turn.BLACK)
                return true;
        }

        return false;
    }

    private bool ValidateMovement (TileField moveField)
    {
        if (m_HighlightedTile.CanMoveUp)
        {
            if(m_HighlightedTile.UpTile == moveField)
                return true;
        }

        if (m_HighlightedTile.CanMoveDown)
        {
            if(m_HighlightedTile.DownTile == moveField)
                return true;
        }

        if (m_HighlightedTile.CanMoveRight)
        {
            if(m_HighlightedTile.RightTile == moveField)
                return true;
        }

        if (m_HighlightedTile.CanMoveLeft)
        {
            if(m_HighlightedTile.LeftTile == moveField)
                return true;
        }

        return false;
    }

    public void UpdateBoard ()
    {
        StartCoroutine (Updater ());
    }

    IEnumerator Updater ()
    {
        UpdatePieces ();

        foreach (Piece piece in m_Pieces)
        {
            if (piece != null)
            {
                if (piece.VerifyAll () > 0)
                    yield return fieldUpdateRate;
            }
        }

        UpdatePieces ();

        int count = 0;
        foreach (Piece piece in m_WhitePieces)
        {
            if (piece == null)
            {
                count++;
            }
        }

        if (count == 12)
        {
            EndGame (PieceType.BLACK, VictoryType.TOTAL);
            yield return null;
        }
        else
            count = 0;

        foreach (Piece piece in m_BlackPieces)
        {
            if (piece == null)
            {
                count++;
            }
        }

        if (count == 12)
        {
            EndGame (PieceType.WHITE, VictoryType.TOTAL);
        }

        NextTurn ();
        /*if (m_HighlightedTile.piece.turnCaptures == 0)
        {
            NextTurn ();
        }
        else
        {
            m_HighlightedTile.HighlightEmptyAdjacents ();
            m_HighlightedTile.piece.isCapturingSequence = true;
        }*/

        isUpdating = false;
    }

    public void EndGame (PieceType winner, VictoryType victoryType)
    {
        if (onGameEndCallback == null)
            return;

        CurrentState = GameState.END;

        if (winner == PieceType.WHITE)
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

    public void StartPositioningPhase ()
    {
        m_CurrentGameState = GameState.POSITIONING;

        if (onStateChangeCallback != null)
            onStateChangeCallback.Invoke ();

        if (onTurnChangeCallback != null)
            onTurnChangeCallback.Invoke ();
    }

    public void VerifyState ()
    {
        if (m_CurrentTurn == Turn.WHITE)
        {
            if (LastpieceIsPlaced (PieceType.BLACK))
                StartMovementPhase ();
            else
                NextTurn ();
        }
        else
        {
            if (LastpieceIsPlaced (PieceType.WHITE))
                StartMovementPhase ();
            else
                NextTurn ();
        }
    }

    public void UpdatePlacedCounters ()
    {
        this.placedCounter++;
        if (this.placedCounter == 2)
        {
            this.placedCounter = 0;
            VerifyState ();
        }
    }

    public void NextTurn ()
    {
        if (m_CurrentTurn == Turn.WHITE)
        {
            m_CurrentTurn = Turn.BLACK;
        }
        else
        {
            m_CurrentTurn = Turn.WHITE;
        }

        m_HighlightedTile = null;

        if (onTurnChangeCallback != null)
            onTurnChangeCallback.Invoke ();
    }

    public void StartMovementPhase ()
    {
        m_CurrentGameState = GameState.MOVEMENT;

        if (onStateChangeCallback != null)
            onStateChangeCallback.Invoke ();
    }

    //Game Methods

    /// <summary>
    /// Returns the first non placed White piece avaiable.null Returns null if none.
    /// </summary>
    public Piece GetNonPlacedWhitePiece ()
    {
        foreach (Piece piece in m_WhitePieces)
        {
            if (!piece.placed)
            {
                return piece;
            }
        }

        if (LastpieceIsPlaced (PieceType.WHITE))
            HasWhitePieces = false;

        return null;
    }

    /// <summary>
    /// Returns the first non placed Black piece avaiable.null Returns null if none.
    /// </summary>
    public Piece GetNonPlacedBlackPiece ()
    {
        foreach (Piece piece in m_BlackPieces)
        {
            if (!piece.placed)
            {
                return piece;
            }
        }

        if (LastpieceIsPlaced (PieceType.BLACK))
            HasBlackPieces = false;

        return null;
    }

    private bool LastpieceIsPlaced (PieceType type)
    {
        if (type == PieceType.WHITE)
        {
            return m_WhitePieces[(m_WhitePieces.Count - 1)].placed;
        }
        else
        {
            return m_BlackPieces[(m_BlackPieces.Count - 1)].placed;
        }
    }

    public void UpdatePieces ()
    {
        foreach (TileField tile in m_Fields)
        {
            tile.CheckForPiece ();
        }
    }

    public void DehighlightAll ()
    {
        foreach (TileField tile in m_Fields)
        {
            if (tile.highlighting)
            {
                tile.DehighlightAdjacents();
            }
        }
    }

    public void VerifyAllPieces ()
    {
        foreach (Piece piece in m_WhitePieces)
        {
            piece.VerifyAll ();
        }

        foreach (Piece piece in m_BlackPieces)
        {
            piece.VerifyAll ();
        }
    }

    public void NewMovementOccured ()
    {
        foreach (Piece piece in m_Pieces)
        {
            if (piece != null)
            {
                //piece.turnCaptures = 0;
                piece.movemented = false;
                //piece.isCapturingSequence = false;
            }
        }
    }
}