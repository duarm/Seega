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
    public PieceType type;
    [HideInInspector]
    public bool isPlaced;
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
    }

    public void MoveTo (TileField tile)
    {
        iTween.MoveTo (this.gameObject, new Vector3 (tile.transform.position.x, .5f, tile.transform.position.z), .3f);
        isPlaced = true;
    }

    public void Capture ()
    {
        m_DeathParticle.Play ();
        Destroy (this.gameObject, .5f);
    }
}