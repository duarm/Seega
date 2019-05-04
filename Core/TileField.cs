using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileField : MonoBehaviour
{
    public bool highlighting = false;

    bool isWhite;
    Board m_Board;
    MeshRenderer m_MeshRenderer;

    public int Row { get; private set; }
    public int Column { get; private set; }
    public Piece Piece { get; private set; }

    private void Start() 
    {
        m_Board = Board.Instance;
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }

    public override string ToString()
    {
        return "Row: " + Row + " Column: " + Column;
    }

    public TileField Initialize(int row, int column, bool isWhite)
    {
        this.Row= row;
        this.Column = column;
        this.isWhite = isWhite;
        return this;
    }

    public void SetPiece(Piece piece)
    {
        this.Piece = piece;
        Piece.Teleport(this); 
    }

    public void MoveFrom(TileField field)
    {
        Piece = field.Piece;
        Piece.MoveTo(this);
    }

    public void MovePieceTo(TileField field)
    {
        field.MoveFrom(this);
        Piece = null;
    }

    public void Capture()
    {
        //Play Sound
        Piece.Capture();
        Piece = null;
    }

    //highlighting this tile by changing its material
    public void Highlight()
    {
        highlighting = true;
        if(isWhite)
            m_MeshRenderer.material = m_Board.whiteHighlightMaterial;
        else
            m_MeshRenderer.material = m_Board.blackHighlightMaterial;
    }

    //dehighlighting this tile by changing its material
    public void Dehighlight()
    {
        highlighting = false;
        if(isWhite)
            m_MeshRenderer.material = m_Board.whiteNormalMaterial;
        else
            m_MeshRenderer.material = m_Board.blackNormalMaterial;
    }
}