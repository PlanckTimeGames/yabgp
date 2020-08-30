using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightPiece : ChessPiece
{
    public override PieceType Type { get { return PieceType.Knight; } }
    public override List<ChessTurn> CalculateValidMoves(ChessTurn turn, bool considerChecks)
    {
        var curPos = GetPosition() as ChessBoardPosition;
        ChessMoveInfo[] possibleMoves = new ChessMoveInfo[]
        {
                                            new ChessMoveInfo(-1, 2, false), new ChessMoveInfo(1, 2, false),
            new ChessMoveInfo(-2, 1, false),                                                                  new ChessMoveInfo(2, 1, false),
            new ChessMoveInfo(-2,-1, false),                                                                  new ChessMoveInfo(2,-1, false),
                                            new ChessMoveInfo(-1,-2, false), new ChessMoveInfo(1,-2, false)
        };

        return boardInfo.GetValidMoves(turn, possibleMoves.ToList(), considerChecks);
    }
}
