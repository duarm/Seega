using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (Transform)), CanEditMultipleObjects]
public class AutoPlaceObject : Editor
{
    private Event current;

    public void OnSceneGUI ()
    {
        current = Event.current;
        if (!current.isKey || current.type != EventType.KeyDown)
            return;

        switch (current.keyCode)
        {
            case KeyCode.Keypad0:
                SnapToSurface ();
                break;
        }
    }

    public void SnapToSurface ()
    {
        if (Selection.transforms.Length == 1)
        {
            var selected = Selection.activeGameObject;
            RaycastHit hit;
            Ray ray = new Ray (selected.transform.position, Vector3.down);
            if (Physics.Raycast (ray, out hit, Mathf.Infinity))
            {
                selected.transform.position = new Vector3 (selected.transform.position.x, hit.collider.transform.position.y, selected.transform.position.z);
                selected.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);
                //Debug.Log (hit.collider.transform.position);
            }
        }
        else if (Selection.transforms.Length > 1)
        {
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                RaycastHit hit;
                Ray ray = new Ray (Selection.transforms[i].position, Vector3.down);
                if (Physics.Raycast (ray, out hit, Mathf.Infinity))
                {
                    Selection.transforms[i].position = new Vector3 (Selection.transforms[i].position.x, hit.collider.transform.position.y, Selection.transforms[i].position.z);
                    Selection.transforms[i].rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);
                    //Debug.Log (hit.collider.transform.position);
                }
            }
        }
    }
}