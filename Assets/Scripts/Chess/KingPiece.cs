using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KingPiece : ChessPiece
{
    public override PieceType Type { get { return PieceType.King; } }
    public override List<ChessTurn> CalculateValidMoves(ChessTurn turn, bool considerChecks)
    {
        var curPos = GetPosition() as ChessBoardPosition;
        ChessMoveInfo[] possibleMoves = new ChessMoveInfo[]
        {
            new ChessMoveInfo(-1, 1, false), new ChessMoveInfo( 0, 1, false), new ChessMoveInfo( 1, 1, false),
            new ChessMoveInfo(-1, 0, false),                                  new ChessMoveInfo( 1, 0, false),
            new ChessMoveInfo(-1,-1, false), new ChessMoveInfo( 0,-1, false), new ChessMoveInfo( 1,-1, false)
        };

        return boardInfo.GetValidMoves(turn, possibleMoves.ToList(), considerChecks);
    }
}
