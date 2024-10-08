using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    //public static PuzzleManager Instance { get; private set; }

    [SerializeField] private TileBoard board;
    [SerializeField] private CanvasGroup gameOver;

    public Camera puzzleCamera;


    //[SerializeField] private TextMeshProUGUI scoreText;
    //[SerializeField] private TextMeshProUGUI hiscoreText;

    //private int score;
    //public int Score => score;

    private void Awake()
    {
        //if (Instance != null) {
        //    DestroyImmediate(gameObject);
        //} else {
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}

    }

    private void Start()
    {
        const float center = 4 / 2f - 0.5f;
        puzzleCamera.transform.position = new Vector3(center, center + 1f, -15.0f);

        NewGame();
    }

    public void NewGame()
    {
        board.ResetPuzzle();
        // reset score
        //SetScore(0);
        //hiscoreText.text = LoadHiscore().ToString();

        // hide game over screen
        gameOver.alpha = 0f;
        gameOver.interactable = false;

        // update board state
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;

        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }


    //public void IncreaseScore(int points)
    //{
    //    SetScore(score + points);
    //}

    //private void SetScore(int score)
    //{
    //    this.score = score;
    //    scoreText.text = score.ToString();

    //    SaveHiscore();
    //}

    //private void SaveHiscore()
    //{
    //    int hiscore = LoadHiscore();

    //    if (score > hiscore) {
    //        PlayerPrefs.SetInt("hiscore", score);
    //    }
    //}

    //private int LoadHiscore()
    //{
    //    return PlayerPrefs.GetInt("hiscore", 0);
    //}

}
