using Kurenaiz.Utilities.Types;
using UnityEngine;

namespace Seega.Scripts.Core
{
    public class FieldProviderWithForeach : MonoBehaviour, IFieldProvider
    {
        public Transform tileFieldParent;
        
        public Safe2DArray CreateField()
        {
            var row = 0;
            var column = 0;
            var isWhite = true;
            var fields = new Safe2DArray(5,5);
            foreach (Transform field in tileFieldParent)
            {
                if (row == 5)
                {
                    column++;
                    row = 0;
                }

                fields[row, column] = field.GetComponent<TileField>().Initialize(row, column, isWhite);
                isWhite = !isWhite;
                row++;
            }

            return fields;
        }
    }
}