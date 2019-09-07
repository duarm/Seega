using Seega.Scripts.Types;
using UnityEngine;

namespace Seega.Scripts.Core
{
    public class TileField : MonoBehaviour
    {
        public Coordinates Coordinates { get { return coordinates; } }
        public Piece Piece { get; private set; }
        public bool IsHighlighting { get { return _selectable.IsSelected (); } }

        ISelectable _selectable;
        Coordinates coordinates;
        
        private void Start()
        {
            _selectable = GetComponent<ISelectable>();
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
            _selectable.OnSelect ();
        }

        public void OnDeselect ()
        {
            _selectable.OnDeselect ();
        }
    }
}