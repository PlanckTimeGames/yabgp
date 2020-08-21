using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public abstract class Piece : MonoBehaviour
    {
        BoardPosition mPosition;

        public virtual void Init(ChessBoardPosition position, IPlayer player)
        {
            SetPosition(position);
            ownerPlayer = player;
            ownerPlayer.AddPiece(this);
        }

        public virtual void Remove()
        {
            if (ownerPlayer != null)
                ownerPlayer.RemovePiece(this);
            SetPosition(null);
        }
        public virtual void SetPosition(BoardPosition position)
        {
            if (mPosition == position)
                return;

            if (mPosition != null)
                mPosition.RemoveOccupantPiece(this);
            mPosition = position;
            if (mPosition != null)
            {
                mPosition.AddOccupantPiece(this);
            }
        }
        public BoardPosition GetPosition()
        {
            return mPosition;
        }

        public IPlayer ownerPlayer { get; set; }
    }
}