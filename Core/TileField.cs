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
    public bool highlighting = false;

    Board m_Board;
    MeshRenderer m_MeshRenderer;
    Piece m_Piece;

    public int Row { get; private set; }
    public int Column { get; private set; }

    public Piece Piece 
    {
        get { return m_Piece; } 
        set 
        { 
            m_Piece = value;
            m_Piece.Teleport(this); 
        }
    }

    public override string ToString()
    {
        return "Row: " + Row + " Column: " + Column;
    }

    private void Start() 
    {
        m_Board = Board.Instance;
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }

    public TileField Initialize(int row, int column)
    {
        this.Row= row;
        this.Column = column;
        return this;
    }

    public void MoveFrom()
    {

    }

    public void MoveTo()
    {
        
    }

    //highlighting this tile by changing its material
    public void Highlight(bool isWhite = true)
    {
        highlighting = true;
        if(isWhite)
            m_MeshRenderer.material = m_Board.whiteHighlightMaterial;
        else
            m_MeshRenderer.material = m_Board.blackHighlightMaterial;
    }

    //dehighlighting this tile by changing its material
    public void Dehighlight(bool isWhite = true)
    {
        highlighting = false;
        if(isWhite)
            m_MeshRenderer.material = m_Board.whiteNormalMaterial;
        else
            m_MeshRenderer.material = m_Board.blackNormalMaterial;
    }
}