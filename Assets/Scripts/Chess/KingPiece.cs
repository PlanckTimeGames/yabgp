using Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KingPiece : ChessPiece
{
    private ChessMoveInfo mForwardMove;
    public override void Init(ChessBoardPosition position, IPlayer player, Vector3 forwardDirection, IChessBoardInfo boardInfo)
    {
        base.Init(position, player, forwardDirection, boardInfo);
        mForwardMove = new ChessMoveInfo((int)forwardDirection.x, (int)forwardDirection.z, false);
    }
    public override PieceType Type { get { return PieceType.King; } }
    public override List<ChessTurn> CalculateLegalTurns(ChessTurn turn, bool considerChecks, bool onlyCapturingMoves)
    {
        var curPos = GetPosition() as ChessBoardPosition;
        if (mPossibleMoves == null)
        {
            mPossibleMoves = new List<ChessMoveInfo>
            {
                new ChessMoveInfo(-1, 1, false), new ChessMoveInfo( 0, 1, false), new ChessMoveInfo( 1, 1, false),
                new ChessMoveInfo(-1, 0, false),                                  new ChessMoveInfo( 1, 0, false),
                new ChessMoveInfo(-1,-1, false), new ChessMoveInfo( 0,-1, false), new ChessMoveInfo( 1,-1, false)
            };

            mPossibleMoves.AddRange(CalcCastlingMoves());
        }

        return boardInfo.CalculateLegalTurnsFromRelativeMoves(turn, mPossibleMoves, considerChecks, onlyCapturingMoves);
    }
    private ChessMoveInfo[] CalcCastlingMoves()
    {
        var castlingtMoves = new ChessMoveInfo[2];
        if (mForwardMove.xOffset == 0)
        {
            castlingtMoves[0] = new ChessMoveInfo(-2, 0, false);
            castlingtMoves[1] = new ChessMoveInfo(2, 0, false);
        }
        else if (mForwardMove.yOffset == 0)
        {
            castlingtMoves[0] = new ChessMoveInfo(0, -2, false);
            castlingtMoves[1] = new ChessMoveInfo(0, 2, false);
        }
        return castlingtMoves;
    }
}
