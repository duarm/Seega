using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour 
{
	public TextMeshProUGUI turnText;
	public TextMeshProUGUI stateText;

	public TextMeshProUGUI winnerText;
	public TextMeshProUGUI victoryTypeText;

	public GameObject endGameWindow;
	public Button restartButton;
	public GameObject surrenderButton;

	private string[] turns = new string[2] {"Branca","Preta"};
	private string[] states = new string[2] {"Posicionamento", "Movimento"};

	private void Start() 
	{
		Board.Instance.onTurnChangeCallback += UpdateTurnText;
		Board.Instance.onStateChangeCallback += UpdateStateUI;
		Board.Instance.onGameEndCallback += ActivateEndWindow;
		restartButton.onClick.AddListener(RestartButton);
	}

	public void Starting(int starting)
	{
		if(starting == 0)
		{
			Board.Instance.CurrentTurn = Turn.WHITE;
		}
		else if(starting == 1)
		{
			Board.Instance.CurrentTurn = Turn.BLACK;
		}
		else
		{
			Debug.LogError("Invalid choice index, check the Button parameter");
		}

		Board.Instance.StartPositioningPhase();
	}

	public void ActivateEndWindow(string winner, string winType)
	{
		ActiveSurrenderButton(false);
		endGameWindow.SetActive(true);
		winnerText.text = winner;
		victoryTypeText.text = winType;
	}

	public void ActiveSurrenderButton(bool value)
	{
		surrenderButton.SetActive(value);
	}


	public void SurrenderButton()
	{
		if(Board.Instance.CurrentTurn == Turn.WHITE)
			Board.Instance.EndGame(TokenType.BLACK, VictoryType.GREAT);
		else
			Board.Instance.EndGame(TokenType.WHITE, VictoryType.GREAT);
	}

	public void RestartButton()
	{
		StartCoroutine(ScreenFader.Instance.Restart());
	}

	public void UpdateTurnText()
	{
		if(turnText == null)
			Debug.LogError("Turn Text display is null, verify the Editor reference.");

		if(Board.Instance.CurrentTurn == Turn.WHITE)
		{
			turnText.text = turns[0];
		}
		else
		{
			turnText.text = turns[1];
		}
	}

	public void UpdateStateUI()
	{
		if(stateText == null)
			Debug.LogError("State Text display is null, verify the Editor reference.");

		if(Board.Instance.CurrentState == GameState.POSITIONING)
		{
			stateText.text = states[0];
		}
		else if(Board.Instance.CurrentState == GameState.MOVEMENT)
		{
			stateText.text = states[1];
		}
		else
		{

		}
	}
}
