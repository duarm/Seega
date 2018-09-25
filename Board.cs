using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState
{
	STARTING,
	POSITIONING,
	MOVEMENT,
	END
}

public enum Turn
{
	WHITE,
	BLACK
}

public enum VictoryType
{
	TOTAL,
	GREAT,
	MINOR
}


public class Board : MonoBehaviour 
{
	#region Singleton
	static protected Board s_BoardInstance;
	static public Board Instance { get { return s_BoardInstance; } }
	#endregion

	public bool IsUpdating
	{
		get {return isUpdating;}
		set {isUpdating = value;}
	}

	[Header("Tokens")]
	public List<Token> tokens;
	public List<Token> whiteTokens;
	public List<Token> blackTokens;

	public List<TileField> fields;

	//Layers Masks
	[Header("Layer Masks")]
	public LayerMask whatIsPiece;
	public LayerMask whatIsTile;

	//Colors
	[Header("Colors")]
	public Color highlighColor;
	public Color removeTokenColor;
	public Color normalColor;
	
	public bool movementLocked;
	public float fieldUpdateTime = .5f;	//Determines how fast the board will update

	public TileField highlightedField;	//Keep track of the current selected token

	private bool isUpdating;
	private int placedCounter;		//Counts the number of placed tokens by the player during this turn in the positioning phase
	private WaitForSeconds fieldUpdateRate;
	private Turn m_CurrentTurn;
	private GameState m_CurrentGameState;

	public Turn CurrentTurn
	{
		get { return m_CurrentTurn; }
		set { m_CurrentTurn = value; }
	}

	public GameState CurrentState
	{
		get { return m_CurrentGameState; }
		set { m_CurrentGameState = value; }
	}

	public bool HasWhiteTokens
	{
		get;
		private set;
	}

	public bool HasBlackTokens
	{
		get;
		private set;
	}

	public delegate void OnTurnChange();
	public OnTurnChange onTurnChangeCallback;

	public delegate void OnStateChange();
	public OnStateChange onStateChangeCallback;

	public delegate void OnGameEnd(string winner, string winType);
	public OnGameEnd onGameEndCallback;


	//Unity Methods
	void Reset()
	{
		tokens.Clear();
		whiteTokens.Clear();
		blackTokens.Clear(); 
	}

	void Awake()
	{
		s_BoardInstance = this;
		HasWhiteTokens = true;
		HasBlackTokens = true;
	}

