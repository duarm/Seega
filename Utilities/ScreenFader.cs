using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Unity 2d game kit Screen Fader
public class ScreenFader : MonoBehaviour
{
    void Awake ()
    {

        DontDestroyOnLoad (gameObject);
    }

    public enum FadeType
    {
        Black
    }

    public bool IsFading
    {
        get { return m_IsFading; }
    }

    public CanvasGroup faderCanvasGroup;

    private bool m_IsFading;

    const int k_MaxSortingLayer = 32767;

    protected IEnumerator Fade (float finalAlpha, CanvasGroup canvasGroup, float fadeDuration)
    {
        m_IsFading = true;
        canvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs (canvasGroup.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately (canvasGroup.alpha, finalAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards (canvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        canvasGroup.alpha = finalAlpha;
        m_IsFading = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void SetAlpha (float alpha)
    {
        faderCanvasGroup.alpha = alpha;
    }

    /// <summary>
    /// Fade the scene in. 
    /// From black(alpha 1) to transparent(alpha 0).
    /// </summary>
    /// <param name="finalAlpha">The final value of alpha. default 0.</param>
    /// <param name="fadeDuration">The duration of the fade. default .5f.</param>
    public IEnumerator FadeSceneIn (float finalAlpha = 0f, float fadeDuration = .5f)
    {
        CanvasGroup canvasGroup;
        canvasGroup = faderCanvasGroup;

        Debug.Log ("Fading in");
        yield return StartCoroutine (Fade (finalAlpha, canvasGroup, fadeDuration));

        if (finalAlpha == 0)
            canvasGroup.gameObject.SetActive (false);
    }

    /// <summary>
    /// Fade the scene out.
    /// From transparent(alpha 0) to black(alpha 1)
    /// </summary>
    /// <param name="finalAlpha">The final value of alpha. default 1.</param>
    /// <param name="fadeDuration">The duration of the fade. default .5f.</param>
    /// <param name="fadeType">The type of the fade. default FadeType.Black</param>
    public IEnumerator FadeSceneOut (float finalAlpha = 1f, float fadeDuration = .5f, FadeType fadeType = FadeType.Black)
    {
        CanvasGroup canvasGroup;
        canvasGroup = faderCanvasGroup;

        canvasGroup.gameObject.SetActive (true);

        yield return StartCoroutine (Fade (finalAlpha, canvasGroup, fadeDuration));
    }
}