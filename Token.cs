using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TokenType
{
    WHITE,
    BLACK
}

public class Token : MonoBehaviour 
{
    public bool placed;
    public TokenType type;
    public bool movemented;
    public bool isMiddle;
    public bool isCapturingSequence;
    public int turnCaptures;

    public void MoveTo(Vector3 position)
    {
        this.transform.position = position;
        placed = true;
        movemented = true;
    }

    public void MoveTo(TileField tile)
    {
        iTween.MoveTo(this.gameObject, new Vector3(tile.transform.position.x,tile.transform.position.y, -.6f), .7f);
        placed = true;
        movemented = true;
    }

    private TileField GetCurrentTile()
    {
        RaycastHit hit;
		Ray ray = new Ray(transform.position,Vector3.forward);
		if(Physics.Raycast(ray, out hit, 1, Board.Instance.whatIsTile))
		{
			if(hit.collider != null)
			{
				return hit.collider.GetComponent<TileField>();
			}
		}

		return null;
    }

    private int VerifyUp(Token token)
    {
		RaycastHit[] hits;
        RaycastHit[] wallHits;
		hits = Physics.RaycastAll(token.transform.position, Vector3.up, 2, Board.Instance.whatIsPiece);
        wallHits = Physics.RaycastAll(token.transform.position, Vector3.up, 4, Board.Instance.whatIsPiece);
		Debug.DrawRay(token.transform.position, Vector3.up, Color.red, 1);

		if(hits == null)
			return 0;

        Token[] hittedToken = new Token[2];
        Token[] wallTokens = new Token[4];

        for(int i = 0; i < wallHits.Length; i++)
        {
            wallTokens[i] = wallHits[i].collider.GetComponent<Token>();
        }

        if(this.type == TokenType.WHITE)
        {
            if(wallTokens[0] != null &&
               wallTokens[1] != null &&
               wallTokens[2] != null &&
               wallTokens[3] != null)
            {
                if(wallTokens[0].type == TokenType.WHITE &&
                wallTokens[1].type == TokenType.WHITE && 
                wallTokens[2].type == TokenType.WHITE && 
                wallTokens[3].type == TokenType.WHITE)
                {
                    Board.Instance.EndGame(TokenType.WHITE,VictoryType.MINOR);
                    return 0;
                }
            }
        }
        else
        {
            if(wallTokens[0] != null &&
               wallTokens[1] != null &&
               wallTokens[2] != null &&
               wallTokens[3] != null)
            {
                if(wallTokens[0].type == TokenType.BLACK &&
                wallTokens[1].type == TokenType.BLACK && 
                wallTokens[2].type == TokenType.BLACK && 
                wallTokens[3].type == TokenType.BLACK)
                {
                    Board.Instance.EndGame(TokenType.BLACK,VictoryType.MINOR);
                    return 0;
                }
            }
        }

		for (int i = 0; i < hits.Length; i++)
		{
			hittedToken[i] = hits[i].collider.GetComponent<Token>();
		}

        if(hittedToken[0] == null | hittedToken[1] == null)
            return 0;

        if(MatchCaptureConditions(hittedToken))
        {
            hittedToken[0].Capture();
            this.turnCaptures++;
            return 1;
        }

        return 0;
    }

    private int VerifyDown(Token token)
    {
        RaycastHit[] hits;
        RaycastHit[] wallHits;
		hits = Physics.RaycastAll(token.transform.position, Vector3.down, 2, Board.Instance.whatIsPiece);
        wallHits = Physics.RaycastAll(token.transform.position, Vector3.down, 4, Board.Instance.whatIsPiece);
		Debug.DrawRay(token.transform.position, Vector3.down, Color.red, 1);

		if(hits == null)
			return 0;

		Token[] hittedToken = new Token[2];

		for (int i = 0; i < hits.Length; i++)
		{
			hittedToken[i] = hits[i].collider.GetComponent<Token>();
		}

        Token[] wallTokens = new Token[4];

        for(int i = 0; i < wallHits.Length; i++)
        {
            wallTokens[i] = wallHits[i].collider.GetComponent<Token>();
        }

        if(this.type == TokenType.WHITE)
        {
            if(wallTokens[0] != null &&
               wallTokens[1] != null &&
               wallTokens[2] != null &&
               wallTokens[3] != null)
            {
                if(wallTokens[0].type == TokenType.WHITE &&
                wallTokens[1].type == TokenType.WHITE && 
                wallTokens[2].type == TokenType.WHITE && 
                wallTokens[3].type == TokenType.WHITE)
                {
                    Board.Instance.EndGame(TokenType.WHITE,VictoryType.MINOR);
                    return 0;
                }
            }
        }
        else
        {
            if(wallTokens[0] != null &&
               wallTokens[1] != null &&
               wallTokens[2] != null &&
               wallTokens[3] != null)
            {

                if(wallTokens[0].type == TokenType.BLACK &&
                wallTokens[1].type == TokenType.BLACK && 
                wallTokens[2].type == TokenType.BLACK && 
                wallTokens[3].type == TokenType.BLACK)
                {
                    Board.Instance.EndGame(TokenType.BLACK,VictoryType.MINOR);
                    return 0;
                }
            }
        }
        

        if(hittedToken[0] == null | hittedToken[1] == null)
            return 0;

        if(MatchCaptureConditions(hittedToken))
        {
            hittedToken[0].Capture();
            Debug.Log(hittedToken[0].name + " was captured by " + this.name);
            this.turnCaptures++;
            return 1;
        }

        return 0;
    }

