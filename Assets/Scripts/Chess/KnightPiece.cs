using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightPiece : ChessPiece
{
    public override PieceType Type { get { return PieceType.Knight; } }
    public override List<ChessTurn> CalculateLegalTurns(ChessTurn turn, bool considerChecks, bool onlyCapturingMoves)
    {
        var curPos = GetPosition() as ChessBoardPosition;
        if (mPossibleMoves == null)
        {
            mPossibleMoves = new List<ChessMoveInfo>
            {
                                                new ChessMoveInfo(-1, 2, false), new ChessMoveInfo(1, 2, false),
                new ChessMoveInfo(-2, 1, false),                                                                  new ChessMoveInfo(2, 1, false),
                new ChessMoveInfo(-2,-1, false),                                                                  new ChessMoveInfo(2,-1, false),
                                                new ChessMoveInfo(-1,-2, false), new ChessMoveInfo(1,-2, false)
            };
        }

        return boardInfo.CalculateLegalTurnsFromRelativeMoves(turn, mPossibleMoves, considerChecks, onlyCapturingMoves);
    }
}
