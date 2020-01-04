using Kurenaiz.Utilities.Types;
using Seega.Enums;
using Seega.Types;
using UnityEngine;
using Zenject;

public class CaptureVerifier : MonoBehaviour, ICaptureVerifier
{
    private IFieldProvider _fieldProvider;
    private IGameFinisher _gameFinisher;

    [Inject]
    private void Construct (IFieldProvider fieldProvider, IGameFinisher gameFinisher)
    {
        _fieldProvider = fieldProvider;
        _gameFinisher = gameFinisher;
    }
    
    int ICaptureVerifier.VerifyCapture (TileField currentField, Movement movement)
    {
        var kills = 0;
        var fields = _fieldProvider.GetField();
        var coordinates = new Coordinates (currentField);
        Coordinates[] coordsToVerify = { coordinates.up, coordinates.right, coordinates.down, coordinates.left };

        for (int i = 0; i < coordsToVerify.Length; i++)
        {
            //Verifying the piece adjacent to the currentField
            //[farTile][inBetweenTile][currentTile]
            var farCoordinates = coordsToVerify[i];
            var inBetweenTile = fields[coordsToVerify[i]];

            if (currentField.Coordinates.x > coordsToVerify[i].x)
                farCoordinates.x--;
            else if (currentField.Coordinates.x < coordsToVerify[i].x)
                farCoordinates.x++;
            else if (currentField.Coordinates.y < coordsToVerify[i].y)
                farCoordinates.y++;
            else if (currentField.Coordinates.y > coordsToVerify[i].y)
                farCoordinates.y--;

            var farTile = fields[farCoordinates];

            //Verifying the adjacent tile is an enemy
            if (inBetweenTile != null &&
                inBetweenTile.Piece != null &&
                inBetweenTile.Piece.type != currentField.Piece.type)
            {
                //Veryfying if the far tile is an ally
                if (farTile != null &&
                    farTile.Piece != null &&
                    farTile.Piece.type == currentField.Piece.type)
                {
                    //if the inBetweenPiece is on the middle, it can't be captured
                    if (inBetweenTile.Coordinates.x != 2 ||
                        inBetweenTile.Coordinates.y != 2)
                    {
                        inBetweenTile.Capture ();
                        kills++;
                    }
                }
            }
        }

        return kills;
    }
}