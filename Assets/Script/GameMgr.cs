using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public bool isTimeOver = false;
    public bool isGridFull = false;
    public bool isPlayerFirst = false;
    public bool isBossFirst = false;
    public bool isPlayerDie = false;
    public bool isBattleStageClear = false;

    public int filledGridCount;
    public int maxValue;

    private void Start()
    {
       
    }
    private void Update()
    {
        PuzzleOver();
    }

    private void PuzzleOver()
    {
        if(isTimeOver)
        {
            isPlayerFirst = true;
            isTimeOver = false;
        }

        if(isGridFull)
        {
            isBossFirst = true;
            isGridFull = false;
        }
    }

    private void BattleOver()
    {
        if(isPlayerDie)
        {
            //실패 창 띄우기
        }
        
        if(isBattleStageClear)
        {

        }
    }
}
