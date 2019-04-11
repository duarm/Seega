using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum FieldColor
{
    BLACK,
    WHITE
}

public class TileField : MonoBehaviour
{
    [Header("Tile Info")]
    public string column;
    public int line;
    public FieldColor fieldColor = FieldColor.BLACK;

    [Header("Debugging")]
    public bool highlighting = false;
    public Piece piece;

    TileField m_UpTile;
    TileField m_DownTile;
    TileField m_RightTile;
    TileField m_LeftTile;
    Board m_Board;
    MeshRenderer m_MeshRenderer;

     public bool CanMoveUp
    {
        get 
        { 
            if(m_UpTile != null) return UpTile.piece == null;
            return false;
        }
    }

    public bool CanMoveDown
    {
        get 
        { 
            if(m_DownTile != null) return m_DownTile.piece == null;
            return false;
        }
    }

    public bool CanMoveRight
    {
        get 
        { 
            if(RightTile != null) return RightTile.piece == null;
            return false;
        }
    }

    public bool CanMoveLeft
    {
        get 
        { 
            if(m_LeftTile != null) return m_LeftTile.piece == null;
            return false;
        }
    }

    public TileField UpTile
    {
        get { return m_UpTile; }
    }

    public TileField DownTile
    {
        get { return m_DownTile; }
    }

    public TileField RightTile
    {
        get { return m_RightTile; }
    }

    public TileField LeftTile
    {
        get { return m_LeftTile; }
    }

    private void Start() 
    {
        m_Board = Board.Instance;
        m_MeshRenderer = GetComponent<MeshRenderer>();
        Initialize();
    }

    private void OnMouseDown ()
    {
        if (Board.Instance.CurrentState == GameState.POSITIONING && !m_Board.IsUpdating)
        {
            if (piece != null)
                return;

            if (this.column == "c" && this.line == 3)
                return;

            if (m_Board.CurrentTurn == Turn.BLACK)
            {
                Piece piece = m_Board.GetNonPlacedBlackPiece();
                if (piece != null)
                {
                    //teleporting the piece to the right place
                    Vector3 move = new Vector3 (this.transform.localPosition.x, .5f, this.transform.localPosition.y);
                    piece.Teleport (move);
                }

                this.piece = piece;
            }
            else
            {
                Piece piece = Board.Instance.GetNonPlacedWhitePiece();
                if (piece != null)
                {
                    Vector3 move = new Vector3 (this.transform.localPosition.x, .5f, this.transform.localPosition.y);
                    piece.Teleport (move);
                }

                this.piece = piece;
            }

            Board.Instance.UpdatePlacedCounters ();
        }
    }

    private void Initialize()
    {
        m_UpTile = GetUpTile();
        m_DownTile = GetDownTile();
        m_RightTile = GetRightTile();
        m_LeftTile = GetLeftTile();
    }

    //Storing Adjacent Tiles
    private TileField GetLeftTile ()
    {
        RaycastHit hit;
        Ray ray = new Ray (transform.position, Vector3.left);
        if (Physics.Raycast (ray, out hit, 1, Board.Instance.whatIsTile))
            return hit.collider?.GetComponent<TileField>();

        return null;
    }

    private TileField GetRightTile ()
    {
        RaycastHit hit;
        Ray ray = new Ray (transform.position, Vector3.right);
        if (Physics.Raycast (ray, out hit, 1, Board.Instance.whatIsTile))
            return hit.collider?.GetComponent<TileField>();

        return null;
    }

    private TileField GetUpTile ()
    {
        RaycastHit hit;
        Ray ray = new Ray (transform.position, Vector3.forward);
        if (Physics.Raycast (ray, out hit, 1, Board.Instance.whatIsTile))
            return hit.collider?.GetComponent<TileField>();

        return null;
    }

    private TileField GetDownTile ()
    {
        RaycastHit hit;
        Ray ray = new Ray (transform.position, Vector3.back);
        if (Physics.Raycast (ray, out hit, 1, Board.Instance.whatIsTile))
            return hit.collider?.GetComponent<TileField>();

        return null;
    }

