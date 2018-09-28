using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    #region Singleton
    public static SceneController Instance
    {
        get
        {
            if (s_Instance != null)
                return s_Instance;

            s_Instance = FindObjectOfType<SceneController> ();

            if (s_Instance != null)
                return s_Instance;

            Create ();

            return s_Instance;
        }
    }

    protected static SceneController s_Instance;

    public Button restartButton;

    public static void Create ()
    {
        GameObject screenFaderGameObject = new GameObject ("SceneController");
        s_Instance = screenFaderGameObject.AddComponent<SceneController> ();
    }

    void Awake ()
    {
        if (Instance != this)
        {
            Destroy (gameObject);
            return;
        }

        DontDestroyOnLoad (gameObject);
    }
    #endregion

    public void RestartScene ()
    {
        if (ScreenFader.IsFading)
            return;

        StartCoroutine (Restart ());
    }

    private IEnumerator Restart ()
    {
        yield return StartCoroutine (ScreenFader.FadeSceneOut (1, 1));
        yield return SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().name);
        yield return StartCoroutine (ScreenFader.FadeSceneIn (0, 1));
    }
}