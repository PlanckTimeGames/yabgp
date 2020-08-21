using Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ChessTurn : ITurn
{
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
}
