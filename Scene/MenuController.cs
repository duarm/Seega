using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuController : MonoBehaviour 
{
	public Slider slider;
	public GameObject loadBar;
	public GameObject text;

	bool isShowing;
    private SceneController _sceneController;
    private ScreenFader _screenFader;

    [Inject]
	private void Construct(SceneController sceneController, ScreenFader screenFader){
		_sceneController = sceneController;
		_screenFader = screenFader;
	}

	private void Start() 
	{
		StartCoroutine(ShowPressToStart());
	}

	private void Update() 
	{
		if(Input.GetKeyDown(KeyCode.Return) && isShowing)
		{
			isShowing = false;
			StartCoroutine(_sceneController.LoadLevel(slider,text,loadBar));
		}
	}

	IEnumerator ShowPressToStart()
	{
		yield return StartCoroutine(_screenFader.FadeSceneIn());
		isShowing = true;
	}
}
