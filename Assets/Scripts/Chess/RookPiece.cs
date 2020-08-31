using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RookPiece : ChessPiece
{
    public override PieceType Type { get { return PieceType.Rook; } }
    public override List<ChessTurn> CalculateLegalTurns(ChessTurn turn, bool considerChecks, bool onlyCapturingMoves)
    {
        var curPos = GetPosition() as ChessBoardPosition;
        if (mPossibleMoves == null)
        {
            mPossibleMoves = new List<ChessMoveInfo>
            {
                                                new ChessMoveInfo( 0, 1, true),
                new ChessMoveInfo(-1, 0, true),                                 new ChessMoveInfo( 1, 0, true),
                                                new ChessMoveInfo( 0,-1, true)
            };
        }

        return boardInfo.CalculateLegalTurnsFromRelativeMoves(turn, mPossibleMoves, considerChecks, onlyCapturingMoves);
    }
}
