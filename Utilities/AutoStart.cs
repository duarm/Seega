using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//AUTO TURN AN OBJECT
public class AutoStart : MonoBehaviour 
{
	public GameObject startMenu;

	void Start () 
	{
		if(!startMenu.activeInHierarchy)
			startMenu.SetActive(true);
	}
}
