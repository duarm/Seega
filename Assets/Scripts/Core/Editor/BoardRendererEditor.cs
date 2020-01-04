using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (BoardRenderer))]
public class BoardRendererEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        DrawDefaultInspector ();

        var script = (BoardRenderer) target;

        if (GUILayout.Button ("Rebuild"))
        {
            script.Build ();
        }
    }
}