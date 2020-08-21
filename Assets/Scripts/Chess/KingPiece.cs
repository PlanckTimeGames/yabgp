using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KingPiece : ChessPiece
{

    public override List<ChessBoardPosition> CalculateValidMoves()
    {
        var curPos = GetPosition() as ChessBoardPosition;
        ChessMoveInfo[] possibleMoves = new ChessMoveInfo[]
        {
            new ChessMoveInfo(-1, 1, false), new ChessMoveInfo( 0, 1, false), new ChessMoveInfo( 1, 1, false),
            new ChessMoveInfo(-1, 0, false),                                  new ChessMoveInfo( 1, 0, false),
            new ChessMoveInfo(-1,-1, false), new ChessMoveInfo( 0,-1, false), new ChessMoveInfo( 1,-1, false)
        };

        return (validPositions = boardInfo.GetValidMoves(this, curPos, possibleMoves.ToList()));
    }
}
