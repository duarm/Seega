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

    AsyncOperation async;

    public void RestartScene ()
    {
        if (ScreenFader.IsFading)
            return;

        StartCoroutine (Restart());
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public IEnumerator LoadLevel(Slider slider, GameObject text, GameObject loadBar)
	{
		yield return StartCoroutine(ScreenFader.FadeSceneOut());
		text.SetActive(false);

		ScreenFader.SetAlpha(0);
		loadBar.SetActive(true);
		async = SceneManager.LoadSceneAsync(1);
		async.allowSceneActivation = false;

		while(!async.isDone)
		{
			slider.value = async.progress;
			if(async.progress == 0.9f)
			{
				slider.value = 1f;
				async.allowSceneActivation = true;
			}
			yield return null;
		}
	}

    IEnumerator Restart()
	{
		yield return StartCoroutine(ScreenFader.FadeSceneOut());
		yield return SceneManager.LoadSceneAsync ("Game");
		yield return StartCoroutine(ScreenFader.FadeSceneIn());
	}
}