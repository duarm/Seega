using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{       
    #region Singleton
    public static ScreenFader Instance
    {
        get
        {
            if (s_Instance != null)
                return s_Instance;

            s_Instance = FindObjectOfType<ScreenFader> ();

            if (s_Instance != null)
                return s_Instance;

            Create ();

            return s_Instance;
        }
    }

    protected static ScreenFader s_Instance;

    public static void Create ()
    {
        GameObject screenFaderGameObject = new GameObject("SceneFader");
        s_Instance = screenFaderGameObject.AddComponent<ScreenFader>();
    }
    #endregion

    public enum FadeType
    {
        Black
    }
    
    public static bool IsFading
    {
        get { return Instance.m_IsFading; }
    }

    public CanvasGroup faderCanvasGroup;

    protected bool m_IsFading;

    const int k_MaxSortingLayer = 32767;

    void Awake ()
    {
        if (Instance != this)
        {
            Destroy (gameObject);
            return;
        }
    
        DontDestroyOnLoad (gameObject);
    }

	public IEnumerator Restart()
	{
		yield return ScreenFader.FadeSceneOut(1, 1);
		yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
		yield return ScreenFader.FadeSceneIn(0,.5f);
	}

    protected IEnumerator Fade(float finalAlpha, CanvasGroup canvasGroup, float fadeDuration)
    {
        m_IsFading = true;
        canvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        canvasGroup.alpha = finalAlpha;
        m_IsFading = false;
        canvasGroup.blocksRaycasts = false;
    }

    public static void SetAlpha (float alpha)
    {
        Instance.faderCanvasGroup.alpha = alpha;
    }

    /// <summary>
    /// Fade the scene in. 
    /// From black(alpha 1) to transparent(alpha 0).
    /// </summary>
    /// <param name="finalAlpha">The final value of alpha. default 0.</param>
    /// <param name="fadeDuration">The duration of the fade. default .5f.</param>
    public static IEnumerator FadeSceneIn (float finalAlpha = 0f, float fadeDuration = .5f)
    {
        CanvasGroup canvasGroup;
        canvasGroup = Instance.faderCanvasGroup;
        
        yield return Instance.StartCoroutine(Instance.Fade(finalAlpha, canvasGroup, fadeDuration));

        if(finalAlpha == 0)
            canvasGroup.gameObject.SetActive (false);
    }

    /// <summary>
    /// Fade the scene out.
    /// From transparent(alpha 0) to black(alpha 1)
    /// </summary>
    /// <param name="finalAlpha">The final value of alpha. default 1.</param>
    /// <param name="fadeDuration">The duration of the fade. default .5f.</param>
    /// <param name="fadeType">The type of the fade. default FadeType.Black</param>
    public static IEnumerator FadeSceneOut (float finalAlpha = 1f, float fadeDuration = .5f, FadeType fadeType = FadeType.Black)
    {
        CanvasGroup canvasGroup;
        canvasGroup = Instance.faderCanvasGroup;
        
        canvasGroup.gameObject.SetActive (true);
        
        yield return Instance.StartCoroutine(Instance.Fade(finalAlpha, canvasGroup, fadeDuration));
    }
}