using UnityEngine;

public class RaycastShooterWithLayerMask : MonoBehaviour, ISelector
{
    public LayerMask mask;

    private RaycastHit _hit;

    public bool Check(Ray ray) => Physics.Raycast(ray,out _hit, 20,mask);

    public Collider GetSelection() => _hit.collider;
}