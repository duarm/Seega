public interface IPieceProvider
{
    Piece GetNonPlacedBlackPiece ();
    Piece GetNonPlacedWhitePiece ();
    int PieceCount { get; }
}