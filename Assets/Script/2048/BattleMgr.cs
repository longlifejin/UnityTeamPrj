//using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class BattleMgr : MonoBehaviour
{
    public GameObject gameManager;
    private GameMgr gameMgr;

    public GameObject quitMenu;

    private StringTable stringTable;
    private PlayerDataTable playerTable;
    private BossDataTable bossTable;
    private StageDataTable stageTable;

    public Boss boss = new Boss();
    private GameObject player;

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

    private bool bossSpecialAttack = false;

    public GameObject battleGround;
    public RawImage battleBack;

    private AudioSource battleAudioSource;
    public AudioClip[] battleBGMClips;


    public ParticleSystem[] bossAttackParticles;
    public AudioClip[] bossAttackAudioes;
    
    public ParticleSystem[] bossSpecialAttackParticles;
    public AudioSource bossAudioSource;
    public AudioClip[] bossSpecialAttackAudioes;

    public ParticleSystem[] playerChargingParticles;
    public AudioClip[] playerChargingAudioes;

    public ParticleSystem[] playerAttackParticles;
    public AudioSource playerAudioSource;
    public AudioClip[] playerAttackAudioes;

    public GameObject playerfloatingDamage;
    public GameObject bossfloatingDamage;

    public DynamicTextData floatTextData;

    public bool is16Attack = false;
    public bool isBossAttack = false; 
    
    float timer;
    float bossAttackInterval = 2f;

    int currentBossAttackPattern;
    private Dictionary<BossSkill, System.Action> bossSkillActions;

    public Vector3 playerPos;
    public Vector3 bossPos;

    public enum BossSkill
    {
        Normal = 1,
        StopSwipe,
        ReverseSwipe,
        SecretePuzzle,
    }

    private void BossNormalAttack()
    {
        bossAnimator.SetTrigger(AnimatorIds.bossSpecialAtkAni);
        playerAnimator.SetTrigger(AnimatorIds.playerDamagedAni);
        //Player.Instance.hp -= boss.atk * gameMgr.filledGridCount;
        Player.Instance.hp -= 10;
        Vector3 pos = new Vector3(-2.45f, 2f, -0.5f);
        ShowDamage(10, pos, Color.white);

        playerHpBar.fillAmount = Player.Instance.hp / playerOriginHp;
        CheckHealth();

        gameMgr.isStopAttack = false;
        gameMgr.isReverseSAttack = false;
        gameMgr.isSecreteAttack = false;
    }

    private void BossStopSwipe()
    {
        Debug.Log("StopSwipe");
        gameMgr.isStopAttack = true;
        gameMgr.isReverseSAttack = false;
        gameMgr.isSecreteAttack = false;
    }

    private void BossReverseSwipe()
    {
        Debug.Log("ReverseSwipe");
        gameMgr.isReverseSAttack = true;
        gameMgr.isStopAttack = false;
        gameMgr.isSecreteAttack = false;
    }

    private void BossSecretePuzzle()
    {
        Debug.Log("SecretePuzzle");
        gameMgr.isSecreteAttack = true;
        gameMgr.isStopAttack = false;
        gameMgr.isReverseSAttack = false;
    }

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        //player.AddComponent<Enemy>();
        //player.GetComponent<Enemy>().textData = floatTextData;
    }


    public void Start()
    {
        gameMgr = gameManager.GetComponent<GameMgr>();
        battleAudioSource = GetComponent<AudioSource>();
        battleAudioSource.loop = true;
        battleAudioSource.clip = battleBGMClips[(int)gameMgr.currentStage - 3001];
        battleAudioSource.Play();

        if (bossPrefab != null)
        {
            Destroy(bossPrefab);
        }
        bossPrefab = Instantiate(bossPrefabs[(int)gameMgr.currentStage - 3001], battleMap.transform);
        bossPrefab.AddComponent<EffectSystem>();
        bossPrefab.AddComponent<Enemy>();
        bossPrefab.GetComponent<Enemy>().textData = floatTextData;
        bossPrefab.transform.localPosition = new Vector3(1.3f, 0f, 0f);
        bossPrefab.AddComponent<AudioSource>();

        playerPos = new Vector3(-1.3f, 0f, 0f);
        bossPos = bossPrefab.transform.localPosition;
        bossAudioSource = bossPrefab.GetComponent<AudioSource>();

        var bossRot = Quaternion.Euler(0,-130,0);
        bossPrefab.transform.rotation = bossRot;

        bossAnimator = bossPrefab.GetComponent<Animator>();

        stringTable = DataTableMgr.Get<StringTable>(DataTableIds.String);
        playerTable = DataTableMgr.Get<PlayerDataTable>(DataTableIds.PlayerTable);
        bossTable = DataTableMgr.Get<BossDataTable>(DataTableIds.BossTable);
        stageTable = DataTableMgr.Get<StageDataTable>(DataTableIds.StageTable);

        Player.Instance.hp = playerTable.Get(DataTableIds.playerID).Player_Hp + Player.Instance.gainedHp;
        playerOriginHp = Player.Instance.hp;
        Player.Instance.atk = playerTable.Get(DataTableIds.playerID).Player_Atk + Player.Instance.gainedAtk;
        Player.Instance.critical = playerTable.Get(DataTableIds.playerID).Player_Critical + Player.Instance.gainedCritical;
        Player.Instance.imageId = playerTable.Get(DataTableIds.playerID).Player_Image;

        DataTableIds.stageID = ((int)gameMgr.currentStage).ToString();
        string bossID = stageTable.Get(DataTableIds.stageID).Boss_ID;
        boss.name = stringTable.Get(bossTable.Get(bossID).Boss_Name);
        boss.hp = bossTable.Get(bossID).Boss_Hp;
        bossOriginHp = boss.hp;
        boss.atk = bossTable.Get(bossID).Boss_Atk;

        bossAttackInterval = bossTable.Get(bossID).Boss_ATKtime;

        boss.bossPattern[0] = bossTable.Get(bossID).Boss_patternA;
        boss.bossPattern[1] = bossTable.Get(bossID).Boss_patternB;
        boss.bossPattern[2] = bossTable.Get(bossID).Boss_patternC;
        boss.bossPattern[3] = bossTable.Get(bossID).Boss_patternD;
        boss.bossPattern[4] = bossTable.Get(bossID).Boss_patternE;
        currentBossAttackPattern = 0;

        battleBack.texture = stageTable.Get(DataTableIds.stageID).GetBack;
        var ground = stageTable.Get(DataTableIds.stageID).GetGround;
        Material groundMaterial = new Material(Shader.Find("Standard"));
        groundMaterial.mainTexture = ground;
        battleGround.GetComponent<MeshRenderer>().material = groundMaterial;


        playerHpBar.fillAmount = Player.Instance.hp / playerOriginHp;
        bossHpBar.fillAmount = boss.hp / bossOriginHp;

        timer = bossAttackInterval;

        gameMgr.isPlayerDie = false;
        gameMgr.isBattleStageClear = false;

        quitMenu.SetActive(false);
        bossSpecialAttack = false;

        //playerAnimator.Play(AnimatorIds.playerIdleAni, -1, 0f);
        playerAnimator.ResetTrigger(AnimatorIds.playerAtkAni);
        playerAnimator.ResetTrigger(AnimatorIds.playerDamagedAni);

        Debug.Log("플레이어 hp : " + Player.Instance.hp);
        Debug.Log("보스 ID : " + bossID);
        InitBossSkill();
    }

    private void Update()
    {
        //if (gameMgr.isPlayerFirst && !gameMgr.isBossFirst)
        //{
        //    StartCoroutine(PlayerFirst());
        //    gameMgr.isPlayerFirst = false;
        //}

        //if (gameMgr.isBossFirst && !gameMgr.isPlayerFirst)
        //{
        //    StartCoroutine(BossFirst());
        //    gameMgr.isBossFirst = false;
        //    bossSpecialAttack = true;
        //}

        if (is16Attack)
        {
            PlayerTurn();
           
           is16Attack = false;
        }

        if(gameMgr.isGameStart)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0f && !gameMgr.isBattleStageClear && gameMgr.isGameStart)
        {
            BossTurn();
            timer = bossAttackInterval;
        }

        if(gameMgr.isGridFull)
        {
            Player.Instance.hp = 0;
            playerHpBar.fillAmount = 0f;
            CheckHealth();
        }
    }

    //private IEnumerator PlayerFirst()
    //{
    //    Debug.Log("PlayerFirst Start");
    //    yield return StartCoroutine(PlayerTurn());
    //    if(!gameMgr.isBattleStageClear)
    //    {
    //        yield return StartCoroutine(BossTurn());
    //    }
    //    if(!gameMgr.isPlayerDie && !gameMgr.isBattleStageClear)
    //    {
    //        GoNextRound();
    //    }
    //}
    //private IEnumerator BossFirst()
    //{
    //    Debug.Log("BossFirst Start");
    //    boss.atk *= 32;
    //    yield return StartCoroutine(BossTurn());

    //    if (!gameMgr.isBattleStageClear && !gameMgr.isPlayerDie)
    //    {
    //        yield return StartCoroutine(PlayerTurn());
    //        GoNextRound();
    //    }
    //}

    private void PlayerTurn()
    {
        if(!gameMgr.isPlayerDie)
        {
            StartCoroutine(gameMgr.PlayParticleSystem(gameMgr.playerParticle));
            //yield return new WaitForSeconds(1f);

            playerAnimator.SetTrigger(AnimatorIds.playerAtkAni);
            bossAnimator.SetTrigger(AnimatorIds.bossDamagedAni);

            int criticalValue = 1;
            Color damageColor = Color.white;
            int randomValue = Random.Range(0, 100);
            if(randomValue <= Player.Instance.critical)
            {
                criticalValue = 2;
                damageColor = Color.red;
            }

            boss.hp -= Player.Instance.atk * criticalValue;
            Vector3 pos = new Vector3(0f, 2f, -0.5f);
            ShowDamage(Player.Instance.atk * criticalValue, pos, damageColor);

            bossHpBar.fillAmount = boss.hp / bossOriginHp;
            CheckHealth();
            Debug.Log("Boss HP : " + boss.hp);
        }
    }

    private void BossTurn()
    {
        if (!gameMgr.isPlayerDie)
        {
            //StartCoroutine(gameMgr.PlayBossParticleSystem(gameMgr.bossParticle, gameMgr.bossParticlePos));
            //yield return new WaitForSeconds(1f);

            //if (bossSpecialAttack)
            //{
                

            //}
            //else
            //{
            //    bossAnimator.SetTrigger(AnimatorIds.bossAtkAni);
            //}
            //bossSpecialAttack = false;
            
            //yield return new WaitForSeconds(0.1f);



            if (currentBossAttackPattern >= 5)
            {
                currentBossAttackPattern = 0;
            }
            BossSkill skill = (BossSkill)boss.bossPattern[currentBossAttackPattern];
            bossSkillActions[skill].Invoke();
            ++currentBossAttackPattern;

        }
    }
    

    private void CheckHealth()
    {
        if (Player.Instance.hp <= 0)
        {
            playerAnimator.SetTrigger(AnimatorIds.playerDieAni);
            gameMgr.isPlayerDie = true;

            bossAnimator.SetTrigger(AnimatorIds.bossVictoryAni);

            //StopAllCoroutines();
        }
        else if (boss.hp <= 0)
        {
            bossAnimator.SetTrigger(AnimatorIds.bossDiedAni);
            playerAnimator.SetTrigger(AnimatorIds.playerIdleAni);
            gameMgr.isBattleStageClear = true;
            gainedGold = stageTable.Get(DataTableIds.stageID).Stage_Reward;
            Player.Instance.gold += gainedGold;

           // StopAllCoroutines();
        }
        else
        {
            return;
        }
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
    public void ShowDamage(int damage, Vector3 position, Color damageCol)
    {
        //var floatingText = Instantiate(playerfloatingDamage, battleMap.transform);

        //playerfloatingDamage.transform.localPosition = position;
        //var text = playerfloatingDamage.GetComponentInChildren<TextMeshProUGUI>();
        //text.text = damage.ToString();
        //position.z += 10f;
        //playerfloatingDamage.transform.DOMove(position, 1f);
        //StartCoroutine(RemoveDamageText(floatingText));
        var floatingText = Instantiate(playerfloatingDamage, battleMap.GetComponentInChildren<Canvas>().transform);

        floatingText.transform.localPosition = position;
        var text = floatingText.GetComponentInChildren<TextMeshProUGUI>();
        text.color = damageCol;
        text.text = damage.ToString();

        // Animate the damage text
        Vector3 endPosition = position + new Vector3(0, 1f, 0); // Adjust the Y value to move upwards
        floatingText.transform.DOLocalMove(endPosition, 1.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Destroy(floatingText);
        });

        // Fade out the text
        text.DOFade(0, 1.5f);
    }

    private IEnumerator RemoveDamageText(GameObject textObject)
    {
        yield return new WaitForSeconds(1f);
        Destroy(textObject);
    }

    public void OnClickQuit()
    {
        quitMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnClickResume()
    {
        quitMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        gameMgr.RestartGame();
    }

    public void OnClickStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageSelect");
    }

    private void InitBossSkill()
    {
        bossSkillActions = new Dictionary<BossSkill, System.Action>
        {
            { BossSkill.Normal, BossNormalAttack },
            { BossSkill.StopSwipe, BossStopSwipe },
            { BossSkill.ReverseSwipe, BossReverseSwipe },
            { BossSkill.SecretePuzzle, BossSecretePuzzle }
        };
    }
}