	void Start()
	{
		m_CurrentGameState = GameState.STARTING;
		fieldUpdateRate = new WaitForSeconds(fieldUpdateTime);
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0) && !isUpdating)
		{
			if(m_CurrentGameState == GameState.MOVEMENT)
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				//shooting a raycast to get the tile that the player clicked
				if (Physics.Raycast(ray, out hit, 20, whatIsTile))
				{
					var tile = hit.collider.GetComponent<TileField>();

					//if theres a token on this tile
					if(tile.token != null)
					{
						//we check in which turn we are to see if its able to highligh the neighbors of that field
						if(ValidateHighlight(tile.token))
							tile.HighlightEmptyNeighbors();
					}
					else if(tile.token == null && highlightedField != null)
					{
						//if we already have a field highlighted, and we clicked on a empty tile, we valid the movement and move if its possible
						if(ValidateMovement(tile))
							StartCoroutine(tile.TokenMoved());
					}
				}
			}
		}

		//DEBUG
		if(Input.GetKeyDown(KeyCode.G))
		{
			Debug.Log(highlightedField);
		}
	}

	/// <summary>
	/// Check if the clicked token corresponds to this turn
	/// </summary>
	public bool ValidateHighlight(Token token)
	{
		if(token.type == TokenType.WHITE)
		{
			if(m_CurrentTurn == Turn.WHITE)
				return true;
		}
		else
		{
			if(m_CurrentTurn == Turn.BLACK)
				return true;
		}

		return false;
	}

	private bool ValidateMovement(TileField moveField)
	{
		if(highlightedField.moveDownField != null)
		{
			if(highlightedField.moveDownField == moveField)
			{
				return true;
			}
		}

		if(highlightedField.moveLeftField != null)
		{
			if(highlightedField.moveLeftField == moveField)
			{
				return true;
			}
		}

		if(highlightedField.moveRightField != null)
		{
			if(highlightedField.moveRightField == moveField)
			{
				return true;
			}
		}

		if(highlightedField.moveUpField != null)
		{
			if(highlightedField.moveUpField == moveField)
			{
				return true;
			}
		}

		return false;
	}

	public void UpdateBoard()
	{
		VerifyAllTiles();
		StartCoroutine(Updater());
	}

	IEnumerator Updater()
	{
		foreach(Token token in tokens)
		{
			if(token != null)
			{
				if(token.Verify() > 0)
					yield return fieldUpdateRate;
			}
		}

		VerifyAllTiles();

		int count = 0;
		foreach(Token token in whiteTokens)
		{
			if(token == null)
			{
				count++;
			}
		}

		if(count == 12)
		{
			EndGame(TokenType.BLACK, VictoryType.TOTAL);
			yield return null;
		}
		else
			count = 0;

		foreach(Token token in blackTokens)
		{
			if(token == null)
			{
				count++;
			}
		}

		if(count == 12)
		{
			EndGame(TokenType.WHITE, VictoryType.TOTAL);
		}

		if(highlightedField.token.turnCaptures == 0)
		{
			NextTurn();
		}
		else
		{
			highlightedField.HighlightEmptyNeighbors();
			highlightedField.token.isCapturingSequence = true;
		}

		isUpdating = false;
	}

	public void EndGame(TokenType winner, VictoryType victoryType)
	{
		if(onGameEndCallback == null)
			return;

		CurrentState = GameState.END;

		if(winner == TokenType.WHITE)
		{
			if(victoryType == VictoryType.TOTAL)
			{
				onGameEndCallback.Invoke("Brancas","Total");
			}
			else if(victoryType == VictoryType.GREAT)
			{
				onGameEndCallback.Invoke("Brancas","Grande");
			}
			else if(victoryType == VictoryType.MINOR)
			{
				onGameEndCallback.Invoke("Brancas","Pequena");
			}
		}
		else
		{
			if(victoryType == VictoryType.TOTAL)
			{
				onGameEndCallback.Invoke("Pretas","Total");
			}
			else if(victoryType == VictoryType.GREAT)
			{
				onGameEndCallback.Invoke("Pretas","Grande");
			}
			else if(victoryType == VictoryType.MINOR)
			{
				onGameEndCallback.Invoke("Pretas","Pequena");
			}
		}
	}

	public void StartPositioningPhase()
	{
		m_CurrentGameState = GameState.POSITIONING;

		if(onStateChangeCallback != null)
			onStateChangeCallback.Invoke();

		if(onTurnChangeCallback != null)
			onTurnChangeCallback.Invoke();
	}

	public void VerifyState()
	{
		if(m_CurrentTurn == Turn.WHITE)
		{
			if(LastTokenIsPlaced(TokenType.BLACK))
			{
				Debug.Log("Starting Phase");
				StartMovementPhase();
			}
			else
			{
				NextTurn();
			}
		}
		else
		{
			if(LastTokenIsPlaced(TokenType.WHITE))
			{
				Debug.Log("Starting Phase");
				StartMovementPhase();
			}
			else
			{
				NextTurn();
			}
		}
	}

	public void UpdatePlacedCounters()
	{
		this.placedCounter++;
		if(this.placedCounter == 2)
		{
			this.placedCounter = 0;
			VerifyState();
		}
	}

	
	public void NextTurn()
	{
		if(m_CurrentTurn == Turn.WHITE)
		{
			m_CurrentTurn = Turn.BLACK;
		}
		else
		{
			m_CurrentTurn = Turn.WHITE;
		}

		highlightedField = null;
		
		if(onTurnChangeCallback != null)
			onTurnChangeCallback.Invoke();
	}

	public void StartMovementPhase()
	{
		m_CurrentGameState = GameState.MOVEMENT;
		VerifyAllTiles();

		if(onStateChangeCallback != null)
			onStateChangeCallback.Invoke();
	}

	//Game Methods

	/// <summary>
	/// Returns the first non placed White token avaiable.null Returns null if none.
	/// </summary>
	public Token GetNonPlacedWhiteToken()
	{
		foreach(Token token in whiteTokens)
		{
			if(!token.placed)
			{
				return token;
			}
		}

		if(LastTokenIsPlaced(TokenType.WHITE))
			HasWhiteTokens = false;

		return null;
	}

	/// <summary>
	/// Returns the first non placed Black token avaiable.null Returns null if none.
	/// </summary>
	public Token GetNonPlacedBlackToken()
	{
		foreach(Token token in blackTokens)
		{
			if(!token.placed)
			{
				return token;
			}
		}

		if(LastTokenIsPlaced(TokenType.BLACK))
			HasBlackTokens = false;

		return null;
	}

	private bool LastTokenIsPlaced(TokenType type)
	{
		if(type == TokenType.WHITE)
		{
			return whiteTokens[(whiteTokens.Count - 1)].placed;				
		}
		else
		{
			return blackTokens[(blackTokens.Count - 1)].placed;	
		}
	}

	public void VerifyAllTiles()
	{
		foreach(TileField tile in fields)
		{
			tile.Verify();
		}
	}

	public void DehighlightAll()
    {
        foreach(TileField tile in Board.Instance.fields)
		{
			if(tile.highlighting)
			{
				tile.DehighlightNeighbors();
			}
		}
    }

	public void VerifyAllTokens()
	{
		foreach(Token token in whiteTokens)
		{
			token.Verify();
		}

		foreach(Token token in blackTokens)
		{
			token.Verify();
		}
	}

	public void newMovementOccured()
	{
		foreach(Token token in tokens)
		{
			if(token != null)
			{
				token.turnCaptures = 0;
				token.movemented = false;
				token.isCapturingSequence = false;
			}
		}
	}
}