    private int VerifyLeft(Token token)
    {
		RaycastHit[] hits;
        RaycastHit[] wallHits;
		hits = Physics.RaycastAll(token.transform.position, Vector3.left, 2, Board.Instance.whatIsPiece);
        wallHits = Physics.RaycastAll(token.transform.position, Vector3.left, 4, Board.Instance.whatIsPiece);
		Debug.DrawRay(token.transform.position, Vector3.left, Color.red, 1);
        
		if(hits == null)
			return 0;

		Token[] hittedToken = new Token[2];

		for (int i = 0; i < hits.Length; i++)
		{
			hittedToken[i] = hits[i].collider.GetComponent<Token>();
		}

        Token[] wallTokens = new Token[4];

        for(int i = 0; i < wallHits.Length; i++)
        {
            wallTokens[i] = wallHits[i].collider.GetComponent<Token>();
        }

        if(this.type == TokenType.WHITE)
        {
            if(wallTokens[0] != null &&
               wallTokens[1] != null &&
               wallTokens[2] != null &&
               wallTokens[3] != null)
            {
                if(wallTokens[0].type == TokenType.WHITE &&
                wallTokens[1].type == TokenType.WHITE && 
                wallTokens[2].type == TokenType.WHITE && 
                wallTokens[3].type == TokenType.WHITE)
                {
                    Board.Instance.EndGame(TokenType.WHITE,VictoryType.MINOR);
                    return 0;
                }
            }
        }
        else
        {
            if(wallTokens[0] != null &&
               wallTokens[1] != null &&
               wallTokens[2] != null &&
               wallTokens[3] != null)
            {
                if(wallTokens[0].type == TokenType.BLACK &&
                wallTokens[1].type == TokenType.BLACK && 
                wallTokens[2].type == TokenType.BLACK && 
                wallTokens[3].type == TokenType.BLACK)
                {
                    Board.Instance.EndGame(TokenType.BLACK,VictoryType.MINOR);
                    return 0;
                }
            }
        }

        if(hittedToken[0] == null | hittedToken[1] == null)
            return 0;

        if(MatchCaptureConditions(hittedToken))
        {
            hittedToken[0].Capture();
            Debug.Log(hittedToken[0].name + " was captured by " + this.name);
            this.turnCaptures++;
            return 1;
        }
        
        return 0;
    }

    private int VerifyRight(Token token)
    {
        RaycastHit[] hits;
        RaycastHit[] wallHits;
		hits = Physics.RaycastAll(token.transform.position, Vector3.right, 2, Board.Instance.whatIsPiece);
        wallHits = Physics.RaycastAll(token.transform.position, Vector3.right, 4, Board.Instance.whatIsPiece);
		Debug.DrawRay(token.transform.position, Vector3.right, Color.red, 1);

		if(hits == null)
			return 0;

		Token[] hittedToken = new Token[2];

		for (int i = 0; i < hits.Length; i++)
		{
			hittedToken[i] = hits[i].collider.GetComponent<Token>();
		}

        Token[] wallTokens = new Token[4];

        for(int i = 0; i < wallHits.Length; i++)
        {
            wallTokens[i] = wallHits[i].collider.GetComponent<Token>();
        }

        if(this.type == TokenType.WHITE)
        {
            if(wallTokens[0] != null &&
               wallTokens[1] != null &&
               wallTokens[2] != null &&
               wallTokens[3] != null)
            {
                if(wallTokens[0].type == TokenType.WHITE &&
                wallTokens[1].type == TokenType.WHITE && 
                wallTokens[2].type == TokenType.WHITE && 
                wallTokens[3].type == TokenType.WHITE)
                {
                    Board.Instance.EndGame(TokenType.WHITE,VictoryType.MINOR);
                    return 0;
                }
            }
        }
        else
        {
            if(wallTokens[0] != null &&
               wallTokens[1] != null &&
               wallTokens[2] != null &&
               wallTokens[3] != null)
            {
                if(wallTokens[0].type == TokenType.BLACK &&
                wallTokens[1].type == TokenType.BLACK && 
                wallTokens[2].type == TokenType.BLACK && 
                wallTokens[3].type == TokenType.BLACK)
                {
                    Board.Instance.EndGame(TokenType.BLACK,VictoryType.MINOR);
                    return 0;
                }
            }
        }

        if(hittedToken[0] == null | hittedToken[1] == null)
            return 0;

        if(MatchCaptureConditions(hittedToken))
        {
            hittedToken[0].Capture();
            Debug.Log(hittedToken[0].name + " was captured by " + this.name);
            this.turnCaptures++;
            return 1;
        }

        return 0;
    }

    private bool MatchCaptureConditions(Token[] hittedToken)
    {
        if(this.type == TokenType.WHITE)
        {
            if(hittedToken[0].type == TokenType.BLACK && hittedToken[1].type == TokenType.WHITE && !hittedToken[0].movemented && !hittedToken[0].isMiddle)
            {
                return true;
            }
        }
        else
        {
            if(hittedToken[0].type == TokenType.WHITE && hittedToken[1].type == TokenType.BLACK && !hittedToken[0].movemented && !hittedToken[0].isMiddle)
            {
                return true;
            }
        }

        return false;
    }

	public int Verify()
	{
        int killCounter = 0;
        killCounter += VerifyUp(this);
        killCounter += VerifyRight(this);
        killCounter += VerifyDown(this);
        killCounter += VerifyLeft(this);
        return killCounter;
	}

    public void Capture()
    {
        this.GetCurrentTile().RemovePiece();
        this.GetComponentInChildren<ParticleSystem>().Play();
        Destroy(this.gameObject,.5f);
    }
}
