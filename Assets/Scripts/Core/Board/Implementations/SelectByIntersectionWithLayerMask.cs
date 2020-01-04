using UnityEngine;

public class SelectByIntersectionWithLayerMask : MonoBehaviour, ISelector2D
{
    public LayerMask tileMask;

    private RaycastHit2D[] _hit = new RaycastHit2D[1];

    public bool Check (Ray ray)
    {
        Debug.DrawLine(ray.origin, ray.direction * 15, Color.red, 5);
        return Physics2D.GetRayIntersectionNonAlloc (ray, _hit, 15, tileMask) > 0;
    }

    public Collider2D GetSelection () => _hit[0].collider;
}