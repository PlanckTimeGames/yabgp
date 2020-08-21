using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BishopPiece : ChessPiece
{

    public override List<ChessBoardPosition> CalculateValidMoves()
    {
        var curPos = GetPosition() as ChessBoardPosition;
        ChessMoveInfo[] possibleMoves = new ChessMoveInfo[]
        {
            new ChessMoveInfo( 1, 1, true), new ChessMoveInfo( 1,-1, true),
            new ChessMoveInfo(-1, 1, true), new ChessMoveInfo(-1,-1, true)
        };

        return (validPositions = boardInfo.GetValidMoves(this, curPos, possibleMoves.ToList()));
    }
}
