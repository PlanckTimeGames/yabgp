using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using PathCreation;

public struct ChessMoveInfo
{
    public int xOffset;
    public int yOffset;
    public bool repeating;

    public ChessMoveInfo(int xOffset, int yOffset, bool repeating)
    {
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.repeating = repeating;
    }
}
public interface IChessBoardInfo
{
    List<ChessTurn> CalculateLegalTurnsFromRelativeMoves(ChessTurn turn, List<ChessMoveInfo> possibleMoves, bool considerChecks, bool onlyCapturingMoves);
}

public abstract class ChessPiece : Piece
{
    public enum PieceType
    {
        King = 1,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }

    protected List<ChessMoveInfo> mPossibleMoves;

    private bool mIsMoving;
    private VertexPath mAnimationPath;
    private MoveCompletionCallback mMoveCompletionCallback;
    private float mMoveSpeed = 20.0f;
    private float mMoveDistance = 0.0f;
    public virtual void Init(ChessBoardPosition position, IPlayer player, Vector3 forwardDirection, IChessBoardInfo boardInfo)
    {
        base.Init(position, player);
        transform.position = position.vec3;
        transform.forward = forwardDirection;
        this.boardInfo = boardInfo;

        var renderer = GetComponent<Renderer>();
        var mat = renderer.material;
        if (mat)
        {
            mat = Instantiate<Material>(mat);
            mat.color = ownerPlayer.color;
            renderer.material = mat;
        }
    }

    public override void Remove()
    {
        base.Remove();
        var renderer = GetComponent<Renderer>();
        renderer.enabled = false;
    }

    public virtual PieceType Type { get; }

    void Update()
    {
        if (mIsMoving)
        {
            mMoveDistance += mMoveSpeed * Time.deltaTime;
            if (mMoveDistance < mAnimationPath.length)
                transform.position = mAnimationPath.GetPointAtDistance(mMoveDistance, EndOfPathInstruction.Stop);
            else
            {
                mIsMoving = false;
                mMoveDistance = 0;
                mAnimationPath = null;
                transform.position = GetPosition().vec3;

                Invoke("NotifyPlayerAfterMove", 0.01f);
            }
        }
    }

    public IChessBoardInfo boardInfo { get; private set; }

    public virtual List<ChessTurn> CalculateLegalTurns(ChessTurn turn, bool considerChecks, bool onlyCapturingMoves)
    {
        return new List<ChessTurn>();
    }

    public void MovePiece(ChessBoardPosition newPos, VertexPath animationPath, MoveCompletionCallback moveCompletionCallback)
    {
        mIsMoving = true;
        mAnimationPath = animationPath;
        mMoveCompletionCallback = moveCompletionCallback;
        SetPosition(newPos);
    }

    private void NotifyPlayerAfterMove()
    {
        mMoveCompletionCallback();
        mMoveCompletionCallback = null;
    }
}

