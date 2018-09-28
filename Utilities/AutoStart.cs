using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//AUTO ACTIVE OR DEACTIVE AN OBJECT ON START
public class AutoStart : MonoBehaviour
{
    [Tooltip ("Value to set on Start")]
    public GameObject[] gameObjects;
    public bool active;
    void Start ()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject != null)
            {
                gameObject.SetActive (active);
            }
        }
    }
}