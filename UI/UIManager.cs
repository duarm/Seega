using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI stateText;

    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI victoryTypeText;

    public GameObject endGameWindow;
    public GameObject surrenderButton;

    private string[] turns = new string[2] { "Branca", "Preta" };
    private string[] states = new string[2] { "Posicionamento", "Movimento" };

    private void Start ()
    {
        Board.Instance.onTurnChangeCallback += UpdateTurnUI;
        Board.Instance.onStateChangeCallback += UpdateStateUI;
        Board.Instance.onGameEndCallback += ActivateEndWindow;
    }

    public void RestartButton ()
    {
        SceneController.Instance.RestartScene ();
    }

    public void ActivateEndWindow (string winner, string winType)
    {
        surrenderButton.SetActive (false);
        endGameWindow.SetActive (true);
        winnerText.text = winner;
        victoryTypeText.text = winType;
    }

    public void UpdateTurnUI (bool isWhiteTurn)
    {
        if (isWhiteTurn)
            turnText.text = turns[0];
        else
            turnText.text = turns[1];
    }

    public void UpdateStateUI (GameState state)
    {
        if(!surrenderButton.activeInHierarchy)
            surrenderButton.SetActive (true);

        if (state == GameState.POSITIONING)
            stateText.text = states[0];
        else if (state == GameState.MOVEMENT)
            stateText.text = states[1];
    }

    //Called by the Help Button
    public void InvertActive(GameObject objectToInvert)
    {
        objectToInvert.SetActive(!objectToInvert.activeInHierarchy);
    }
}