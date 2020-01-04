using System.Collections.Generic;
using System.Linq;
using Seega.Enums;
using UnityEngine;

public class PieceProviderWithFindObjectsOfType : MonoBehaviour, IPieceProvider
{
    private List<Piece> _pieces;

    void Start ()
    {
        _pieces = FindObjectsOfType<Piece> ().ToList ();
    }

    public Piece GetNonPlacedBlackPiece ()
    {
        foreach (Piece piece in _pieces)
        {
            if (piece.type == ColorType.BLACK && !piece.isPlaced)
            {
                return piece;
            }
        }

        return null;
    }

    public Piece GetNonPlacedWhitePiece ()
    {
        foreach (Piece piece in _pieces)
        {
            if (piece.type == ColorType.WHITE && !piece.isPlaced)
            {
                return piece;
            }
        }

        return null;
    }

    public int PieceCount => _pieces.Count;
}