using Interfaces;
using System;

public class ChessTurn : ITurn
{
    public enum SpecialFlag
    {
        kNormalTurn = 0,
        kPawnDoubleMove,
        kPawnEnPassant,
        kKingRookCastle
    }
    public ChessTurn(ChessPlayer chessPlayer)
    {
        this.chessPlayer = chessPlayer;
    }

    public ITurn Clone()
    {
        var clone = new ChessTurn(chessPlayer);
        clone.pieceMoved    = pieceMoved;
        clone.pieceCaptured = pieceCaptured;
        clone.startPosition = startPosition;
        clone.endPosition   = endPosition;
        clone.specialFlag = specialFlag;
        clone.enPassantPosition = enPassantPosition;
        clone.secondaryPieceMove = secondaryPieceMove;
        return clone;
    }

    public ChessPlayer chessPlayer { get; set; }

    public IPlayer player
    {
        get { return chessPlayer; }
        set { chessPlayer = value as ChessPlayer; }
    }

    public ChessPiece pieceMoved    { get; set; }
    public ChessPiece pieceCaptured { get; set; }
    public ChessBoardPosition startPosition { get; set; }
    public ChessBoardPosition endPosition   { get; set; }
    public SpecialFlag specialFlag { get; set; }

    public ChessBoardPosition enPassantPosition { get; set; }

    /// <summary>
    ///  Used primarily for castling.
    /// </summary>
    public struct SecondaryPieceMove
    {
        public ChessPiece pieceMoved;
        public ChessBoardPosition startPosition;
        public ChessBoardPosition endPosition;
    }
     public SecondaryPieceMove secondaryPieceMove { get; set; }
}


public class HypotheticalTurn : IDisposable
{
    private ChessTurn mTurn;
    private BoardPosition mCapturedPiecePos;
    public HypotheticalTurn(ChessTurn turn)
    {
        mTurn = turn;

        if (mTurn.pieceCaptured != null)
        {
            mCapturedPiecePos = mTurn.pieceCaptured.GetPosition();
            mTurn.pieceCaptured.SetPosition(null);
        }

        mTurn.pieceMoved.SetPosition(mTurn.endPosition);
    }
    public void Dispose()
    {
        mTurn.pieceMoved.SetPosition(mTurn.startPosition);

        if (mTurn.pieceCaptured != null)
            mTurn.pieceCaptured.SetPosition(mCapturedPiecePos);
    }
}