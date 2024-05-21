using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleMgr : MonoBehaviour
{
    public GameObject gameManager;
    private GameMgr gameMgr;

    public GameObject quitMenu;

    private StringTable stringTable;
    private PlayerDataTable playerTable;
    private BossDataTable bossTable;
    private StageDataTable stageTable;

    //public Player player = new Player();
    public Boss boss = new Boss();

    public Image playerHpBar;
    public Image bossHpBar;

    private float playerOriginHp;
    private float bossOriginHp;

    public Animator playerAnimator;
    public Animator bossAnimator;

    public int gainedGold;

    public GameObject[] bossPrefabs;
    public GameObject battleMap;
    private GameObject bossPrefab;



    public void Start()
    {
        gameMgr = gameManager.GetComponent<GameMgr>();

        if (bossPrefab != null)
        {
            Destroy(bossPrefab);
        }
        bossPrefab = Instantiate(bossPrefabs[(int)gameMgr.currentStage - 3001], battleMap.transform);
        bossPrefab.transform.localPosition = new Vector3(1.3f, 0f, 0f);
        var bossRot = Quaternion.Euler(0,-130,0);
        bossPrefab.transform.rotation = bossRot;

        bossAnimator = bossPrefab.GetComponent<Animator>();

        stringTable = DataTableMgr.Get<StringTable>(DataTableIds.String);
        playerTable = DataTableMgr.Get<PlayerDataTable>(DataTableIds.PlayerTable);
        bossTable = DataTableMgr.Get<BossDataTable>(DataTableIds.BossTable);
        stageTable = DataTableMgr.Get<StageDataTable>(DataTableIds.StageTable);

        //TO-DO : 스테이지 별 체력, 공격력, 이미지 설정 추가
        //player.hp = playerTable.Get(DataTableIds.playerID).Player_Hp;
        //playerOriginHp = player.hp;
        //player.atk = playerTable.Get(DataTableIds.playerID).Player_Atk;
        //player.imageId = playerTable.Get(DataTableIds.playerID).Player_Image;

        Player.Instance.hp = playerTable.Get(DataTableIds.playerID).Player_Hp;
        playerOriginHp = Player.Instance.hp;
        Player.Instance.atk = playerTable.Get(DataTableIds.playerID).Player_Atk;
        Player.Instance.imageId = playerTable.Get(DataTableIds.playerID).Player_Image;

        string bossID = stageTable.Get(DataTableIds.stageID).Boss_ID;
        boss.name = stringTable.Get(bossTable.Get(bossID).Boss_Name);
        boss.hp = bossTable.Get(bossID).Boss_Hp;
        bossOriginHp = boss.hp;
        boss.atk = bossTable.Get(bossID).Boss_Atk;
        boss.imageId = bossTable.Get(bossID).Boss_Image;

        playerHpBar.fillAmount = Player.Instance.hp / playerOriginHp;
        bossHpBar.fillAmount = boss.hp / bossOriginHp;

        gameMgr.isPlayerDie = false;
        gameMgr.isBattleStageClear = false;

        quitMenu.SetActive(false);

        Debug.Log("플레이어 hp : " + Player.Instance.hp);
        Debug.Log("보스 ID : " + bossID);
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
        yield return StartCoroutine(PlayerTurn());
        if(!gameMgr.isBattleStageClear)
        {
            yield return StartCoroutine(BossTurn());
        }
        if(!gameMgr.isPlayerDie && !gameMgr.isBattleStageClear)
        {
            GoNextRound();
        }
    }
    private IEnumerator BossFirst()
    {
        Debug.Log("BossFirst Start");
        boss.atk *= 32;
        yield return StartCoroutine(BossTurn());

        if (!gameMgr.isBattleStageClear && !gameMgr.isPlayerDie)
        {
            yield return StartCoroutine(PlayerTurn());
            GoNextRound();
        }
    }

    private IEnumerator PlayerTurn()
    {
        if(!gameMgr.isPlayerDie)
        {
            StartCoroutine(gameMgr.PlayParticleSystem(gameMgr.playerParticle));
            yield return new WaitForSeconds(2f);

            playerAnimator.SetTrigger(AnimatorIds.playerAtkAni);
            bossAnimator.SetTrigger(AnimatorIds.bossDamagedAni);
            boss.hp -= Player.Instance.atk * gameMgr.maxValue;
            yield return new WaitForSeconds(2f);

            bossHpBar.fillAmount = boss.hp / bossOriginHp;
            CheckHealth();
            Debug.Log("Boss HP : " + boss.hp);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator BossTurn()
    {
        if (!gameMgr.isPlayerDie)
        {
            StartCoroutine(gameMgr.PlayBossParticleSystem(gameMgr.bossParticle, gameMgr.bossParticlePos));
            yield return new WaitForSeconds(2f);

            //왜 플레이어 피격이 먼저 재생되고 보스 공격 애니메이션이 재생될까..
            bossAnimator.SetTrigger(AnimatorIds.bossAtkAni);
            playerAnimator.SetTrigger(AnimatorIds.playerIdleAni);
            yield return new WaitForSeconds(1.5f);

            playerAnimator.SetTrigger(AnimatorIds.playerDamagedAni);

            Player.Instance.hp -= boss.atk * gameMgr.filledGridCount; 
            yield return new WaitForSeconds(2f);

            playerHpBar.fillAmount = Player.Instance.hp / playerOriginHp;
            CheckHealth();
            Debug.Log("Player HP : " + Player.Instance.hp);
            yield return new WaitForSeconds(2f);
        }
    }
    

    private void CheckHealth()
    {
        if (Player.Instance.hp <= 0)
        {
            playerAnimator.SetTrigger(AnimatorIds.playerDieAni);
            gameMgr.isPlayerDie = true;

            //Invoke("PlayerDie", 1f); 

            StopAllCoroutines();
        }
        else if (boss.hp <= 0)
        {
            bossAnimator.SetTrigger(AnimatorIds.bossDiedAni);
            gameMgr.isBattleStageClear = true;
            gainedGold = stageTable.Get(DataTableIds.stageID).Stage_Reward;
            Player.Instance.gold += gainedGold;

            StopAllCoroutines();
        }
        else
        {
            return;
        }

        //왜 이거 호출을 안할까?
        gameMgr.BattleOver();
    }

    private void GoNextRound()
    {
        if (Player.Instance.hp > 0 && boss.hp > 0)
        {
            Debug.Log("Go Next Round");
            gameMgr.StartNextRound();
        }
    }

    private void PlayerDie()
    {
        gameMgr.isPlayerDie = true;
    }

    public void OnClickQuit()
    {
        quitMenu.SetActive(true);
    }

    public void OnClickResume()
    {
        quitMenu.SetActive(false);
    }

    public void OnClickRestart()
    {
        gameMgr.RestartGame();
    }

    public void OnClickStage()
    {
        SceneManager.LoadScene("Stagebackground");
    }
}
