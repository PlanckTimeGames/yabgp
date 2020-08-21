using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using PathCreation;
using System.Threading.Tasks;

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
    ChessBoardPosition GetRelativeBoardPosition(ChessBoardPosition startingPos, int xOffset, int yOffset);

    List<ChessBoardPosition> GetValidMoves(ChessPiece piece, ChessBoardPosition startingPos, List<ChessMoveInfo> possibleMoves);
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

    private bool mIsMoving;
    private VertexPath mAnimationPath;
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

    public virtual List<ChessBoardPosition> CalculateValidMoves()
    {
        return new List<ChessBoardPosition>();
    }

    public List<ChessBoardPosition> validPositions { get; protected set; }

    public void MovePiece(ChessBoardPosition newPos, VertexPath animationPath)
    {
        mIsMoving = true;
        mAnimationPath = animationPath;
        SetPosition(newPos);
    }

    private void NotifyPlayerAfterMove()
    {
        var player = ownerPlayer as ChessPlayer;
        player.PieceMoved(this);
    }
}

