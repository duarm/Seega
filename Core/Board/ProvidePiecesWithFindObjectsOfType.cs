using UnityEngine;

namespace Seega.Scripts.Core
{
    public class ProvidePiecesWithFindObjectsOfType : MonoBehaviour, IPieceProvider
    {
        public Piece[] CreatePieces()
        {
            return FindObjectsOfType<Piece>();
        }
    }
}