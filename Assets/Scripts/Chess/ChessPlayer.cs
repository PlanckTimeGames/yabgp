using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using System.Runtime.CompilerServices;
using PathCreation;
using System.Linq;

public delegate void MoveCompletionCallback();
public interface IChessBoardCommands
{
    ChessPiece MovePiece(ChessPiece pieceToMove, ChessBoardPosition toPosition, MoveCompletionCallback callback);
    void DeselectAllPositions();
}

public class ChessPlayer : IPlayer
{
    public enum PlayerColor
    {
        White = 1,
        Black
    }

    protected ChessTurn mTurn;
    protected IChessBoardCommands mChessBoardCommands;
    private EndTurnCallback mEndTurnCallback;

    public ChessPlayer(PlayerColor playerColor_, ChessTeam team_, IChessBoardCommands chessBoardCommands)
    {
        playerColor = playerColor_;
        team = team_;
        mChessBoardCommands = chessBoardCommands;
        ownedPieces = new HashSet<Piece>();
        IsInCheck = false;

        switch (playerColor)
        {
            case PlayerColor.White:
                color = new Color(0.9f, 0.9f, 0.9f);
                cameraPosition = new Vector3(0, 6f, -6.16f);
                forwardDirection = Vector3.forward;
                break;
            case PlayerColor.Black:
                color = new Color(0.2f, 0.2f, 0.2f);
                cameraPosition = new Vector3(0, 6f, 6.16f);
                forwardDirection = Vector3.back;
                break;
            default:
                color = Color.clear;
                cameraPosition = Vector3.zero;
                forwardDirection = Vector3.zero;
                break;
        }
    }

    public PlayerColor playerColor { get; set; }
    public Color color { get; set; }
    public Vector3 cameraPosition { get; set; }
    public ITeam team { get; set; }
    public Vector3 forwardDirection { get; private set; }
    public bool IsInCheck { get; set; }
    public HashSet<Piece> ownedPieces { get; private set; }
    public void AddPiece(Piece piece)
    {
        if (piece != null)
            ownedPieces.Add(piece);
    }
    public void RemovePiece(Piece piece)
    {
        if (piece != null)
            ownedPieces.Remove(piece);
    }

    public virtual void StartTurn(ITurn turn, EndTurnCallback endTurnCallback)
    {
        mTurn = turn as ChessTurn;
        mEndTurnCallback = endTurnCallback;
    }

    protected virtual void EndTurn()
    {
        mEndTurnCallback(mTurn);
        mEndTurnCallback = null;
        mTurn = null;
    }

}

public class HumanChessPlayer : ChessPlayer 
{
    private ICameraController mCameraController;
    private List<ChessTurn> mValidMoves;
    public HumanChessPlayer(PlayerColor playerColor_, ChessTeam team_, IChessBoardCommands chessBoardCommands, ICameraController cameraController)
        : base(playerColor_, team_, chessBoardCommands)
    {
        mCameraController = cameraController;
    }
    public override void StartTurn(ITurn turn, EndTurnCallback endTurnCallback)
    {
        base.StartTurn(turn, endTurnCallback);

        var startLoc = mCameraController.GetLocation();
        var endLoc = cameraPosition;
        if (startLoc != endLoc)
        {
            BezierPath cameraBPath = new BezierPath(new Vector3[] { startLoc, endLoc });
            cameraBPath.ControlPointMode = BezierPath.ControlMode.Free;
            var control1 = startLoc;
            control1.y = 0.0f;
            var horizDirection = new Vector2(control1.x, control1.z);
            horizDirection = Vector2.Perpendicular(horizDirection);
            control1 += new Vector3(horizDirection.x, 0.0f, horizDirection.y);
            cameraBPath.MovePoint(1, control1);

            var control2 = endLoc;
            control2.y = 0.0f;
            horizDirection = new Vector2(control2.x, control2.z);
            horizDirection = Vector2.Perpendicular(horizDirection);
            control2 -= new Vector3(horizDirection.x, 0.0f, horizDirection.y);
            cameraBPath.MovePoint(2, control2);

            var cameraPath = new VertexPath(cameraBPath, mCameraController.GetReferenceTransform());
            mCameraController.MoveCamera(cameraPath);
        }

        foreach (var piece in ownedPieces)
        {
            var position = piece.GetPosition() as ChessBoardPosition;
            position.userSelectionEnabled = true;
        }

        ChessGame.sPositionSelectedEvent.AddListener(OnPositionSelected);
    }

    private void OnPositionSelected(ChessBoardPosition position)
    {
        bool handled = false;
        if (position.occupantPieces.Count > 0)
        {
            var piece = position.occupantPieces.First() as ChessPiece;
            if (piece.ownerPlayer == this)
            {
                mChessBoardCommands.DeselectAllPositions();

                position.Select();
                mTurn.startPosition = position;
                mTurn.pieceMoved = piece;
                mValidMoves = piece.CalculateValidMoves(mTurn, true);

                foreach (var move in mValidMoves)
                {
                    move.endPosition.Select();
                }
                handled = true;
            }
        }

        if (!handled && mValidMoves != null)
        {
            mChessBoardCommands.DeselectAllPositions();
            var validMove = FindInValidMoves(position);
            if (validMove != null)
            {
                mTurn = validMove;
                
                // ---- START CLEANUP ----
                // Remove listener for position selections.
                ChessGame.sPositionSelectedEvent.RemoveListener(OnPositionSelected);
                // Disable position selection by user.
                foreach (var piece in ownedPieces)
                {
                    var piecePosition = piece.GetPosition() as ChessBoardPosition;
                    piecePosition.userSelectionEnabled = false;
                }
                // In addition, disable selection from move's fromPosition as well.
                mTurn.startPosition.userSelectionEnabled = false;
                // Clear turn-related variables.
                mValidMoves = null;
                // ---- END CLEANUP ----

                // Actually move the piece.
                mTurn.pieceCaptured = mChessBoardCommands.MovePiece(mTurn.pieceMoved, position, EndTurn);
            }
        }
    }

    private ChessTurn FindInValidMoves(ChessBoardPosition position)
    {
        foreach (var move in mValidMoves)
        {
            if (move.endPosition == position)
                return move;
        }
        return null;
    }

}

