using Seega.Enums;
using Seega.Types;

public interface IMovementValidator
{
    TileField SelectedField { get; }
    bool CanSelect (ColorType pieceColor, ColorType turnColor);
    void Deselect ();
    void Select (TileField field);
}