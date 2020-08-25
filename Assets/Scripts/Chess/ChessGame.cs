using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using UnityEngine.Timeline;
using System;
using System.Linq;
using PathCreation;
using System.Threading.Tasks;
using UnityEngine.Events;

public class ChessTeam : ITeam
{
    public ChessTeam() { players = new List<IPlayer>(); }
    public List<IPlayer> players { get; set; }
}

public class ChessGame : MonoBehaviour, IGame, IChessBoardInfo, IChessBoardCommands
{
    public static UnityEvent<ChessBoardPosition> sPositionSelectedEvent;

    public GameObject chessPositionPrefab;
    public ChessPieceFactory chessPieceFactory;
    public CameraRotation cameraRotation;

    //readonly (ChessPiece.PieceType, int)[] numPieces = new (ChessPiece.PieceType, int)[]
    //{
    //    ( ChessPiece.PieceType.King,   1 ),
    //    ( ChessPiece.PieceType.Queen,  1 ),
    //    ( ChessPiece.PieceType.Rook,   2 ),
    //    ( ChessPiece.PieceType.Bishop, 2 ),
    //    ( ChessPiece.PieceType.Knight, 2 ),
    //    ( ChessPiece.PieceType.Pawn,   8 ),
    //};

    readonly (float,float,ChessPiece.PieceType,ChessPlayer.PlayerColor)[,] kPositions = new (float, float, ChessPiece.PieceType, ChessPlayer.PlayerColor)[,]{
        { (-3.731f,-3.731f, ChessPiece.PieceType.Rook, ChessPlayer.PlayerColor.White), (-2.665f,-3.731f, ChessPiece.PieceType.Knight, ChessPlayer.PlayerColor.White), (-1.599f,-3.731f, ChessPiece.PieceType.Bishop, ChessPlayer.PlayerColor.White), (-0.533f,-3.731f, ChessPiece.PieceType.Queen, ChessPlayer.PlayerColor.White), (0.533f,-3.731f, ChessPiece.PieceType.King, ChessPlayer.PlayerColor.White), (1.599f,-3.731f, ChessPiece.PieceType.Bishop, ChessPlayer.PlayerColor.White), (2.665f,-3.731f, ChessPiece.PieceType.Knight, ChessPlayer.PlayerColor.White), (3.731f,-3.731f, ChessPiece.PieceType.Rook, ChessPlayer.PlayerColor.White) },
        { (-3.731f,-2.665f, ChessPiece.PieceType.Pawn, ChessPlayer.PlayerColor.White), (-2.665f,-2.665f, ChessPiece.PieceType.Pawn,   ChessPlayer.PlayerColor.White), (-1.599f,-2.665f, ChessPiece.PieceType.Pawn,   ChessPlayer.PlayerColor.White), (-0.533f,-2.665f, ChessPiece.PieceType.Pawn,  ChessPlayer.PlayerColor.White), (0.533f,-2.665f, ChessPiece.PieceType.Pawn, ChessPlayer.PlayerColor.White), (1.599f,-2.665f, ChessPiece.PieceType.Pawn,   ChessPlayer.PlayerColor.White), (2.665f,-2.665f, ChessPiece.PieceType.Pawn,   ChessPlayer.PlayerColor.White), (3.731f,-2.665f, ChessPiece.PieceType.Pawn, ChessPlayer.PlayerColor.White) },
        { (-3.731f,-1.599f, 0, 0), (-2.665f,-1.599f, 0, 0), (-1.599f,-1.599f, 0, 0), (-0.533f,-1.599f, 0, 0), (0.533f,-1.599f, 0, 0), (1.599f,-1.599f, 0, 0), (2.665f,-1.599f, 0, 0), (3.731f,-1.599f, 0, 0) },
        { (-3.731f,-0.533f, 0, 0), (-2.665f,-0.533f, 0, 0), (-1.599f,-0.533f, 0, 0), (-0.533f,-0.533f, 0, 0), (0.533f,-0.533f, 0, 0), (1.599f,-0.533f, 0, 0), (2.665f,-0.533f, 0, 0), (3.731f,-0.533f, 0, 0) },
        { (-3.731f, 0.533f, 0, 0), (-2.665f, 0.533f, 0, 0), (-1.599f, 0.533f, 0, 0), (-0.533f, 0.533f, 0, 0), (0.533f, 0.533f, 0, 0), (1.599f, 0.533f, 0, 0), (2.665f, 0.533f, 0, 0), (3.731f, 0.533f, 0, 0) },
        { (-3.731f, 1.599f, 0, 0), (-2.665f, 1.599f, 0, 0), (-1.599f, 1.599f, 0, 0), (-0.533f, 1.599f, 0, 0), (0.533f, 1.599f, 0, 0), (1.599f, 1.599f, 0, 0), (2.665f, 1.599f, 0, 0), (3.731f, 1.599f, 0, 0) },
        { (-3.731f, 2.665f, ChessPiece.PieceType.Pawn, ChessPlayer.PlayerColor.Black), (-2.665f, 2.665f, ChessPiece.PieceType.Pawn,   ChessPlayer.PlayerColor.Black), (-1.599f, 2.665f, ChessPiece.PieceType.Pawn,   ChessPlayer.PlayerColor.Black), (-0.533f, 2.665f, ChessPiece.PieceType.Pawn,  ChessPlayer.PlayerColor.Black), (0.533f, 2.665f, ChessPiece.PieceType.Pawn, ChessPlayer.PlayerColor.Black), (1.599f, 2.665f, ChessPiece.PieceType.Pawn,   ChessPlayer.PlayerColor.Black), (2.665f, 2.665f, ChessPiece.PieceType.Pawn,   ChessPlayer.PlayerColor.Black), (3.731f, 2.665f, ChessPiece.PieceType.Pawn, ChessPlayer.PlayerColor.Black) },
        { (-3.731f, 3.731f, ChessPiece.PieceType.Rook, ChessPlayer.PlayerColor.Black), (-2.665f, 3.731f, ChessPiece.PieceType.Knight, ChessPlayer.PlayerColor.Black), (-1.599f, 3.731f, ChessPiece.PieceType.Bishop, ChessPlayer.PlayerColor.Black), (-0.533f, 3.731f, ChessPiece.PieceType.Queen, ChessPlayer.PlayerColor.Black), (0.533f, 3.731f, ChessPiece.PieceType.King, ChessPlayer.PlayerColor.Black), (1.599f, 3.731f, ChessPiece.PieceType.Bishop, ChessPlayer.PlayerColor.Black), (2.665f, 3.731f, ChessPiece.PieceType.Knight, ChessPlayer.PlayerColor.Black), (3.731f, 3.731f, ChessPiece.PieceType.Rook, ChessPlayer.PlayerColor.Black) }
    };
    private ChessBoardPosition[,] mChessPositions = new ChessBoardPosition[8, 8];
    private ChessTeam mWhiteTeam, mBlackTeam;
    private Material mBoardMat;
    private ITurn[] mTurnSequence;
    private int mCurrentTurnIdx;

