using Seega.Enums;

public interface ITurnManager
{
    void NextTurn();
    void Start(ColorType starter);
    bool IsWhiteTurn();
    ColorType GetCurrentTurn();
}