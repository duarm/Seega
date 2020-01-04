using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoardRenderer : MonoBehaviour
{
    /*
    [SerializeField] Transform[] whitePieces;
    [SerializeField] int numbersBeforeRejection = 30;
    [SerializeField] float radius = .5f;
    [SerializeField] Vector2 area = new Vector2 (2, 2);*/

    [Header ("Board")]
    [SerializeField] bool chess = true;
    [SerializeField] Vector2Int boardSize = new Vector2Int (5, 5);
    [SerializeField] Vector2 spacing;
    [SerializeField] Vector2 offset;
    [Header ("Cell")]
    [SerializeField] Sprite sprite;
    [SerializeField] Vector2 cellSize;
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject prefabVariant;
    [SerializeField] float scaleFactor = 5;
    int oldCellCount = 0;
    bool inverter = false;

    private void OnValidate ()
    {
        /*var positions = PoissonDiscSampling.GeneratePoints (radius, area, numbersBeforeRejection);
        for (int i = 0; i < whitePieces.Length; i++)
        {
            if(i > positions.Count)
            {
                Debug.LogWarning("Could not fit all the points in the given area, lower the radius or increase the area.");
                break;
            }
            whitePieces[i].position = positions[i];
        }*/
    }

    public void Build ()
    {

        var temp = new GameObject[transform.childCount];

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = transform.GetChild (i).gameObject;
        }

        foreach (var child in temp)
        {
            SafeDestroy (child.gameObject);
        }

        for (int x = 0; x < boardSize.x; x++)
        {
            var xPos = x * (cellSize.x + spacing.x) + offset.x;

            for (int y = 0; y < boardSize.y; y++)
            {
                var yPos = y * (cellSize.y + spacing.y) + offset.y;
                var cell = chess && inverter ? Instantiate (prefabVariant, transform) : Instantiate (prefab, transform);
                inverter = !inverter;
                cell.name = $"{x}:{y}";
                cell.transform.localScale = cellSize;
                cell.transform.position = new Vector2 (xPos, yPos) / scaleFactor;
            }
        }
    }

    public static T SafeDestroy<T> (T obj) where T : Object
    {
        if (Application.isEditor)
            Object.DestroyImmediate (obj);
        else
            Object.Destroy (obj);

        return null;
    }
    public static T SafeDestroyGameObject<T> (T component) where T : Component
    {
        if (component != null)
            SafeDestroy (component.gameObject);
        return null;
    }
}