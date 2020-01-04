using Seega.Types;
using UnityEngine;

public class TileField : MonoBehaviour
{
    public Coordinates Coordinates { get { return coordinates; } }
    public Piece Piece { get; private set; }
    public bool IsSelected { get { return selected; } }

    bool selected = false;
    Coordinates coordinates;
    ISelectable _selectable;

    private void Start ()
    {
        _selectable = GetComponent<ISelectable> ();
    }

    public TileField Initialize (int row, int column, bool isWhite)
    {
        this.coordinates.x = row;
        this.coordinates.y = column;
        return this;
    }

    public void SetPiece (Piece piece)
    {
        this.Piece = piece;
        Piece.Teleport (this);
    }

    private void MoveFrom (TileField field)
    {
        Piece = field.Piece;
        Piece.MoveTo (this);
    }

    public void MovePieceTo (TileField field)
    {
        //TODO: Play Sound
        field.MoveFrom (this);
        Piece = null;
    }

    public void Capture ()
    {
        //TODO: Play Sound
        Piece.Capture ();
        Piece = null;
    }

    public void OnSelect ()
    {
        selected = true;
        _selectable?.OnSelect ();
    }

    public void OnDeselect ()
    {
        selected = false;
        _selectable?.OnDeselect ();
    }
}