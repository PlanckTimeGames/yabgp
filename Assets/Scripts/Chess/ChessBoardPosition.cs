using Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class ChessBoardPosition : SquarePosition
{
    private bool mSelected = false;

    public int rowIdx { get; private set; }
    public int colIdx { get; private set; }

    public void Init(int rowIdx_, int colIdx_, Vector3 vec3Position)
    {
        rowIdx = rowIdx_;
        colIdx = colIdx_;
        vec3 = vec3Position;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hide at the beginning.
        GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    private void OnMouseEnter()
    {
        if (occupantPieces.Count > 0 && userSelectionEnabled)
            Highlight(true);
    }

    private void OnMouseExit()
    {
        if (!mSelected)
            Highlight(false);
    }
    
    private void OnMouseDown()
    {
        ChessGame.sPositionSelectedEvent.Invoke(this);
    }

    public void Select()
    {
        mSelected = true;
        Highlight(true);
    }

    public void Deselect()
    {
        mSelected = false;
        Highlight(false);
    }

    public bool IsSelected { get { return mSelected; } }

    public bool userSelectionEnabled { get; set; }

    public override void Highlight(bool show) 
    {
        GetComponent<Renderer>().enabled = show;
    }
}
