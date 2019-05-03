using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PieceType
{
    WHITE,
    BLACK
}

public class Piece : MonoBehaviour
{
    public bool isPlaced;
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

    public void Teleport (TileField tile)
    {
        transform.position = new Vector3 (tile.transform.position.x, .5f, tile.transform.position.z);
        isPlaced = true;
        movemented = true;
    }

    public void MoveTo (TileField tile)
    {
        iTween.MoveTo (this.gameObject, new Vector3 (tile.transform.position.x, .5f, tile.transform.position.z), .3f);
        isPlaced = true;
        movemented = true;
    }

    private void CheckForWall (ref Piece[] pieces)
    {
        //if ((pieces[0] ?? pieces[1] ?? pieces[2] ?? pieces[3]) != null)
        if (pieces.All(s => s == null))
        {
            if(this.type == PieceType.WHITE)
            {
                //the wall consists in 5 SAME COLOR pieces along a whole column or line
                if (IsAllPieceType(ref pieces, PieceType.WHITE))
                {
                    StopAllCoroutines ();
                    Board.Instance.EndGame (PieceType.WHITE, VictoryType.MINOR);
                }
            }
            else
            {
                if (IsAllPieceType(ref pieces, PieceType.BLACK))
                {
                    StopAllCoroutines ();
                    Board.Instance.EndGame (PieceType.BLACK, VictoryType.MINOR);
                }
            }
        }
    }

    private bool IsAllPieceType(ref Piece[] pieces, PieceType type)
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

    public void Capture ()
    {
        m_DeathParticle.Play ();
        Destroy (this.gameObject, .5f);
    }
}