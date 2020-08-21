using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using UnityEngine.ProBuilder;

public class BoardScript : MonoBehaviour
{
    public GameManager gameMgr;
    private BoardPosition[] mBoardPositions;
    
    
    // Start is called before the first frame update
    void Start()
    {
        IGame game = gameMgr.GetCurrentGame();
        Material boardMat = game.GetBoardMaterial();

        Renderer rend = GetComponent<Renderer>();
        Material[] mats = rend.materials;
        mats[1] = boardMat;
        rend.materials = mats;

        mBoardPositions = game.GetPositions();
        game.CreateTeams();
        game.CreateInitPieces();
        game.StartGame();
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
