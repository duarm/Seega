using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileField : MonoBehaviour 
{
	public string column;
	public int line;
	public Token token;

	public TileField moveUpField;
	public TileField moveDownField;
	public TileField moveLeftField;
	public TileField moveRightField;

	public bool highlighting = false;
	

	private void OnMouseDown()
	{
		if(Board.Instance.CurrentState == GameState.POSITIONING && !Board.Instance.IsUpdating)
		{
			if(token != null)
				return;

			if(this.column == "c" && this.line == 3)
				return;

			if(Board.Instance.CurrentTurn == Turn.BLACK)
			{
				GetToken();
				Token token = Board.Instance.GetNonPlacedBlackToken();
				if(token != null)
					token.MoveTo(new Vector3(this.transform.position.x,this.transform.position.y, -0.6f));
				
				this.token = token;
			}
			else if(Board.Instance.CurrentTurn == Turn.WHITE)
			{
				Token token = Board.Instance.GetNonPlacedWhiteToken();
				if(token != null)
					token.MoveTo(new Vector3(this.transform.position.x,this.transform.position.y, -0.6f));
				
				this.token = token;
			}

			Board.Instance.UpdatePlacedCounters();
		}
	}

    private GameObject GetLeftTile()
	{
		RaycastHit hit;
		Ray ray = new Ray(transform.position,Vector3.left);
		if(Physics.Raycast(ray, out hit, 1, Board.Instance.whatIsTile))
		{
			if(hit.collider != null)
			{
				return hit.collider.gameObject;
			}
		}

		return null;
	}

	private GameObject GetRightTile()
	{
		RaycastHit hit;
		Ray ray = new Ray(transform.position,Vector3.right);
		if(Physics.Raycast(ray, out hit, 1, Board.Instance.whatIsTile))
		{
			if(hit.collider != null)
			{
				return hit.collider.gameObject;
			}
		}

		return null;
	}

	private GameObject GetUpTile()
	{
		RaycastHit hit;
		Ray ray = new Ray(transform.position,Vector3.up);
		if(Physics.Raycast(ray, out hit, 1, Board.Instance.whatIsTile))
		{
			if(hit.collider != null)
			{
				return hit.collider.gameObject;
			}
		}

		return null;
	}

	private GameObject GetDownTile()
	{
		RaycastHit hit;
		Ray ray = new Ray(transform.position,Vector3.down);
		if(Physics.Raycast(ray, out hit, 1, Board.Instance.whatIsTile))
		{
			if(hit.collider != null)
			{
				return hit.collider.gameObject;
			}
		}

		return null;
	}

	public void RemovePiece()
	{
		this.token = null;
	}

	public void Verify()
	{
		var tile = GetUpTile();
		if(tile != null)
		{
			var upField = tile.GetComponent<TileField>();
			if(upField.token == null)
			{
				moveUpField = upField;
			}
			else
				moveUpField = null;
		}

		tile = GetDownTile();
		if(tile != null)
		{
			var downField = tile.GetComponent<TileField>();
			if(downField.token == null)
			{
				moveDownField = downField;
			}
			else
				moveDownField = null;
		}

		tile = GetLeftTile();
		if(tile != null)
		{
			var leftField = tile.GetComponent<TileField>();
			if(leftField.token == null)
			{
				moveLeftField = leftField;
			}
			else
				moveLeftField = null;
		}
		tile = GetRightTile();
		if(tile != null)
		{
			var rightField = tile.GetComponent<TileField>();
			if(rightField.token == null)
			{
				moveRightField = rightField;
			}
			else
				moveRightField = null;
		}
	}

	//WORKING
	private void GetToken()
	{
		Debug.Log("Draw Ray");
		RaycastHit hit;
		Ray ray = new Ray(transform.position,Vector3.back);
		Debug.DrawRay(transform.position, Vector3.back,Color.red, 2);
	}
	//END WORKING

	public void HighlightEmptyNeighbors()
	{
		if(this.highlighting)
		{
			DehighlightNeighbors();
			return;
		}

		Board.Instance.DehighlightAll();

		this.highlighting = true;
		Board.Instance.highlightedField = GetComponent<TileField>();
		
		if(moveUpField != null)
		{
			moveUpField.GetComponent<Renderer>().material.color = Board.Instance.highlighColor;
		}

		if(moveDownField != null)
		{
			moveDownField.GetComponent<Renderer>().material.color = Board.Instance.highlighColor;
		}

		if(moveLeftField != null)
		{
			moveLeftField.GetComponent<Renderer>().material.color = Board.Instance.highlighColor;
		}
			
		if(moveRightField != null)
		{
			moveRightField.GetComponent<Renderer>().material.color = Board.Instance.highlighColor;
		}
	}

	//A piece moved to this tile
	public IEnumerator TokenMoved()
	{
		Board.Instance.IsUpdating = true;

		Board.Instance.DehighlightAll();
		Board.Instance.highlightedField.token.MoveTo(GetComponent<TileField>());
		yield return new WaitForSeconds(.7f); 

		Board.Instance.newMovementOccured();

		this.token = Board.Instance.highlightedField.token;
		Board.Instance.highlightedField.RemovePiece();
		
		Board.Instance.highlightedField = this;

		if(this.column == "c" && this.line == 3)
		{
			this.token.isMiddle = true;
		}
		else
			this.token.isMiddle = false;
		
		this.token.movemented = true;

		Board.Instance.UpdateBoard();
	}

    public void DehighlightNeighbors()
    {
        this.highlighting = false;

		var tile = GetUpTile();
		if(tile != null)
		{
			tile.GetComponent<Renderer>().material.color = Board.Instance.normalColor;
		}

		tile = GetDownTile();
		if(tile != null)
		{
			tile.GetComponent<Renderer>().material.color = Board.Instance.normalColor;
		}

		tile = GetLeftTile();
		if(tile != null)
		{
			tile.GetComponent<Renderer>().material.color = Board.Instance.normalColor;
		}
		tile = GetRightTile();
		if(tile != null)
		{
			tile.GetComponent<Renderer>().material.color = Board.Instance.normalColor;
		}
    }
}
