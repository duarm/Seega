
using Seega.Enums;
using Seega.Types;

public interface IGameFinisher
{
    void VerifyCaptures(TileField currentField, Movement movement);
    void Surrender(ColorType winner);
    void VerifyWall(TileField currentField, Movement movement);
    void VerifyWall();
}
