using UnityEngine;

public class ShootRayFromCamera : MonoBehaviour, IRayProvider
{
    Camera main;

    private void Start() => main = Camera.main;

    public Ray CreateRay() => main.ScreenPointToRay(Input.mousePosition);
}