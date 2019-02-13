using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    WHITE,
    BLACK
}

public class Piece : MonoBehaviour
{
    public bool placed;
    public PieceType type;
    public bool movemented;
    public bool isMiddle;
    public bool isCapturingSequence;
    public int turnCaptures;

    public void Teleport (Vector3 position)
    {
        this.transform.position = position;
        placed = true;
        movemented = true;
    }

    public void MoveTo (TileField tile)
    {
        iTween.MoveTo (this.gameObject, new Vector3 (tile.transform.position.x, .5f, tile.transform.position.z), .7f);
        placed = true;
        movemented = true;
    }

    private int VerifyUp (Piece piece)
    {
        //Two arrays for organizational purposes
        RaycastHit[] hits;
        RaycastHit[] wallHits;
        hits = Physics.RaycastAll (piece.transform.position, Vector3.forward, 2, Board.Instance.whatIsPiece);
        wallHits = Physics.RaycastAll (piece.transform.position, Vector3.forward, 4, Board.Instance.whatIsPiece);
        Debug.DrawRay (piece.transform.position, Vector3.up, Color.white, 1);

        if (hits == null)
            return 0;

        //Filling both with the hits
        Piece[] hittedPiece = new Piece[2];

        for (int i = 0; i < hits.Length; i++)
        {
            hittedPiece[i] = hits[i].collider.GetComponent<Piece> ();
        }

        Piece[] wallPieces = new Piece[4];

        for (int i = 0; i < wallHits.Length; i++)
        {
            wallPieces[i] = wallHits[i].collider.GetComponent<Piece> ();
        }

        //Checking for a wall play
        CheckForWall (wallPieces);

        if (hittedPiece[0] == null | hittedPiece[1] == null)
            return 0;

        if (MatchCaptureConditions (hittedPiece))
        {
            hittedPiece[0].Capture ();
            this.turnCaptures++;
            return 1;
        }

        return 0;
    }

    private int VerifyDown (Piece piece)
    {
        RaycastHit[] hits;
        RaycastHit[] wallHits;
        hits = Physics.RaycastAll (piece.transform.position, Vector3.back, 2, Board.Instance.whatIsPiece);
        wallHits = Physics.RaycastAll (piece.transform.position, Vector3.back, 4, Board.Instance.whatIsPiece);
        //Debug.DrawRay (piece.transform.position, Vector3.down, Color.red, 1);

        if (hits == null)
            return 0;

        Piece[] hittedPiece = new Piece[2];

        for (int i = 0; i < hits.Length; i++)
        {
            hittedPiece[i] = hits[i].collider.GetComponent<Piece> ();
        }

        Piece[] wallPieces = new Piece[4];

        for (int i = 0; i < wallHits.Length; i++)
        {
            wallPieces[i] = wallHits[i].collider.GetComponent<Piece> ();
        }

        CheckForWall (wallPieces);

        if (hittedPiece[0] == null | hittedPiece[1] == null)
            return 0;

        if (MatchCaptureConditions (hittedPiece))
        {
            hittedPiece[0].Capture ();
            this.turnCaptures++;
            return 1;
        }

        return 0;
    }

    private int VerifyLeft (Piece piece)
    {
        RaycastHit[] hits;
        RaycastHit[] wallHits;
        hits = Physics.RaycastAll (piece.transform.position, Vector3.left, 2, Board.Instance.whatIsPiece);
        wallHits = Physics.RaycastAll (piece.transform.position, Vector3.left, 4, Board.Instance.whatIsPiece);
        Debug.DrawRay (piece.transform.position, Vector3.left, Color.blue, 1);

        if (hits == null)
            return 0;

        Piece[] hittedPiece = new Piece[2];

        for (int i = 0; i < hits.Length; i++)
        {
            hittedPiece[i] = hits[i].collider.GetComponent<Piece> ();
        }

        Piece[] wallPieces = new Piece[4];

        for (int i = 0; i < wallHits.Length; i++)
        {
            wallPieces[i] = wallHits[i].collider.GetComponent<Piece> ();
        }

        CheckForWall (wallPieces);

        if (hittedPiece[0] == null | hittedPiece[1] == null)
            return 0;

        if (MatchCaptureConditions (hittedPiece))
        {
            hittedPiece[0].Capture ();
            this.turnCaptures++;
            return 1;
        }

        return 0;
    }

