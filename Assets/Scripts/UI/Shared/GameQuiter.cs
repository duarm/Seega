using UnityEngine;

public class GameQuiter : MonoBehaviour
{
    public void QuitGame ()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }
}