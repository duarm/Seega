using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneController : MonoBehaviour
{
    AsyncOperation async;
    private ScreenFader _screenFader;

    [Inject]
    private void Construct (ScreenFader screenFader)
    {
        _screenFader = screenFader;
    }

    public void RestartScene ()
    {
        if (_screenFader.IsFading)
            return;

        StartCoroutine (Restart ());
    }

    public IEnumerator LoadLevel ()
    {
        yield return StartCoroutine (_screenFader.FadeSceneOut ());
        SceneManager.LoadSceneAsync ("Game");
        yield return StartCoroutine (_screenFader.FadeSceneIn ());
    }

    IEnumerator Restart ()
    {
        yield return StartCoroutine (_screenFader.FadeSceneOut ());
        yield return SceneManager.LoadSceneAsync ("Game");
        yield return StartCoroutine (_screenFader.FadeSceneIn ());
    }
}