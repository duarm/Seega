
using Seega.Enums;

public interface IGameFinisher
{
    void TotalVictory(ColorType winner);
    void GreatVictory(ColorType winner);
    void MinorVictory(ColorType winner);
}