    //WORKING
    public void CheckForPiece()
    {
        RaycastHit hit;
        Ray ray = new Ray (this.transform.position, Vector3.up);
        if (Physics.Raycast (ray, out hit, 2, Board.Instance.whatIsPiece))
        {
            if (hit.collider != null)
            {
                this.piece = hit.collider.GetComponent<Piece> ();
            }
        }
        else
        {
            Debug.Log("No token found above " + this.gameObject.name);
            this.piece = null;
        }
    }

    public void RemovePiece ()
    {
        this.piece = null;
    }
    //END WORKING

    //CHANGE
    //Called when a piece is moved to this tile
    public IEnumerator PieceMoved ()
    {
        m_Board.IsUpdating = true;
        m_Board.NewMovementOccured ();

        m_Board.HighlightedField.piece.MoveTo (this);
        m_Board.DehighlightAll ();

        yield return new WaitForSeconds (.7f);
        CheckForPiece ();

        m_Board.HighlightedField = this;

        if (column == "c" && line == 3)
            piece.inMiddle = true;
        else
            piece.inMiddle = false;

        Board.Instance.UpdateBoard ();
    }

    //Highlight all empty Adjacent Tiles that has no Piece, in order to show which tile is currently avaiable to move
    public void HighlightEmptyAdjacents ()
    {
        m_Board.DehighlightAll ();

        highlighting = true;
        m_Board.HighlightedField = this;

        if(CanMoveUp)
        {
            if(m_UpTile.fieldColor == FieldColor.WHITE)
                m_UpTile.Highlight();
            else
                m_UpTile.Highlight(false);
        }

        if(CanMoveDown)
        {
            if(m_DownTile.fieldColor == FieldColor.WHITE)
                m_DownTile.Highlight();
            else
                m_DownTile.Highlight(false);
        }

        if(CanMoveRight)
        {
            if(m_RightTile.fieldColor == FieldColor.WHITE)
                m_RightTile.Highlight();
            else
                m_RightTile.Highlight(false);
        }

        if(CanMoveLeft)
        {
            if(m_LeftTile.fieldColor == FieldColor.WHITE)
                m_LeftTile.Highlight();
            else
                m_LeftTile.Highlight(false);
        }
    }

    //Dehighlight all adjacent tiles
    public void DehighlightAdjacents ()
    {
        this.highlighting = false;
        m_Board.HighlightedField = null;

        if(CanMoveUp)
        {
            if(m_UpTile.fieldColor == FieldColor.WHITE)
                m_UpTile.Dehighlight();
            else
                m_UpTile.Dehighlight(false);
        }

        if(CanMoveDown)
        {
            if(m_DownTile.fieldColor == FieldColor.WHITE)
                m_DownTile.Dehighlight();
            else
                m_DownTile.Dehighlight(false);
        }

        if(CanMoveRight)
        {
            if(m_RightTile.fieldColor == FieldColor.WHITE)
                m_RightTile.Dehighlight();
            else
                m_RightTile.Dehighlight(false);
        }

        if(CanMoveLeft)
        {
            if(m_LeftTile.fieldColor == FieldColor.WHITE)
                m_LeftTile.Dehighlight();
            else
                m_LeftTile.Dehighlight(false);
        }
    }

    //highlighting this tile by changing its material
    private void Highlight(bool isWhite = true)
    {
        if(isWhite)
            m_MeshRenderer.material = m_Board.whiteHighlightMaterial;
        else
            m_MeshRenderer.material = m_Board.blackHighlightMaterial;
    }

    //dehighlighting this tile by changing its material
    private void Dehighlight(bool isWhite = true)
    {
        if(isWhite)
            m_MeshRenderer.material = m_Board.whiteNormalMaterial;
        else
            m_MeshRenderer.material = m_Board.blackNormalMaterial;
    }
}