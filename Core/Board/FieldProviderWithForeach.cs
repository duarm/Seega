using Kurenaiz.Utilities.Types;
using UnityEngine;

public class FieldProviderWithForeach : MonoBehaviour, IFieldProvider
{
    public Transform tileFieldParent;
    private Safe2DArray _fields;

    private void Start ()
    {
        CreateField ();
    }

    private void CreateField ()
    {
        var row = 0;
        var column = 0;
        var isWhite = true;
        _fields = new Safe2DArray (5, 5);
        foreach (Transform field in tileFieldParent)
        {
            if (row == 5)
            {
                column++;
                row = 0;
            }

            _fields[row, column] = field.GetComponent<TileField> ().Initialize (row, column, isWhite);
            isWhite = !isWhite;
            row++;
        }
    }

    Safe2DArray IFieldProvider.GetField ()
    {
        return _fields;
    }
}