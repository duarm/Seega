using Seega.Enums;

public interface ITurnManager
{
    void NextTurn ();
    void Initialize (ColorType starter);
    bool IsWhiteTurn ();
    ColorType Turn { get; }
}