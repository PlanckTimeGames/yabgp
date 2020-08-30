using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BishopPiece : ChessPiece
{
    public override PieceType Type { get { return PieceType.Bishop; } }
    public override List<ChessTurn> CalculateValidMoves(ChessTurn turn, bool considerChecks)
    {
        var curPos = GetPosition() as ChessBoardPosition;
        ChessMoveInfo[] possibleMoves = new ChessMoveInfo[]
        {
            new ChessMoveInfo( 1, 1, true), new ChessMoveInfo( 1,-1, true),
            new ChessMoveInfo(-1, 1, true), new ChessMoveInfo(-1,-1, true)
        };

        return boardInfo.GetValidMoves(turn, possibleMoves.ToList(), considerChecks);
    }
}
