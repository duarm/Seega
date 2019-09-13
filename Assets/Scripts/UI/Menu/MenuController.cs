using System.Collections;
using UnityEngine;
using Zenject;

public class MenuController : MonoBehaviour
{
	public GameObject startContainer;
	public GameObject optionsContainer;

	bool isShowing;
	
	private SceneController _sceneController;
	private ScreenFader _screenFader;

	[Inject]
	private void Construct (SceneController sceneController, ScreenFader screenFader)
	{
		_sceneController = sceneController;
		_screenFader = screenFader;
	}

	void Start ()
	{
		StartCoroutine (ShowPressToStart ());
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Return) && isShowing)
		{
			isShowing = false;
			startContainer.SetActive(false);
			optionsContainer.SetActive(true);
		}
	}

	IEnumerator ShowPressToStart()
	{
		yield return _screenFader.FadeSceneIn();
		isShowing = true;
	}
}