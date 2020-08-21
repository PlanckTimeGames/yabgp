using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using System.Runtime.CompilerServices;
using PathCreation;

public class ChessPlayer : IPlayer
{
    public enum PlayerColor
    {
        White = 1,
        Black
    }

    protected ChessTurn mTurn;
    private IEndTurnCallback mEndTurnCallback;

    public ChessPlayer(PlayerColor playerColor_, ChessTeam team_)
    {
        playerColor = playerColor_;
        team = team_;
        ownedPieces = new HashSet<Piece>();

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
    public virtual void PieceMoved(ChessPiece pieceMoved)
    {
        mEndTurnCallback.TurnEnded(mTurn);
    }

    public virtual void StartTurn(ITurn turn, IEndTurnCallback endTurnCallback)
    {
        mTurn = turn as ChessTurn;
        mEndTurnCallback = endTurnCallback;
    }
}

public class HumanChessPlayer : ChessPlayer 
{
    private ICameraController mCameraController;
    public HumanChessPlayer(PlayerColor playerColor_, ChessTeam team_, ICameraController cameraController)
        : base(playerColor_, team_)
    {
        mCameraController = cameraController;
    }
    public override void StartTurn(ITurn turn, IEndTurnCallback endTurnCallback)
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
    }

    public override void PieceMoved(ChessPiece pieceMoved)
    {
        foreach (var piece in ownedPieces)
        {
            var position = piece.GetPosition() as ChessBoardPosition;
            position.userSelectionEnabled = false;
        }
        // In addition, disable selection from move's fromPosition as well.
        mTurn.startPosition.userSelectionEnabled = false;
        base.PieceMoved(pieceMoved);
    }
}

