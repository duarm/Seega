using Seega.Enums;
using Seega.Types;
using UnityEngine;
using Zenject;

public class MovementValidator : MonoBehaviour, IMovementValidator
{
    TileField _selectedField; //Keep track of the current selected piece
    private IFieldProvider _fieldProvider;

    [Inject]
    private void Construct (IFieldProvider fieldProvider)
    {
        _fieldProvider = fieldProvider;
    }

    private bool TryHighlight (TileField tile)
    {
        if (tile != null && tile.Piece == null)
        {
            tile.OnSelect ();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if the clicked piece corresponds to this turn
    /// </summary>
    public bool CanSelect (ColorType pieceColor, ColorType turnColor)
    {
        return pieceColor == turnColor;
    }

    public TileField SelectedField => _selectedField;

    void IMovementValidator.Deselect ()
    {
        if (_selectedField == null)
            return;

        TileField field;
        var coor = new Coordinates (_selectedField);
        var fields = _fieldProvider.GetField ();
        foreach (var dir in coor.cardinal4)
        {
            field = fields[dir];
            if (field != null && field.IsSelected)
                field.OnDeselect ();
        }

        _selectedField = null;
    }

    void IMovementValidator.Select (TileField tile)
    {
        var coor = new Coordinates (tile);
        var fields = _fieldProvider.GetField ();
        var highlighted = false;

        foreach (var dir in coor.cardinal4)
        {
            var field = fields[dir];
            if (TryHighlight (field))
                highlighted = true;
        }

        if (highlighted)
            _selectedField = tile;
    }
}