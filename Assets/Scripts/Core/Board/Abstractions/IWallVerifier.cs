using Seega.Enums;
using Seega.Types;

public interface IWallVerifier
{
    bool HasWallOnField(out ColorType? wallType);
    bool HasWall(TileField currentField, Movement movement);
}