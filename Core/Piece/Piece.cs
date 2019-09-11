using Seega.Enums;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public ColorType type;
    public float yOffset = .5f;

    [HideInInspector]
    public bool isPlaced;
    //public bool isCapturingSequence;
    //public int turnCaptures;

    ICapturer _capturer;

    private void Start ()
    {
        _capturer = GetComponent<ICapturer>();
    }

    public void Teleport (TileField tile)
    {
        transform.position = new Vector3 (tile.transform.position.x, tile.transform.position.y + yOffset, tile.transform.position.z);
        isPlaced = true;
    }

    public void MoveTo (TileField tile)
    {
        iTween.MoveTo (this.gameObject, new Vector3 (tile.transform.position.x, tile.transform.position.y + yOffset, tile.transform.position.z), .3f);
        isPlaced = true;
    }

    public void Capture ()
    {
        _capturer.Capture();
    }
}