using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public abstract class BoardPosition : MonoBehaviour
    {
        public virtual void Awake()
        {
            occupantPieces = new HashSet<Piece>();
        }
        public Vector3 vec3
        {
            // Assumes the pivot point of the position is in the exact center.
            get
            {
                Vector3 vertOffset = new Vector3(0, transform.localScale.y / 2f, 0);
                return transform.position - vertOffset;
            }
            set
            {
                Vector3 vertOffset = new Vector3(0, transform.localScale.y / 2f, 0);
                transform.position = value + vertOffset;
            }
        }
        public abstract void Highlight(bool show);
        public HashSet<Piece> occupantPieces { get; private set; }
        public bool AddOccupantPiece(Piece piece)
        {
            if (!piece)
                return false;
            return occupantPieces.Add(piece);
        }
        public bool RemoveOccupantPiece(Piece piece)
        {
            if (!piece)
                return false;
            return occupantPieces.Remove(piece);
        }

    }

}
