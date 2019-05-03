using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Kurenaiz.Utilities.Physics;
using Kurenaiz.Utilities.Types;

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

    public bool IsHighlighting
    {
        get { return m_HighlightedTile != null; }
    }

    public TileField HighlightedField
    {
        get{ return m_HighlightedTile; }
        set{ m_HighlightedTile = value; }
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

    public delegate void OnTurnChange ();
    public OnTurnChange onTurnChangeCallback;

    public delegate void OnStateChange ();
    public OnStateChange onStateChangeCallback;

    public delegate void OnGameEnd (string winner, string winType);
    public OnGameEnd onGameEndCallback;

    void Awake ()
    {
        s_BoardInstance = this;
    }

    void Start ()
    {
        m_CurrentGameState = GameState.STARTING;
        m_FieldUpdateRate = new WaitForSeconds (fieldUpdateTime);
        PopulateArrays();
    }

    //IMPLEMENT COMPONENT CACHING
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
                    // Debug.Log("Clicked  Tiled: " + tile.name);
                    // Debug.Log("Clicked Tile Ray: " + tile.ToString());
                    // Debug.Log("Clicked Tile Array: " + m_Fields[tile.Row, tile.Column].ToString());

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
                            //Debug.Log(m_HighlightedTile);
                            //Debug.Log("Calling Piece Moved of :" + tile.gameObject.name);
                            //MOVE PIECE
                            if(tile.highlighting)
                            {
                                //If this tile is highlighted, we can move to this tile.
                            }
                        }
                    }
                }
                else if (m_CurrentGameState == GameState.POSITIONING)
                {
                    //Debug.Log("Clicked Tile Ray: " + tile.ToString());
                    //Debug.Log("Clicked Tile Array: " + tile.ToString());
                    if(tile.Piece != null)
                        return;

                    //No piece can be put in the middle on the positioning phase
                    if(tile.Row == 2 && tile.Column == 2)
                        return;

                    if (m_IsWhiteTurn)
                        tile.Piece = GetNonPlacedWhitePiece();
                    else
                        tile.Piece = GetNonPlacedBlackPiece();

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

    private void Dehighlight()
    {
        Debug.Log("Dehighlighting");
        Debug.Log(m_HighlightedTile);

        TileField field;
        field = m_Fields[m_HighlightedTile.Row + 1, m_HighlightedTile.Column];
        if(field?.highlighting == true)
        {
            Debug.Log(field.ToString());
            field.Dehighlight();
        }

        field = m_Fields[m_HighlightedTile.Row - 1, m_HighlightedTile.Column];
        if(field?.highlighting == true)
        {
            Debug.Log(field.ToString());
            field.Dehighlight();
        }

        field = m_Fields[m_HighlightedTile.Row, m_HighlightedTile.Column + 1];
        if(field?.highlighting == true)
        {
            Debug.Log(field.ToString());
            field.Dehighlight();
        }

        field = m_Fields[m_HighlightedTile.Row, m_HighlightedTile.Column - 1];
        if(field?.highlighting == true)
        {
            Debug.Log(field.ToString());
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
        foreach (Transform field in tileFieldParent)
        {
            if(row == 5)
            {
                column++;
                row=0;
            }

            m_Fields[row,column] = field.GetComponent<TileField>().Initialize(row, column);
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

    private bool ValidateMovement (TileField moveField)
    {
        

        return false;
    }

    public void UpdateBoard ()
    {
        //StartCoroutine (Updater ());
    }

    /*
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
        /*
        if (m_HighlightedTile.piece.turnCaptures == 0)
        {
            NextTurn ();
        }
        else
        {
            m_HighlightedTile.HighlightEmptyAdjacents ();
            m_HighlightedTile.piece.isCapturingSequence = true;
        }


        isUpdating = false;
    }*/

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

    public void NextTurn ()
    {
        if (m_IsWhiteTurn)
            m_IsWhiteTurn = false;
        else
            m_IsWhiteTurn = true;
            
        m_HighlightedTile = null;

        if (onTurnChangeCallback != null)
            onTurnChangeCallback.Invoke ();
    }

    public void StartPositioningPhase ()
    {
        m_CurrentGameState = GameState.POSITIONING;

        if (onStateChangeCallback != null)
            onStateChangeCallback.Invoke ();

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
    private Piece GetNonPlacedBlackPiece ()
    {
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

    public void DehighlightAll ()
    {
        foreach (TileField piece in m_Fields)
        {

        }
    }

    /*
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
    */
}