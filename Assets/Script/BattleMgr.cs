using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Image playerHpBar;
    public Image bossHpBar;

    private float playerOriginHp;
    private float bossOriginHp;

    public Animator playerAnimator;
    public Animator bossAnimator;

    private void Start()
    {
        gameMgr = gameManager.GetComponent<GameMgr>();

        stringTable = DataTableMgr.Get<StringTable>(DataTableIds.String);
        playerTable = DataTableMgr.Get<PlayerDataTable>(DataTableIds.PlayerTable);
        bossTable = DataTableMgr.Get<BossDataTable>(DataTableIds.BossTable);
        stageTable = DataTableMgr.Get<StageDataTable>(DataTableIds.StageTable);

        //TO-DO : 스테이지 별 체력, 공격력, 이미지 설정 추가
        player.hp = playerTable.Get(DataTableIds.playerID).Player_Hp;
        playerOriginHp = player.hp;
        player.atk = playerTable.Get(DataTableIds.playerID).Player_Atk;
        player.imageId = playerTable.Get(DataTableIds.playerID).Player_Image;

        string bossID = stageTable.Get(DataTableIds.stageID).Boss_ID;
        boss.name = stringTable.Get(bossTable.Get(bossID).Boss_Name);
        boss.hp = bossTable.Get(bossID).Boss_Hp;
        bossOriginHp = boss.hp;
        boss.atk = bossTable.Get(bossID).Boss_Atk;
        boss.imageId = bossTable.Get(bossID).Boss_Image;

        gameMgr.isPlayerDie = false;
        gameMgr.isBattleStageClear = false;

        playerAnimator.SetBool(AnimatorIds.playerAtkAni, false);
        bossAnimator.SetBool(AnimatorIds.bossAtkAni, false);
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
            playerAnimator.SetBool(AnimatorIds.playerAtkAni, true);
            bossAnimator.SetBool(AnimatorIds.bossDamagedAni, true);
            boss.hp -= player.atk * gameMgr.maxValue;
            yield return new WaitForSeconds(2f);

            bossHpBar.fillAmount = boss.hp / bossOriginHp;
            Debug.Log("Boss HP : " + boss.hp);

            playerAnimator.SetBool(AnimatorIds.playerAtkAni, false);
            bossAnimator.SetBool(AnimatorIds.bossDamagedAni, false);
            yield return new WaitForSeconds(2f);
            CheckHealth();
        }

        if(!gameMgr.isPlayerDie)
        {
            bossAnimator.SetBool(AnimatorIds.bossAtkAni, true);
            playerAnimator.SetBool(AnimatorIds.playerDamaedAni, true);
            player.hp -= boss.atk * gameMgr.filledGridCount;
            yield return new WaitForSeconds(2f);

            playerHpBar.fillAmount = player.hp / playerOriginHp;
            Debug.Log("Player HP : " + player.hp);

            bossAnimator.SetBool(AnimatorIds.bossAtkAni, false);
            playerAnimator.SetBool(AnimatorIds.playerDamaedAni, false);
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
            bossAnimator.SetBool(AnimatorIds.bossAtkAni, true);
            playerAnimator.SetBool(AnimatorIds.playerDamaedAni, true);
            player.hp -= penaltyAtk * gameMgr.filledGridCount;
            playerHpBar.fillAmount = player.hp / playerOriginHp;
            Debug.Log("Player HP : " + player.hp);

            yield return new WaitForSeconds(2f);
            bossAnimator.SetBool(AnimatorIds.bossAtkAni, false);
            playerAnimator.SetBool(AnimatorIds.playerDamaedAni, false);
            CheckHealth();
        }
        
        if(!gameMgr.isBattleStageClear)
        {
            playerAnimator.SetBool(AnimatorIds.playerAtkAni, true);
            bossAnimator.SetBool(AnimatorIds.bossDamagedAni, true);
            boss.hp -= player.atk * gameMgr.maxValue;
            bossHpBar.fillAmount = boss.hp / bossOriginHp;
            Debug.Log("Boss HP : " + boss.hp);

            yield return new WaitForSeconds(2f);
            playerAnimator.SetBool(AnimatorIds.playerAtkAni, false);
            bossAnimator.SetBool(AnimatorIds.bossDamagedAni, false);
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
