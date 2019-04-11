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
    public bool inMiddle;
    //public bool isCapturingSequence;
    //public int turnCaptures;

    private ParticleSystem m_DeathParticle;

    private void Start()
    {
        m_DeathParticle = GetComponentInChildren<ParticleSystem>();
    }

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

    private int Verify(Vector3 direction)
    {
        //Two arrays for organizational purposes
        RaycastHit[] hits;
        RaycastHit[] wallHits;
        hits = Physics.RaycastAll (this.transform.position, direction, 2, Board.Instance.whatIsPiece);
        wallHits = Physics.RaycastAll (this.transform.position, direction, 4, Board.Instance.whatIsPiece);

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
            //this.turnCaptures++;
            return 1;
        }

        return 0;
    }

    private void CheckForWall (Piece[] pieces)
    {
        //the wall is a 5 piece play, lets be sure we have all of them
        if ((pieces[0] ?? pieces[1] ?? pieces[2] ?? pieces[3]) != null)
        {
            if(this.type == PieceType.WHITE)
            {
                //the wall consists in 5 SAME COLOR pieces along a whole column or line
                if (IsAllPieceType(pieces, PieceType.WHITE))
                {
                    StopAllCoroutines ();
                    Board.Instance.EndGame (PieceType.WHITE, VictoryType.MINOR);
                }
            }
            else
            {
                if (IsAllPieceType(pieces, PieceType.BLACK))
                {
                    StopAllCoroutines ();
                    Board.Instance.EndGame (PieceType.BLACK, VictoryType.MINOR);
                }
            }
        }
    }

    private bool IsAllPieceType(Piece[] pieces, PieceType type)
    {
        if (pieces[0].type == type &&
            pieces[1].type == type &&
            pieces[2].type == type &&
            pieces[3].type == type)
        {
            return true;
        }

        return false;
    }

    private bool MatchCaptureConditions (Piece[] hittedPiece)
    {
        if (this.type == PieceType.WHITE)
        {
            if (hittedPiece[0].type == PieceType.BLACK && hittedPiece[1].type == PieceType.WHITE && !hittedPiece[0].movemented && !hittedPiece[0].inMiddle)
                return true;
        }
        else
        {
            if (hittedPiece[0].type == PieceType.WHITE && hittedPiece[1].type == PieceType.BLACK && !hittedPiece[0].movemented && !hittedPiece[0].inMiddle)
                return true;
        }

        return false;
    }

    public int VerifyAll ()
    {
        int killCounter = 0;
        killCounter += Verify(Vector3.forward);
        killCounter += Verify(Vector3.back);
        killCounter += Verify(Vector3.left);
        killCounter += Verify(Vector3.right);
        return killCounter;
    }

    public void Capture ()
    {
        m_DeathParticle.Play ();
        Destroy (this.gameObject, .5f);
    }
}