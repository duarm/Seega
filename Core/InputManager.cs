using UnityEngine;
using Zenject;

namespace Seega.Scripts.Core
{
    public class InputManager : MonoBehaviour
    {
        //Depend on Board
        private Board _board;

        [Inject]
        private void Construct (Board board)
        {
            _board = board;
        }

        private void Update ()
        {
            if (Input.GetMouseButtonDown (0))
            {
                OnMouseLeftDown ();
            }
        }

        private void OnMouseLeftDown ()
        {
            _board.OnMouseClick ();
        }
    }
}