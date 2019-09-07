using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class SceneController : MonoBehaviour
{
    AsyncOperation async;
    private ScreenFader _screenFader;

    [Inject]
    private void Construct(ScreenFader screenFader){
        _screenFader = screenFader;
    }

    void Awake ()
    {
        DontDestroyOnLoad (gameObject);
    }
    
    public void RestartScene ()
    {
        if (_screenFader.IsFading)
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
		yield return StartCoroutine(_screenFader.FadeSceneOut());
		text.SetActive(false);

		_screenFader.SetAlpha(0);
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
		yield return StartCoroutine(_screenFader.FadeSceneOut());
		yield return SceneManager.LoadSceneAsync ("Game");
		yield return StartCoroutine(_screenFader.FadeSceneIn());
	}
}