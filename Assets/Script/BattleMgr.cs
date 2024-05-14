using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMgr : MonoBehaviour
{
    public GameObject gameManager;
    private GameMgr gameMgr;

    private StringTable stringTable;
    private PlayerDataTable playerTable;
    private BossDataTable bossTable;
    private StageDataTable stageTable;

    Player player = new Player();
    Boss boss = new Boss();

    private void Start()
    {
        gameMgr = gameManager.GetComponent<GameMgr>();

        stringTable = DataTableMgr.Get<StringTable>(DataTableIds.String);
        playerTable = DataTableMgr.Get<PlayerDataTable>(DataTableIds.PlayerTable);
        bossTable = DataTableMgr.Get<BossDataTable>(DataTableIds.BossTable);
        stageTable = DataTableMgr.Get<StageDataTable>(DataTableIds.StageTable);

        //TO-DO : 스테이지 별 체력, 공격력, 이미지 설정 추가
        player.hp = playerTable.Get(DataTableIds.playerID).Player_Hp;
        player.atk = playerTable.Get(DataTableIds.playerID).Player_Atk;
        player.imageId = playerTable.Get(DataTableIds.playerID).Player_Image;

        string bossID = stageTable.Get(DataTableIds.stageID).Boss_ID;
        boss.name = stringTable.Get(bossTable.Get(bossID).Boss_Name);
        boss.hp = bossTable.Get(bossID).Boss_Hp;
        boss.atk = bossTable.Get(bossID).Boss_Atk;
        boss.imageId = bossTable.Get(bossID).Boss_Image;

        gameMgr.isPlayerDie = false;
        gameMgr.isBattleStageClear = false;
    }

    private void Update()
    {
        if (gameMgr.isPlayerFirst && !gameMgr.isBossFirst)
        {
            StartCoroutine(PlayerFirst());
            gameMgr.isPlayerFirst = false;
        }
        
        if(gameMgr.isBossFirst && !gameMgr.isPlayerFirst)
        {
            StartCoroutine(BossFirst());
            gameMgr.isBossFirst = false;
        }
    }

    private IEnumerator PlayerFirst()
    {
        Debug.Log("PlayerFirst Start");
        if(!gameMgr.isBattleStageClear)
        {
            boss.hp -= player.atk * gameMgr.maxValue;
            CheckHealth();
            Debug.Log("Boss HP : " + boss.hp);
        }

        yield return new WaitForSeconds(2f);

        if(!gameMgr.isPlayerDie)
        {
            player.hp -= boss.atk * gameMgr.filledGridCount;
            Debug.Log("Player HP : " + player.hp);

            yield return new WaitForSeconds(2f);

            CheckHealth();
            GoNextRound();
        }
    }   

    private IEnumerator BossFirst()
    {
        Debug.Log("BossFirst Start");
        if(!gameMgr.isPlayerDie)
        {
            int penaltyAtk = 32 * boss.atk;
            player.hp -= penaltyAtk * gameMgr.filledGridCount;
            Debug.Log("Player HP : " + player.hp);

            yield return new WaitForSeconds(2f);

            CheckHealth();
        }
        
        yield return new WaitForSeconds(2f);

        if(!gameMgr.isBattleStageClear)
        {
            boss.hp -= player.atk * gameMgr.maxValue;
            Debug.Log("Boss HP : " + boss.hp);
            CheckHealth();
            GoNextRound();
        }
    }

    private void CheckHealth()
    {
        if (player.hp <= 0)
        {
            gameMgr.isPlayerDie = true;
            StopAllCoroutines();
        }
        else if (boss.hp <= 0)
        {
            gameMgr.isBattleStageClear = true;
            StopAllCoroutines();
        }
        else
        {
            return;
        }

        gameMgr.BattleOver();
    }

    private void GoNextRound()
    {
        if (player.hp > 0 && boss.hp > 0)
        {
            Debug.Log("Go Next Round");
            gameMgr.StartNextRound();
        }
    }
}
