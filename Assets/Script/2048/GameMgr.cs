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

    public Image popUpPanel;
    TextMeshProUGUI popUpMessage;

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

        bossAnimator = GameObject.FindWithTag("Boss").GetComponent<Animator>();
        bossParticlePos = new List<Vector2>();

        puzzleMgr = puzzleManager.GetComponent<PuzzleManager>();
        battleMgr = battleManager.GetComponent<BattleMgr>();

        //victoryGoldText = popUpVictory.GetComponentInChildren<TextMeshProUGUI>();
        //victoryNextButton = popUpVictory.GetComponentInChildren<Button>();
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

        //popUpPanel.gameObject.SetActive(false);
        //popUpMessage = popUpPanel.GetComponentInChildren<TextMeshProUGUI>();

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
            isTimeOver = false;
            playerParticle.transform.position = playerParticlePos;
            isPlayerFirst = true;
        }

        if (isGridFull)
        {
            Debug.Log("GridFull");
            isGridFull = false;
            isBossFirst = true;
        }
    }

    public void BattleOver()
    {
        if (isPlayerDie)
        {
            //popUpPanel.gameObject.SetActive(true);
            //popUpMessage.text = $"Stage Failed!\n gained Gold : 0";
            popUpDefeat.gameObject.SetActive(true);
            Debug.Log("Player Die");
        }

        if (isBattleStageClear)
        {
            //popUpPanel.gameObject.SetActive(true);
            //popUpMessage.text = $"Stage Clear!\n gained Gold : {battleMgr.gainedGold}";
            popUpVictory.gameObject.SetActive(true);
            victoryGoldText.text = $"{battleMgr.gainedGold}";
            Debug.Log("Stage Clear");

            //다음 스테이지로 진행
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
        //popUpPanel.gameObject.SetActive(false);

        if (isPlayerDie || isBattleStageClear)
        {
            puzzleMgr.NewGame();
            battleMgr.Start();
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
