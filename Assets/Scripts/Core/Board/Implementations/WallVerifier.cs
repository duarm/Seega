using Seega.Enums;
using Seega.Types;
using UnityEngine;
using Zenject;

public class WallVerifier : MonoBehaviour, IWallVerifier
{
    private IFieldProvider _fieldProvider;

    [Inject]
    private void Construct (IFieldProvider fieldProvider)
    {
        _fieldProvider = fieldProvider;
    }

    //TODO: BUG, CHANGE ALGORITHM
    private bool HasVerticalWall (TileField currentField)
    {
        var fields = _fieldProvider.GetField ();
        for (int i = 0; i < 5; i++)
        {
            int positiveIndex = currentField.Coordinates.x + i;
            if (positiveIndex <= 4)
            {
                if (!(currentField.Piece.type == fields[positiveIndex, currentField.Coordinates.y].Piece?.type))
                {
                    return false;
                }
            }

            int negativeIndex = currentField.Coordinates.x - i;
            if (negativeIndex >= 0)
            {
                if (!(currentField.Piece.type == fields[negativeIndex, currentField.Coordinates.y].Piece?.type))
                {
                    return false;
                }
            }
        }

        return true;
    }

    //TODO: BUG, CHANGE ALGORITHM
    private bool HasHorizontalWall (TileField currentField)
    {
        var fields = _fieldProvider.GetField ();
        for (int i = 0; i < 5; i++)
        {
            int positiveIndex = currentField.Coordinates.y + i;
            if (positiveIndex <= 4)
            {
                if (!(currentField.Piece.type == fields[currentField.Coordinates.x, positiveIndex].Piece?.type))
                {
                    return false;
                }
            }

            int negativeIndex = currentField.Coordinates.y - i;
            if (negativeIndex >= 0)
            {
                if (!(currentField.Piece.type == fields[currentField.Coordinates.x, negativeIndex].Piece?.type))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool HasWallOnField (out ColorType? wallType)
    {
        var fields = _fieldProvider.GetField ();
        for (int i = 0; i < fields.XLength; i++)
        {
            var fieldToCheck = fields[i, 0];
            if (HasVerticalWall (fieldToCheck))
            {
                wallType = fieldToCheck.Piece.type;
                return true;
            }
        }

        for (int y = 1; y < fields.YLength; y++)
        {
            var fieldToCheck = fields[0, y];
            if (HasHorizontalWall (fieldToCheck))
            {
                wallType = fieldToCheck.Piece.type;
                return true;
            }
        }

        wallType = null;
        return false;
    }

    public bool HasWall (TileField currentField, Movement movement)
    {
        //If the piece moved vertically, no need to check for a vertical wall
        if (movement.Vertical)
        {
            if (HasHorizontalWall (currentField))
                return true;
        }
        else
        {
            if (HasVerticalWall (currentField))
                return true;
        }

        return false;
    }
}