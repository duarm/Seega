using UnityEngine;

namespace Seega.Assets.Scripts.Core
{
    public class ShootRayFromCamera : MonoBehaviour, IRayProvider
    {
        Camera main;

        private void Start() => main = Camera.main;

        public Ray CreateRay() => main.ScreenPointToRay(Input.mousePosition);
    }
}