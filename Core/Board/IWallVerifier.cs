using Kurenaiz.Utilities.Types;

public interface IWallVerifier
{
    bool HasVerticalWall (Safe2DArray fields, TileField currentField, bool isWhiteTurn);
    bool HasHorizontalWall(Safe2DArray fields, TileField currentField, bool isWhiteTurn);
}