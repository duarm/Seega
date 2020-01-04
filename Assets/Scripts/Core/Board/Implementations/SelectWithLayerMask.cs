using UnityEngine;

public class SelectWithLayerMask : MonoBehaviour, ISelector
{
    public LayerMask tileMask;

    private RaycastHit _hit;

    public bool Check (Ray ray) => Physics.Raycast (ray, out _hit, 20, tileMask);

    public Collider GetSelection () => _hit.collider;
}