using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public bool isPuzzleOver = false;

    public bool isTimeOver = false;
    public bool isGridFull = false;
    public bool isPlayerFirst = false;
    public bool isBossFirst = false;
    public bool isPlayerDie = false;
    public bool isBattleStageClear = false;

    public int filledGridCount;
    public int maxValue;

    public GameObject puzzleManager;
    private PuzzleMgr puzzleMgr;
    public GameObject battleManager;
    private BattleMgr battleMgr;

    private void Start()
    {
       puzzleMgr = puzzleManager.GetComponent<PuzzleMgr>();
       battleMgr = battleManager.GetComponent<BattleMgr>();
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

    public void BattleOver()
    {
        if(isPlayerDie)
        {
            //���� â ����
            //���� ��� ȹ��
            Debug.Log("Player Die");
            //
            isPlayerDie = false;
        }
        
        if(isBattleStageClear)
        {
            //���� ��� ȹ��
            Debug.Log("Stage Clear");
            isBattleStageClear = false;
        }
    }
}
