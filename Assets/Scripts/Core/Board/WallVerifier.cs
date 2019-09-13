using Kurenaiz.Utilities.Types;
using Seega.Enums;
using UnityEngine;

public class WallVerifier : MonoBehaviour, IWallVerifier
{
    //TODO: BUG, CHANGE ALGORITHM
    bool IWallVerifier.HasVerticalWall (Safe2DArray fields, TileField currentField, bool isWhiteTurn)
    {
        var currentPieceType = isWhiteTurn ? ColorType.WHITE : ColorType.BLACK;
        for (int i = 0; i < 5; i++)
        {
            int positiveIndex = currentField.Coordinates.x + i;
            if (positiveIndex <= 4)
            {
                if (!(currentPieceType == fields[positiveIndex, currentField.Coordinates.y].Piece?.type))
                {
                    return false;
                }
            }

            int negativeIndex = currentField.Coordinates.x - i;
            if (negativeIndex >= 0)
            {
                if (!(currentPieceType == fields[negativeIndex, currentField.Coordinates.y].Piece?.type))
                {
                    return false;
                }
            }
        }

        return true;
    }

    //TODO: BUG, CHANGE ALGORITHM
    bool IWallVerifier.HasHorizontalWall (Safe2DArray fields, TileField currentField, bool isWhiteTurn)
    {
        var currentPieceType = isWhiteTurn ? ColorType.WHITE : ColorType.BLACK;
        for (int i = 0; i < 5; i++)
        {
            int positiveIndex = currentField.Coordinates.y + i;
            if (positiveIndex <= 4)
            {
                if (!(currentPieceType == fields[currentField.Coordinates.x, positiveIndex].Piece?.type))
                {
                    return false;
                }
            }

            int negativeIndex = currentField.Coordinates.y - i;
            if (negativeIndex >= 0)
            {
                if (!(currentPieceType == fields[currentField.Coordinates.x, negativeIndex].Piece?.type))
                {
                    return false;
                }
            }
        }

        return true;
    }
}