    public void Awake()
    {
        TurnHistory = new List<ITurn>();
        if (sPositionSelectedEvent == null)
            sPositionSelectedEvent = new UnityEvent<ChessBoardPosition>();
    }

    public Material GetBoardMaterial()
    {
        if (!mBoardMat)
            mBoardMat = Resources.Load<Material>("Materials/Chess/ChessBoardMat");
        return mBoardMat;
    }

    public ChessBoardPosition[,] GetChessPositions()
    {
        if (mChessPositions[0, 0] != null)
            return mChessPositions;

        for (int row = 0; row < 8; ++row)
        {
            for (int col = 0; col < 8; ++col)
            {
                var posObject = Instantiate(chessPositionPrefab);
                ChessBoardPosition pos = posObject.GetComponent<ChessBoardPosition>();
                pos.Init(row, col, new Vector3(kPositions[row, col].Item1, 0, kPositions[row, col].Item2));
                mChessPositions[row, col] = pos;
            }
        }
        return mChessPositions;
    }

    public BoardPosition[] GetPositions()
    {
        var chessPositions = GetChessPositions();
        var positions = new BoardPosition[64];
        int index = 0;
        for (int row = 0; row < 8; ++row)
        {
            for (int col = 0; col < 8; ++col)
            {
                positions[index] = chessPositions[row, col];
                ++index;
            }
        }
        return positions;
    }