    private int VerifyRight (Piece piece)
    {
        RaycastHit[] hits;
        RaycastHit[] wallHits;
        hits = Physics.RaycastAll (piece.transform.position, Vector3.right, 2, Board.Instance.whatIsPiece);
        wallHits = Physics.RaycastAll (piece.transform.position, Vector3.right, 4, Board.Instance.whatIsPiece);
        Debug.DrawRay (piece.transform.position, Vector3.right, Color.green, 1);

        if (hits == null)
            return 0;

        Piece[] hittedPiece = new Piece[2];

        for (int i = 0; i < hits.Length; i++)
        {
            hittedPiece[i] = hits[i].collider.GetComponent<Piece> ();
        }

        Piece[] wallPieces = new Piece[4];

        for (int i = 0; i < wallHits.Length; i++)
        {
            wallPieces[i] = wallHits[i].collider.GetComponent<Piece> ();
        }

        CheckForWall (wallPieces);

        if (hittedPiece[0] == null | hittedPiece[1] == null)
            return 0;

        if (MatchCaptureConditions (hittedPiece))
        {
            hittedPiece[0].Capture ();
            this.turnCaptures++;
            return 1;
        }

        return 0;
    }

    private void CheckForWall (Piece[] wallPieces)
    {
        //We check for both players
        if (this.type == PieceType.WHITE)
        {
            //the wall is a 5 piece play, lets be sure we have all of them
            if (wallPieces[0] != null &&
                wallPieces[1] != null &&
                wallPieces[2] != null &&
                wallPieces[3] != null)
            {
                //the wall consists in 5 SAME COLOR pieces along a whole column or line
                if (wallPieces[0].type == PieceType.WHITE &&
                    wallPieces[1].type == PieceType.WHITE &&
                    wallPieces[2].type == PieceType.WHITE &&
                    wallPieces[3].type == PieceType.WHITE)
                {
                    StopAllCoroutines ();
                    Board.Instance.EndGame (PieceType.WHITE, VictoryType.MINOR);
                }
            }
        }
        else
        {
            if (wallPieces[0] != null &&
                wallPieces[1] != null &&
                wallPieces[2] != null &&
                wallPieces[3] != null)
            {
                if (wallPieces[0].type == PieceType.BLACK &&
                    wallPieces[1].type == PieceType.BLACK &&
                    wallPieces[2].type == PieceType.BLACK &&
                    wallPieces[3].type == PieceType.BLACK)
                {
                    StopAllCoroutines ();
                    Board.Instance.EndGame (PieceType.BLACK, VictoryType.MINOR);
                }
            }
        }
    }

    private bool MatchCaptureConditions (Piece[] hittedPiece)
    {
        if (this.type == PieceType.WHITE)
        {
            if (hittedPiece[0].type == PieceType.BLACK && hittedPiece[1].type == PieceType.WHITE && !hittedPiece[0].movemented && !hittedPiece[0].isMiddle)
            {
                return true;
            }
        }
        else
        {
            if (hittedPiece[0].type == PieceType.WHITE && hittedPiece[1].type == PieceType.BLACK && !hittedPiece[0].movemented && !hittedPiece[0].isMiddle)
            {
                return true;
            }
        }

        return false;
    }

    public int Verify ()
    {
        int killCounter = 0;
        killCounter += VerifyUp (this);
        killCounter += VerifyRight (this);
        killCounter += VerifyDown (this);
        killCounter += VerifyLeft (this);
        return killCounter;
    }

    public void Capture ()
    {
        //this.GetCurrentTile().RemovePiece();
        this.GetComponentInChildren<ParticleSystem> ().Play ();
        Destroy (this.gameObject, .5f);
    }
}