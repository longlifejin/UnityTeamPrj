using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private PuzzleManager puzzleMgr;
    public GameObject battleManager;
    private BattleMgr battleMgr;

    public Store store;

    public Image popUpPanel;
    TextMeshProUGUI popUpMessage;

    public ParticleSystem testParticle;
    public Vector2 playerParticlePos;

    private void Start()
    {
        puzzleMgr = puzzleManager.GetComponent<PuzzleManager>();
        battleMgr = battleManager.GetComponent<BattleMgr>();
        popUpPanel.gameObject.SetActive(false);
        popUpMessage = popUpPanel.GetComponentInChildren<TextMeshProUGUI>();
        testParticle.Stop();
    }
    private void Update()
    {
        PuzzleOver();
    }

    private void PuzzleOver()
    {
        if (isTimeOver)
        {
            isPlayerFirst = true;
            isTimeOver = false;
            testParticle.transform.position = playerParticlePos;
            testParticle.Play();
        }

        if (isGridFull)
        {
            Debug.Log("GridFull");
            isBossFirst = true;
            isGridFull = false;
        }
    }

    public void BattleOver()
    {
        if (isPlayerDie)
        {
            popUpPanel.gameObject.SetActive(true);
            popUpMessage.text = $"Stage Failed!\n gained Gold : 0";
            Debug.Log("Player Die");
        }

        if (isBattleStageClear)
        {
            popUpPanel.gameObject.SetActive(true);
            popUpMessage.text = $"Stage Clear!\n gained Gold : {battleMgr.gainedGold}";
            Debug.Log("Stage Clear");
        }
    }

    public void StartNextRound()
    {
        puzzleMgr.NewGame();
    }

    public void OnClickOK()
    {
        popUpPanel.gameObject.SetActive(false);

        if(isPlayerDie || isBattleStageClear)
        {
            battleMgr.Start();
        }

        isPlayerDie = false;
        isBattleStageClear = false;
    }
}