    public ITeam[] CreateTeams()
    {
        mWhiteTeam = new ChessTeam();
        var whitePlayers = mWhiteTeam.players;
        whitePlayers.Add(new HumanChessPlayer(ChessPlayer.PlayerColor.White, mWhiteTeam, this, cameraRotation));
        mWhiteTeam.players = whitePlayers;

        mBlackTeam = new ChessTeam();
        var blackPlayers = mBlackTeam.players;
        blackPlayers.Add(new HumanChessPlayer(ChessPlayer.PlayerColor.Black, mBlackTeam, this, cameraRotation));
        mBlackTeam.players = blackPlayers;

        return new ITeam[] { mWhiteTeam, mBlackTeam };
    }

    public Piece[] CreateInitPieces()
    {
        Piece[] pieces = new Piece[32];
        int pieceIdx = 0;

        for (int row = 0; row < 8; ++row)
        {
            for (int col = 0; col < 8; ++col)
            {
                var position = kPositions[row, col];
                if (position.Item3 == 0)
                    continue;

                var team = (position.Item4 == ChessPlayer.PlayerColor.White) ? mWhiteTeam : mBlackTeam;
                var player = team.players.First() as ChessPlayer;
                pieces[pieceIdx] = CreatePiece(position.Item3, player, row, col);
                ++pieceIdx;
            }
        }

        return pieces;
    }

    public ChessPiece CreatePiece(ChessPiece.PieceType type, ChessPlayer player, int row, int col)
    {
        var piece = chessPieceFactory.CreatePiece(type);
        piece.Init(GetChessPositions()[row, col], player, player.forwardDirection, this);
        return piece;
    }

    public ITurn[] TurnSequence
    {
        get
        {
            if (mTurnSequence == null)
            {
                mTurnSequence = new ChessTurn[2]
                {
                    new ChessTurn(mWhiteTeam.players.First() as ChessPlayer),
                    new ChessTurn(mBlackTeam.players.First() as ChessPlayer)
                };
            }
            return mTurnSequence;
        }
    }
    public List<ITurn> TurnHistory { get; set; }

    public void StartGame()
    {
        mCurrentTurnIdx = 0;
        var turn = TurnSequence[mCurrentTurnIdx].Clone();
        turn.player.StartTurn(turn, TurnEnded);
    }

    public ChessBoardPosition GetRelativeBoardPosition(ChessBoardPosition startingPos, int xOffset, int yOffset)
    {
        int rowIdx = startingPos.rowIdx + yOffset;
        int colIdx = startingPos.colIdx + xOffset;
        if (rowIdx < 0 || rowIdx >= 8 || colIdx < 0 || colIdx >= 8)
            return null;
        else
            return GetChessPositions()[rowIdx, colIdx];
    }

    public List<ChessBoardPosition> GetValidMoves(ChessPiece piece, ChessBoardPosition startingPos, List<ChessMoveInfo> possibleMoves)
    {
        List<ChessBoardPosition> validPositions;
        var ownTeam = piece.ownerPlayer.team;
        if (piece.GetType() == typeof(PawnPiece))
        {
            return GetValidMovesForPawn(piece, startingPos, possibleMoves);
        }
        else
        {
            validPositions = new List<ChessBoardPosition>();
            foreach (var move in possibleMoves)
            {
                var pos = startingPos;
                do
                {
                    var endingPos = GetRelativeBoardPosition(pos, move.xOffset, move.yOffset);
                    if (endingPos == null)
                    {
                        break; // We went off the board.
                    }
                    else if (endingPos.occupantPieces.Count > 0)
                    {
                        if (endingPos.occupantPieces.First().ownerPlayer.team != ownTeam)
                            validPositions.Add(endingPos);
                        break; // Don't iterate eny further because a piece is stopping us.
                    }
                    else
                    {
                        validPositions.Add(endingPos); // Empty position.
                        pos = endingPos;
                    }
                } while (move.repeating);
            }
        }

        // TODO: Check for checking positions.

        return validPositions;
    }

