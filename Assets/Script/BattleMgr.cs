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

    private bool isPlayerTurn = false;
    private bool isBossTurn = false;
    

    private void Start()
    {
        gameMgr = gameManager.GetComponent<GameMgr>();

        stringTable = DataTableMgr.Get<StringTable>(DataTableIds.String);
        playerTable = DataTableMgr.Get<PlayerDataTable>(DataTableIds.PlayerTable);
        bossTable = DataTableMgr.Get<BossDataTable>(DataTableIds.BossTable);
        stageTable = DataTableMgr.Get<StageDataTable>(DataTableIds.StageTable);

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
            PlayerFirst();
            gameMgr.isPlayerFirst = false;
            
        }
        
        if(gameMgr.isBossFirst && !gameMgr.isPlayerFirst)
        {
            BossFirst();
            gameMgr.isBossFirst = false;
        }

        if(player.hp <= 0)
        {
            gameMgr.isPlayerDie = true;
        }
        else if(boss.hp <= 0)
        {
            gameMgr.isBattleStageClear = true;
        }
    }

    private void PlayerFirst()
    {
        Debug.Log("PlayerFirst Start");

        boss.hp -= player.atk * gameMgr.maxValue;
        player.hp -= boss.atk * gameMgr.filledGridCount;

        Debug.Log("Boss HP : " + boss.hp);
        Debug.Log("Player HP : " + player.hp);
    }   

    private void BossFirst()
    {
        Debug.Log("BossFirst Start");

        int penaltyAtk = 32 * boss.atk;
        player.hp -= penaltyAtk * gameMgr.filledGridCount;
        boss.hp -= player.atk * gameMgr.maxValue;

        Debug.Log("Player HP : " + player.hp);
        Debug.Log("Boss HP : " + boss.hp);
    }
}
