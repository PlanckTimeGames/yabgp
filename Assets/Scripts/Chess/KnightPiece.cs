﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightPiece : ChessPiece
{

    public override List<ChessBoardPosition> CalculateValidMoves()
    {
        var curPos = GetPosition() as ChessBoardPosition;
        ChessMoveInfo[] possibleMoves = new ChessMoveInfo[]
        {
                                            new ChessMoveInfo(-1, 2, false), new ChessMoveInfo(1, 2, false),
            new ChessMoveInfo(-2, 1, false),                                                                  new ChessMoveInfo(2, 1, false),
            new ChessMoveInfo(-2,-1, false),                                                                  new ChessMoveInfo(2,-1, false),
                                            new ChessMoveInfo(-1,-2, false), new ChessMoveInfo(1,-2, false)
        };

        return (validPositions = boardInfo.GetValidMoves(this, curPos, possibleMoves.ToList()));
    }
}