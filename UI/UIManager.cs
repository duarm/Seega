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

    public void Starting (int starting)
    {
        if (starting == 0)
            Board.Instance.IsWhiteTurn = true;
        else if (starting == 1)
            Board.Instance.IsWhiteTurn = false;
        else
            Debug.LogError ("Invalid choice index, check the Button parameter");

        Board.Instance.StartPositioningPhase ();
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

    public void SurrenderButton ()
    {
        if (Board.Instance.IsWhiteTurn)
            Board.Instance.EndGame (PieceType.BLACK, VictoryType.GREAT);
        else
            Board.Instance.EndGame (PieceType.WHITE, VictoryType.GREAT);
    }

    public void UpdateTurnUI ()
    {
        if (turnText == null)
            Debug.LogError ("Turn Text display is null, verify the Editor reference.");

        if (Board.Instance.IsWhiteTurn)
        {
            turnText.text = turns[1];
        }
        else
        {
            turnText.text = turns[0];
        }
    }

    public void UpdateStateUI ()
    {
        if (stateText == null)
            Debug.LogError ("State Text display is null, verify the Editor reference.");

        if (Board.Instance.CurrentState == GameState.POSITIONING)
        {
            stateText.text = states[0];
        }
        else if (Board.Instance.CurrentState == GameState.MOVEMENT)
        {
            Debug.Log ("Turning on Surrender");
            stateText.text = states[1];
            surrenderButton.SetActive (true);
        }
    }

    public void InvertActive(GameObject objectToInvert)
    {
        objectToInvert.SetActive(!objectToInvert.activeInHierarchy);
    }
}