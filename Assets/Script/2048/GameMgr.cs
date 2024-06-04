using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour
{
    public bool isPuzzleOver = false;

    public bool isGameStart = false;
    public bool isTimeOver = false;
    public bool isGridFull = false;
    public bool isPlayerFirst = false;
    public bool isBossFirst = false;
    public bool isPlayerDie = false;
    public bool isBattleStageClear = false;
    public bool is16Value = false;

    public bool isReverseSAttack = false;
    public bool isStopAttack = false;
    public bool isSecreteAttack = false;

    public int filledGridCount;
    public int maxValue;

    public GameObject puzzleManager;
    private PuzzleManager puzzleMgr;
    public GameObject battleManager;
    private BattleMgr battleMgr;

    public GameObject popUpVictory;
    public TextMeshProUGUI victoryGoldText;
    public Button victoryRestartButton;
    public Button victoryStageSelectButton;

    public GameObject popUpDefeat;
    public Button defeatNextButton;
    public Button defeatRestartButton;

    public Animator playerAnimator;
    public Animator bossAnimator;

    public ParticleSystem playerParticle;
    public ParticleSystem bossParticle;

    public Vector2 playerParticlePos;
    public List<Vector2> bossParticlePos;

    public Vector2 particularParticlePos;
    public ParticleSystem particularParticle;

    public RectTransform board;

    public Stage currentStage;

    public AudioSource audioSource;
    public AudioClip clearSound;
    public AudioClip defeatSound;
    public AudioClip puzzleStartSound;
    public AudioClip puzzleEndSound;

    public GameObject stage1_Tutorial;
    public float bossAttackTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        bossParticlePos = new List<Vector2>();

        puzzleMgr = puzzleManager.GetComponent<PuzzleManager>();
        battleMgr = battleManager.GetComponent<BattleMgr>();
    }

    private void Start()
    {
        if(Player.Instance.currentStage == null)
        {
            currentStage = Stage.first;
        }
        else
        {
            currentStage = (Stage)int.Parse(Player.Instance.currentStage);
        }

        if(currentStage == Stage.first)
        {
            stage1_Tutorial.SetActive(true);
            stage1_Tutorial.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                stage1_Tutorial.gameObject.SetActive(false);
            });
        }
        else
        {
            stage1_Tutorial.SetActive(false);
        }

        isGameStart = false;

        victoryRestartButton.onClick.AddListener(() => 
        { 
            OnClickOK(); 
            popUpVictory.SetActive(false);
        });

        victoryStageSelectButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("StageSelect");
        });

        popUpVictory.gameObject.SetActive(false);

        defeatNextButton.onClick.AddListener(()=> { SceneManager.LoadScene("StageSelect"); });
        defeatRestartButton.onClick.AddListener(() => 
        { 
            OnClickOK();
            popUpDefeat.gameObject.SetActive(false);
        });

        popUpDefeat.gameObject.SetActive(false);


        playerParticle.Stop();
    }

    private void Update()
    {
        PuzzleOver();
        DataTableIds.stageID = ((int)currentStage).ToString();

        if(is16Value)
        {
            playerParticle.transform.position = playerParticlePos;
            var pos = playerParticle.transform.localPosition;
            pos.x += 80f;
            playerParticle.transform.localPosition = pos;
            battleMgr.is16Attack = true;
            is16Value = false;
        }
    }

    private void PuzzleOver()
    {
        if (isTimeOver)
        {
            audioSource.PlayOneShot(puzzleEndSound);
            isTimeOver = false;
            playerParticle.transform.position = playerParticlePos;
            isPlayerFirst = true;
        }

        if (isGridFull)
        {
            Player.Instance.hp = 0;
            battleMgr.CheckHealth();
            isGridFull = false;
        }
    }

    public void BattleOver()
    {
        if (isPlayerDie)
        {
            popUpDefeat.gameObject.SetActive(true);
            audioSource.PlayOneShot(defeatSound);
        }

        if (isBattleStageClear)
        {
            popUpVictory.gameObject.SetActive(true);
            audioSource.PlayOneShot(clearSound);
            victoryGoldText.text = $"{battleMgr.gainedGold}";

            if(currentStage == Stage.eightth)
            {
                Player.Instance.stageClear[(int)currentStage - 3001] = true;
            }
            else
            {
                currentStage += 1;
                Player.Instance.stageClear[(int)currentStage-3001] = true;
            }

        }
        SaveLoadSystem.Save();
    }

    public void StartNextRound()
    {
        puzzleMgr.NewGame();
    }

    public void OnClickOK()
    {
        if (isPlayerDie || isBattleStageClear)
        {
            if(isBattleStageClear)
            {
                currentStage -= 1;
            }
            isSecreteAttack = false;
            puzzleMgr.NewGame();
            battleMgr.Start();
            audioSource.Stop();
        }
        isGameStart = false;
        isPlayerDie = false;
        isBattleStageClear = false;
    }

    public IEnumerator PlayParticleSystem(ParticleSystem particle)
    {
        particle.Play();
        yield return new WaitForSeconds(1f);
        particle.Stop();
    }

    public IEnumerator PlayBossParticleSystem(ParticleSystem particle, List<Vector2> poses)
    {
        List<ParticleSystem> activeParticles = new List<ParticleSystem>();

        foreach (var pos in poses)
        {
            ParticleSystem instance = Instantiate(particle, pos, Quaternion.identity, board);
            instance.Play();
            activeParticles.Add(instance);
        }
        yield return new WaitForSeconds(1f);

        foreach (var instance in activeParticles)
        {
            instance.Stop();
            Destroy(instance.gameObject);
        }
    }

    public void RestartGame()
    {
        puzzleMgr.NewGame();
        battleMgr.Start();
        isPlayerDie = false;
        isBattleStageClear = false;
        isGameStart = false;
    }
}
