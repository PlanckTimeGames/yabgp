using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interfaces
{
    public interface IGame
    {
        Material GetBoardMaterial();
        BoardPosition[] GetPositions();
        ITeam[] CreateTeams();
        Piece[] CreateInitPieces();
        ITurn[] TurnSequence { get; }
        List<ITurn> TurnHistory { get; set; }
        void StartGame();
    }

    public interface IPlayer
    {
        Color color { get; set; }
        Vector3 cameraPosition { get; set; }
        ITeam team { get; set; }
        HashSet<Piece> ownedPieces { get; }
        void AddPiece(Piece piece);
        void RemovePiece(Piece piece);
        void StartTurn(ITurn turn, EndTurnCallback endTurnCallback);
    }

    public interface ITeam
    {
        List<IPlayer> players { get; set; }
    }

    public interface ITurn
    {
        IPlayer player { get; set; }
        ITurn Clone();
    }

    public delegate void EndTurnCallback(ITurn endedTurn);
}


public class GameManager : MonoBehaviour
{
    public ChessGame chessGame;
    public IGame GetCurrentGame()
    {
        return chessGame as IGame;
    }
}
