using UnityEngine;

public interface ISelector
{
    bool Check (Ray ray);
    Collider GetSelection ();
}