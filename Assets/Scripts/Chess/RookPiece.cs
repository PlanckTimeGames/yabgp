using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RookPiece : ChessPiece
{
    public override PieceType Type { get { return PieceType.Rook; } }
    public override List<ChessTurn> CalculateValidMoves(ChessTurn turn, bool considerChecks)
    {
        var curPos = GetPosition() as ChessBoardPosition;
        ChessMoveInfo[] possibleMoves = new ChessMoveInfo[]
        {
                                            new ChessMoveInfo( 0, 1, true),
            new ChessMoveInfo(-1, 0, true),                                 new ChessMoveInfo( 1, 0, true),
                                            new ChessMoveInfo( 0,-1, true)
        };

        return boardInfo.GetValidMoves(turn, possibleMoves.ToList(), considerChecks);
    }
}
