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
    void PlayTurn(ChessTurn turnToPlay, MoveCompletionCallback callback);
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
    protected Dictionary<ChessPiece, List<ChessTurn>> mLegalTurns;
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
        CalculateAllLegalTurns();
    }

    protected virtual void EndTurn()
    {
        mLegalTurns.Clear();
        mEndTurnCallback(mTurn);
        mEndTurnCallback = null;
        mTurn = null;
    }

    public int GetNumLegalTurns()
    {
        CalculateAllLegalTurns();
        int numLegalTurns = 0;
        foreach (var legalTurns in mLegalTurns.Values)
        {
            numLegalTurns += legalTurns.Count;
        }
        return numLegalTurns;
    }
    private void CalculateAllLegalTurns()
    {
        if (mLegalTurns == null)
            mLegalTurns = new Dictionary<ChessPiece, List<ChessTurn>>();
        else if (mLegalTurns.Count > 0)
            return; // Turns already calculated.

        var turn = new ChessTurn(this);
        foreach (ChessPiece piece in ownedPieces)
        {
            turn.pieceMoved = piece;
            turn.startPosition = piece.GetPosition() as ChessBoardPosition;
            mLegalTurns[piece] = piece.CalculateLegalTurns(turn, true, false);
        }
    }
}

public class HumanChessPlayer : ChessPlayer 
{
    private ICameraController mCameraController;
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

                var legalTurnsForPiece = mLegalTurns[piece];

                foreach (var turn in legalTurnsForPiece)
                {
                    turn.endPosition.Select();
                }
                handled = true;
            }
        }

        if (!handled && mTurn.pieceMoved != null)
        {
            mChessBoardCommands.DeselectAllPositions();

            // Check if user made a move or cancelled it.
            var validMove = FindInLegalTurns(position);
            if (validMove == null)
            {
                // User clicked on a non-highlighted position, i.e. cancelled the move. Clear the selected piece info.
                mTurn.pieceMoved = null;
                mTurn.startPosition = null;
            }
            else
            {
                // User did a valid move. Proceed with moving the piece and cleanup.
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
                // ---- END CLEANUP ----

                // Actually move the piece.
                mChessBoardCommands.PlayTurn(mTurn, EndTurn);
            }
        }
    }

    private ChessTurn FindInLegalTurns(ChessBoardPosition position)
    {
        var selectedPiece = mTurn.pieceMoved;
        if (selectedPiece == null)
            return null;

        var legalTurnsForPiece = mLegalTurns[selectedPiece];
        foreach (var turn in legalTurnsForPiece)
        {
            if (turn.endPosition == position)
                return turn;
        }
        return null;
    }

}

