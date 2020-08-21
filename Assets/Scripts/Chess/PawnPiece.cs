using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : ChessPiece
{
    private ChessMoveInfo mForwardMove;
    public override void Init(ChessBoardPosition position, IPlayer player, Vector3 forwardDirection, IChessBoardInfo boardInfo)
    {
        base.Init(position, player, forwardDirection, boardInfo);
        mForwardMove = new ChessMoveInfo((int)forwardDirection.x, (int)forwardDirection.z, false);
    }

    public override List<ChessBoardPosition> CalculateValidMoves()
    {
        var curPos = GetPosition() as ChessBoardPosition;
        List<ChessMoveInfo> possibleMoves = new List<ChessMoveInfo>()
        {
            // 2 kinds of forward moves.
            mForwardMove, new ChessMoveInfo(2 * mForwardMove.xOffset, 2 * mForwardMove.yOffset, false)
        };

        possibleMoves.AddRange(CalcTakingMoves());
        possibleMoves.AddRange(CalcEnPassantMoves());

        return (validPositions = boardInfo.GetValidMoves(this, curPos, possibleMoves));
    }

    private ChessMoveInfo[] CalcTakingMoves()
    {
        var takingMoves = new ChessMoveInfo[2];
        if (mForwardMove.xOffset == 0)
        {
            takingMoves[0] = new ChessMoveInfo(-1, mForwardMove.yOffset, false);
            takingMoves[1] = new ChessMoveInfo( 1, mForwardMove.yOffset, false);
        }
        else if (mForwardMove.yOffset == 0)
        {
            takingMoves[0] = new ChessMoveInfo(mForwardMove.xOffset, -1, false);
            takingMoves[1] = new ChessMoveInfo(mForwardMove.xOffset,  1, false);
        }
        return takingMoves;
    }

    private ChessMoveInfo[] CalcEnPassantMoves()
    {
        var enPassantMoves = new ChessMoveInfo[2];
        if (mForwardMove.xOffset == 0)
        {
            enPassantMoves[0] = new ChessMoveInfo(-1, 0, false);
            enPassantMoves[1] = new ChessMoveInfo(1, 0, false);
        }
        else if (mForwardMove.yOffset == 0)
        {
            enPassantMoves[0] = new ChessMoveInfo(0, -1, false);
            enPassantMoves[1] = new ChessMoveInfo(0, 1, false);
        }
        return enPassantMoves;
    }
}
