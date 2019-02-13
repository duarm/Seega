using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour 
{
	public Slider slider;
	public GameObject loadBar;
	public GameObject text;

	bool isShowing;

	private void Start() 
	{
		StartCoroutine(ShowPressToStart());
	}

	private void Update() 
	{
		if(Input.GetKeyDown(KeyCode.Return) && isShowing)
		{
			isShowing = false;
			StartCoroutine(SceneController.Instance.LoadLevel(slider,text,loadBar));
		}
	}

	IEnumerator ShowPressToStart()
	{
		yield return StartCoroutine(ScreenFader.FadeSceneIn());
		isShowing = true;
	}
}