    private List<ChessBoardPosition> GetValidMovesForPawn(ChessPiece pawnPiece, ChessBoardPosition startingPos, List<ChessMoveInfo> possibleMoves)
    {
        // For pawn, the possible moves array needs to be hardcoded. Here are details of each index.
        // Index 0: Single forward move.
        // Index 1: Double forward move.
        // Index 2,3: Diagonal taking moves.
        // Index 4,5: En passant moves corresponding to the 2 and 3 taking moves.

        var validPositions = new List<ChessBoardPosition>();
        var ownTeam = pawnPiece.ownerPlayer.team;

        // Index 0: Single forward move.
        var position = GetRelativeBoardPosition(startingPos, possibleMoves[0].xOffset, possibleMoves[0].yOffset);
        if (position != null && position.occupantPieces.Count == 0)
        {
            validPositions.Add(position); // Empty position.
        }

        // Index 1: Double forward move.
        position = GetRelativeBoardPosition(startingPos, possibleMoves[1].xOffset, possibleMoves[1].yOffset);
        // TODO: This needs move history to be implemented.

        // Index 2,3: Diagonal taking moves.
        for (var i = 2; i <= 3; ++i)
        {
            var move = possibleMoves[i];
            position = GetRelativeBoardPosition(startingPos, move.xOffset, move.yOffset);
            
            if (position != null
                && position.occupantPieces.Count > 0
                && position.occupantPieces.First().ownerPlayer.team != ownTeam)
            {
                validPositions.Add(position);
            }
        }

        // Index 4,5: En passant moves corresponding to the 2 and 3 taking moves.
        for (var i = 4; i <= 5; ++i)
        {
            var move = possibleMoves[i];
            position = GetRelativeBoardPosition(startingPos, move.xOffset, move.yOffset);

            if (position != null
                && position.occupantPieces.Count > 0
                && position.occupantPieces.First().ownerPlayer.team != ownTeam)
            {
                // TODO: Check for opposing piece move history.
                //var correspondingTakingMove = possibleMoves[i - 2];
                //var positionToAdd = GetRelativeBoardPosition(startingPos, correspondingTakingMove.xOffset, correspondingTakingMove.yOffset);
                //if (positionToAdd != null)
                //    validPositions.Add(positionToAdd);
            }
        }

        return validPositions;
    }

    public void DeselectAllPositions()
    {
        foreach (var pos in mChessPositions)
        {
            if (pos.IsSelected)
                pos.Deselect();
        }
    }

    public ChessPiece MovePiece(ChessPiece pieceToMove, ChessBoardPosition toPosition, MoveCompletionCallback callback)
    {
        ChessPiece pieceCaptured = null;
        if (toPosition.occupantPieces.Count() > 0)
        {
            var piece = toPosition.occupantPieces.First();
            piece.Remove();
            pieceCaptured = piece as ChessPiece;

            // TODO: Check for en passant capture.
        }

        var startLoc = pieceToMove.GetPosition().vec3;
        var endLoc = toPosition.vec3;
        BezierPath piecePath = new BezierPath(new Vector3[] { startLoc, endLoc });
        piecePath.ControlPointMode = BezierPath.ControlMode.Free;
        var control1 = startLoc;
        control1.y += 2.0f;
        piecePath.MovePoint(1, control1);
        var control2 = endLoc;
        control2.y += 2.0f;
        piecePath.MovePoint(2, control2);
        
        var pieceVPath = new VertexPath(piecePath, transform);

        pieceToMove.MovePiece(toPosition, pieceVPath, callback);
        return pieceCaptured;
    }

    public void TurnEnded(ITurn endedTurn)
    {
        TurnHistory.Add(endedTurn);
        Invoke("StartNextTurn", 0.01f);
    }

    public void StartNextTurn()
    {
        mCurrentTurnIdx++;
        if (mCurrentTurnIdx >= TurnSequence.Length)
            mCurrentTurnIdx = 0;

        var turn = TurnSequence[mCurrentTurnIdx].Clone();
        turn.player.StartTurn(turn, TurnEnded);
    }    
}
