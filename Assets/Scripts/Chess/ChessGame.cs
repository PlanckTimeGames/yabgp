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
using UnityEditor;

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

    public void TurnEnded(ITurn endedTurn)
    {
        TurnHistory.Add(endedTurn);
        Invoke("StartNextTurn", 0.01f);
    }

    public void StartNextTurn()
    {
        // Increment and wrap around the turn index.
        ++mCurrentTurnIdx;
        if (mCurrentTurnIdx >= TurnSequence.Length)
            mCurrentTurnIdx = 0;

        var turn = TurnSequence[mCurrentTurnIdx].Clone();
        var player = turn.player as ChessPlayer;
        player.IsInCheck = IsPlayerInCheck(player);

        if (0 == player.GetNumLegalTurns())
        {
            // Game Ended.
            if (player.IsInCheck)
                Debug.Log("Checkmate!!");
            else
                Debug.Log("Stalemate!!");
        }
        else
        {
            if (player.IsInCheck)
                Debug.Log("Check!!");
            player.StartTurn(turn, TurnEnded);
        }
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

    public void DeselectAllPositions()
    {
        foreach (var pos in mChessPositions)
        {
            if (pos.IsSelected)
                pos.Deselect();
        }
    }

    public void PlayTurn(ChessTurn turnToPlay, MoveCompletionCallback callback)
    {
        if (turnToPlay.pieceCaptured != null)
        {
            turnToPlay.pieceCaptured.Remove();
        }

        var startLoc = turnToPlay.startPosition.vec3;
        var endLoc = turnToPlay.endPosition.vec3;

        if (turnToPlay.specialFlag != ChessTurn.SpecialFlag.kKingRookCastle)
        {
            var pieceVPath = CreatePieceMovePath(startLoc, endLoc, 2.0f);
            turnToPlay.pieceMoved.MovePiece(turnToPlay.endPosition, pieceVPath, callback);
        }
        else
        {
            var pieceVPath = CreatePieceMovePath(startLoc, endLoc, 4.0f);
            turnToPlay.pieceMoved.MovePiece(turnToPlay.endPosition, pieceVPath, callback);

            var startLoc2 = turnToPlay.secondaryPieceMove.startPosition.vec3;
            var endLoc2 = turnToPlay.secondaryPieceMove.endPosition.vec3;
            var pieceVPath2 = CreatePieceMovePath(startLoc2, endLoc2, 1.0f);
            turnToPlay.secondaryPieceMove.pieceMoved.MovePiece(turnToPlay.secondaryPieceMove.endPosition, pieceVPath2, null);
        }
    }

    private VertexPath CreatePieceMovePath(Vector3 startLoc, Vector3 endLoc, float controlHeight)
    {
        BezierPath piecePath = new BezierPath(new Vector3[] { startLoc, endLoc });
        piecePath.ControlPointMode = BezierPath.ControlMode.Free;
        var control1 = startLoc;
        control1.y += controlHeight;
        piecePath.MovePoint(1, control1);
        var control2 = endLoc;
        control2.y += controlHeight;
        piecePath.MovePoint(2, control2);

        return new VertexPath(piecePath, transform);
    }


    private ChessTurn FindFirstMoveOfPiece(ChessPiece piece)
    {
        bool checkSecondaryPiece = (piece.Type == ChessPiece.PieceType.Rook); // Rooks can move as secondary piece during castling.
        foreach (ChessTurn turn in TurnHistory)
        {
            if (turn.pieceMoved == piece)
                return turn;
            else if (checkSecondaryPiece && turn.secondaryPieceMove.pieceMoved == piece)
                return turn;
        }
        return null;
    }

    private int FindNumMovesOfPiece(ChessPiece piece)
    {
        int turns = 0;
        bool checkSecondaryPiece = (piece.Type == ChessPiece.PieceType.Rook); // Rooks can move as secondary piece during castling.
        foreach (ChessTurn turn in TurnHistory)
        {
            if (turn.pieceMoved == piece)
                ++turns;
            else if (checkSecondaryPiece && turn.secondaryPieceMove.pieceMoved == piece)
                ++turns;
        }
        return turns;
    }

    public List<ChessTurn> CalculateLegalTurnsFromRelativeMoves(ChessTurn turn, List<ChessMoveInfo> possibleMoves, bool considerChecks, bool onlyCapturingMoves)
    {
        List<ChessTurn> validPositions;
        var piece = turn.pieceMoved;
        var startingPos = turn.startPosition;
        var ownTeam = piece.ownerPlayer.team;
        if (piece.Type == ChessPiece.PieceType.Pawn)
        {
            validPositions = CalculateLegalTurnsForPawn(turn, possibleMoves, onlyCapturingMoves);
        }
        else if (piece.Type == ChessPiece.PieceType.King)
        {
            validPositions = CalculateLegalTurnsForKing(turn, possibleMoves, onlyCapturingMoves);
        }
        else
        {
            validPositions = CalculateLegalTurnsGeneric(turn, possibleMoves);
        }

        if (considerChecks)
        {
            foreach (var turnToCheck in validPositions.Reverse<ChessTurn>())
            {
                if (IsPlayerInCheckAfterHypotheticalTurn(turnToCheck.chessPlayer, turnToCheck))
                    validPositions.Remove(turnToCheck);
            }
        }

        return validPositions;
    }

    private List<ChessTurn> CalculateLegalTurnsGeneric(ChessTurn turn, List<ChessMoveInfo> possibleMoves)
    {
        var legalTurns = new List<ChessTurn>();
        var startingPos = turn.startPosition;
        var ownTeam = turn.pieceMoved.ownerPlayer.team;
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
                    ChessPiece pieceToCapture = endingPos.occupantPieces.First() as ChessPiece;
                    if (pieceToCapture.ownerPlayer.team != ownTeam)
                    {
                        // Capturing move.
                        var turnCopy = turn.Clone() as ChessTurn;
                        turnCopy.endPosition = endingPos;
                        turnCopy.specialFlag = ChessTurn.SpecialFlag.kNormalTurn;
                        turnCopy.pieceCaptured = pieceToCapture;
                        legalTurns.Add(turnCopy);
                    }
                    break; // Don't iterate eny further because a piece is stopping us.
                }
                else
                {
                    // Empty position.
                    var turnCopy = turn.Clone() as ChessTurn;
                    turnCopy.endPosition = endingPos;
                    turnCopy.specialFlag = ChessTurn.SpecialFlag.kNormalTurn;
                    legalTurns.Add(turnCopy);
                    pos = endingPos;
                }
            } while (move.repeating);
        }
        return legalTurns;
    }

    private List<ChessTurn> CalculateLegalTurnsForPawn(ChessTurn turn, List<ChessMoveInfo> possibleMoves, bool onlyCapturingMoves)
    {
        // For pawn, the possible moves array needs to be hardcoded. Here are details of each index.
        // Index 0: Single forward move.
        // Index 1: Double forward move.
        // Index 2,3: Diagonal taking moves.
        // Index 4,5: En passant moves corresponding to the 2 and 3 taking moves.

        var validTurns = new List<ChessTurn>();
        var pawnPiece = turn.pieceMoved;
        var startingPos = turn.startPosition;
        var ownTeam = pawnPiece.ownerPlayer.team;

        if (!onlyCapturingMoves)
        {
            // Index 0: Single forward move to an empty position.
            var position = GetRelativeBoardPosition(startingPos, possibleMoves[0].xOffset, possibleMoves[0].yOffset);
            if (position != null && position.occupantPieces.Count == 0)
            {
                var turnCopy = turn.Clone() as ChessTurn;
                turnCopy.endPosition = position;
                turnCopy.specialFlag = ChessTurn.SpecialFlag.kNormalTurn;
                validTurns.Add(turnCopy);

                // Index 1: Double forward move to an empty position. Only enabled on pawn's first move.
                var doubleForwardPos = GetRelativeBoardPosition(startingPos, possibleMoves[1].xOffset, possibleMoves[1].yOffset);
                if (doubleForwardPos != null && doubleForwardPos.occupantPieces.Count == 0
                    && FindFirstMoveOfPiece(pawnPiece) == null)
                {
                    var turnCopy2 = turn.Clone() as ChessTurn;
                    turnCopy2.endPosition = doubleForwardPos;
                    turnCopy2.specialFlag = ChessTurn.SpecialFlag.kPawnDoubleMove;
                    validTurns.Add(turnCopy2);
                }
            }
        }

        // Index 2,3: Diagonal taking moves.
        for (var i = 2; i <= 3; ++i)
        {
            var move = possibleMoves[i];
            var position = GetRelativeBoardPosition(startingPos, move.xOffset, move.yOffset);

            if (position == null)
                continue;

            if (position.occupantPieces.Count > 0
                && position.occupantPieces.First().ownerPlayer.team != ownTeam)
            {
                var turnCopy = turn.Clone() as ChessTurn;
                turnCopy.pieceCaptured = position.occupantPieces.First() as ChessPiece;
                turnCopy.endPosition = position;
                turnCopy.specialFlag = ChessTurn.SpecialFlag.kNormalTurn;
                validTurns.Add(turnCopy);
                continue; // No need to check for en passant now.
            }


            // Index 4,5: En passant moves corresponding to the 2 and 3 taking moves.
            var enPassantMove = possibleMoves[i + 2];
            var enPassantPos = GetRelativeBoardPosition(startingPos, enPassantMove.xOffset, enPassantMove.yOffset);
            if (enPassantPos != null && enPassantPos.occupantPieces.Count > 0)
            {
                var enPassantPiece = enPassantPos.occupantPieces.First() as ChessPiece;
                if (IsEnPassantPossible(pawnPiece, enPassantPiece))
                {
                    var turnCopy = turn.Clone() as ChessTurn;
                    turnCopy.pieceCaptured = enPassantPiece;
                    turnCopy.endPosition = position;
                    turnCopy.specialFlag = ChessTurn.SpecialFlag.kPawnEnPassant;
                    turnCopy.enPassantPosition = enPassantPos;
                    validTurns.Add(turnCopy);
                }
            }
        }

        return validTurns;
    }

    private bool IsEnPassantPossible(ChessPiece pieceToMove, ChessPiece pieceToCapture)
    {
        if (pieceToCapture.ownerPlayer.team == pieceToMove.ownerPlayer.team)
            return false;

        if (pieceToCapture.Type != ChessPiece.PieceType.Pawn)
            return false;

        int pieceLastMoveIdx = -1, enemyPieceFirstMoveIdx = -1;
        for (int i = 0; i < TurnHistory.Count; ++i)
        {
            var turn = TurnHistory[i] as ChessTurn;
            if (turn.pieceMoved == pieceToMove)
                pieceLastMoveIdx = i;
            else if (turn.pieceMoved == pieceToCapture)
            {
                if (enemyPieceFirstMoveIdx >= 0)
                    return false; // Enemy pawn has moved more than once.
                else if (turn.specialFlag != ChessTurn.SpecialFlag.kPawnDoubleMove)
                    return false; // Enemy pawn's first move was not double move.
                else
                    enemyPieceFirstMoveIdx = i;
            }
        }
        if (pieceLastMoveIdx >= 0 && enemyPieceFirstMoveIdx >= 0 && pieceLastMoveIdx < enemyPieceFirstMoveIdx)
            return true;
        else
            return false;
    }

    private List<ChessTurn> CalculateLegalTurnsForKing(ChessTurn turn, List<ChessMoveInfo> possibleMoves, bool onlyCapturingMoves)
    {
        // For kings, the possible moves array needs to be hardcoded. Here are details of each index.
        // Index 0-7: Single moves in all directions.
        // Index 8-9: Castling moves.
        
        var genericMoves = possibleMoves.GetRange(0, 8);
        var legalTurns = CalculateLegalTurnsGeneric(turn, genericMoves);

        // Castling is not a capturing move. Just return the regular moves.
        if (onlyCapturingMoves)
            return legalTurns;
            
        var kingPiece = turn.pieceMoved;

        // King has moved or is under threat.
        if (FindFirstMoveOfPiece(kingPiece) != null || IsPieceUnderThreat(piece: kingPiece))
            return legalTurns;

        var castlingMoves = possibleMoves.GetRange(8, 2);
        foreach (var move in castlingMoves)
        {
            var incrementalMove = move;
            if (incrementalMove.xOffset != 0)
                incrementalMove.xOffset = incrementalMove.xOffset / Math.Abs(incrementalMove.xOffset);
            else if (incrementalMove.yOffset != 0)
                incrementalMove.yOffset = incrementalMove.yOffset / Math.Abs(incrementalMove.yOffset);

            var kingFinalPos = GetRelativeBoardPosition(turn.startPosition, move.xOffset, move.yOffset);
            var rookFinalPos = GetRelativeBoardPosition(turn.startPosition, incrementalMove.xOffset, incrementalMove.yOffset);

            var pos = turn.startPosition;
            bool checkForThreats = true;
            do
            {
                pos = GetRelativeBoardPosition(pos, incrementalMove.xOffset, incrementalMove.yOffset);
                if (pos == null)
                {
                    break; // We went off the board.
                }
                else if (pos.occupantPieces.Count > 0)
                {
                    ChessPiece rookPiece = pos.occupantPieces.First() as ChessPiece;
                    // Needs to be a rook of the same player who has never moved before.
                    if (rookPiece.ownerPlayer == turn.chessPlayer
                        && rookPiece.Type == ChessPiece.PieceType.Rook
                        && FindFirstMoveOfPiece(rookPiece) == null)
                    {
                        var turnCopy = turn.Clone() as ChessTurn;
                        turnCopy.endPosition = kingFinalPos;
                        turnCopy.specialFlag = ChessTurn.SpecialFlag.kKingRookCastle;

                        ChessTurn.SecondaryPieceMove secondaryPieceMove;
                        secondaryPieceMove.pieceMoved = rookPiece;
                        secondaryPieceMove.startPosition = pos;
                        secondaryPieceMove.endPosition = rookFinalPos;
                        turnCopy.secondaryPieceMove = secondaryPieceMove;

                        legalTurns.Add(turnCopy);
                    }
                    break; // Don't iterate any further because a piece is stopping us.
                }
                else if (checkForThreats)
                {
                    var turnCopy = turn.Clone() as ChessTurn;
                    turnCopy.endPosition = pos;
                    if (IsPieceUnderThreatAfterHypotheticalTurn(turnCopy, kingPiece))
                        break;
                }

                if (pos == kingFinalPos)
                    checkForThreats = false; // No need to check for threats after king's final position.
            } while (true);
        }
        return legalTurns;
    }

    private bool IsPieceUnderThreat(ChessPiece piece = null, ChessPiece.PieceType pieceType = 0, ChessPlayer player = null)
    {
        bool checkPieceType = false;
        if (piece != null)
            player = piece.ownerPlayer as ChessPlayer;
        else if (pieceType != 0 && player != null)
            checkPieceType = true;
        else
            return false;

        foreach (var pos in mChessPositions)
        {
            if (pos.occupantPieces.Count == 0)
                continue; // Empty position.

            ChessPiece enemyPiece = pos.occupantPieces.First() as ChessPiece;
            if (enemyPiece.ownerPlayer.team == player.team)
                continue; // Not an enemy piece.

            ChessTurn dummyTurn = new ChessTurn(enemyPiece.ownerPlayer as ChessPlayer);
            dummyTurn.pieceMoved = enemyPiece;
            dummyTurn.startPosition = pos;
            var enemyMoves = enemyPiece.CalculateLegalTurns(dummyTurn, false, true);

            foreach (var enemyTurn in enemyMoves)
            {
                var pieceCaptured = enemyTurn.pieceCaptured;
                if (pieceCaptured == null
                    || pieceCaptured.ownerPlayer != player)
                    continue;
                
                if (checkPieceType)
                {
                    if (pieceCaptured.Type == pieceType)
                        return true;
                }
                else
                {
                    if (pieceCaptured == piece)
                        return true;
                }
            }
        }

        return false;
    }

    private bool IsPieceUnderThreatAfterHypotheticalTurn(ChessTurn turn, ChessPiece piece = null, ChessPiece.PieceType pieceType = 0, ChessPlayer player = null)
    {
        bool isUnderThreat = false;
        using (var hypotheticalTurn = new HypotheticalTurn(turn))
        {
            isUnderThreat = IsPieceUnderThreat(piece, pieceType, player);
        }
        return isUnderThreat;
    }

    private bool IsPlayerInCheck(ChessPlayer player)
    {
        return IsPieceUnderThreat(pieceType: ChessPiece.PieceType.King, player: player);
    }

    private bool IsPlayerInCheckAfterHypotheticalTurn(ChessPlayer player, ChessTurn turn)
    {
        bool isInCheck = false;
        using (var hypotheticalTurn = new HypotheticalTurn(turn))
        {
            isInCheck = IsPlayerInCheck(player);
        }
        return isInCheck;
    }
}

