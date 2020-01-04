using UnityEngine;

public interface ISelector2D
{
    bool Check (Ray ray);
    Collider2D GetSelection ();
}