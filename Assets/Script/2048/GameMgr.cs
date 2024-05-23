using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    public GameObject popUpVictory;
    public Text victoryGoldText;
    public Button victoryNextButton;

    public GameObject popUpDefeat;
    public Button defeatNextButton;
    public Button defeatRestartButton;

    public Animator playerAnimator;
    public Animator bossAnimator;

    public ParticleSystem playerParticle;
    public ParticleSystem bossParticle;

    public Vector2 playerParticlePos;
    public List<Vector2> bossParticlePos;

    public RectTransform board;

    public Stage currentStage;

    public AudioSource audioSource;
    public AudioClip clearSound;
    public AudioClip defeatSound;
    public AudioClip puzzleStartSound;
    public AudioClip puzzleEndSound;


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

        audioSource = GetComponent<AudioSource>();
        //bossAnimator = GameObject.FindWithTag("Boss").GetComponent<Animator>();
        bossParticlePos = new List<Vector2>();

        puzzleMgr = puzzleManager.GetComponent<PuzzleManager>();
        battleMgr = battleManager.GetComponent<BattleMgr>();

        victoryNextButton.onClick.AddListener(() => 
        { 
            OnClickOK(); 
            popUpVictory.SetActive(false);
        });

        popUpVictory.gameObject.SetActive(false);

        defeatNextButton.onClick.AddListener(()=> { SceneManager.LoadScene("Stagebackground"); });
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
            audioSource.PlayOneShot(puzzleEndSound);
            isGridFull = false;
            isBossFirst = true;
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

            currentStage += 1;
            Player.Instance.stageClear[(int)currentStage-3001] = true;

        }
    }

    public void StartNextRound()
    {
        puzzleMgr.NewGame();
    }

    public void OnClickOK()
    {
        if (isPlayerDie || isBattleStageClear)
        {
            puzzleMgr.NewGame();
            battleMgr.Start();
            audioSource.Stop();
        }

        isPlayerDie = false;
        isBattleStageClear = false;
    }

    public IEnumerator PlayParticleSystem(ParticleSystem particle)
    {
        particle.Play();
        yield return new WaitForSeconds(2f);
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
        yield return new WaitForSeconds(2f);

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
    }
}
