using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieceFactory : MonoBehaviour
{
    public GameObject kingPrefab, queenPrefab, rookPrefab, bishopPrefab, knightPrefab, pawnPrefab;
    public ChessPiece CreatePiece(ChessPiece.PieceType type)
    {
        switch (type)
        {
            case ChessPiece.PieceType.King:
                return Instantiate<GameObject>(kingPrefab).GetComponent<KingPiece>();
            case ChessPiece.PieceType.Queen:
                return Instantiate<GameObject>(queenPrefab).GetComponent<QueenPiece>();
            case ChessPiece.PieceType.Rook:
                return Instantiate<GameObject>(rookPrefab).GetComponent<RookPiece>();
            case ChessPiece.PieceType.Bishop:
                return Instantiate<GameObject>(bishopPrefab).GetComponent<BishopPiece>();
            case ChessPiece.PieceType.Knight:
                return Instantiate<GameObject>(knightPrefab).GetComponent<KnightPiece>();
            case ChessPiece.PieceType.Pawn:
                return Instantiate<GameObject>(pawnPrefab).GetComponent<PawnPiece>();
            default:
                return null;
        }
    }
}
