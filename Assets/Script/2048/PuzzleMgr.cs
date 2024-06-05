using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMgr : MonoBehaviour
{
    [SerializeField] private TileBoard board;
    [SerializeField] private CanvasGroup gameOver;

    public Camera puzzleCamera;

    private void Awake()
    {
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
        gameOver.alpha = 0f;
        gameOver.interactable = false;
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    //public void GameOver()
    //{
    //    board.enabled = false;
    //    gameOver.interactable = true;

    //    StartCoroutine(Fade(gameOver, 1f, 1f));
    //}

    //private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    //{
    //    yield return new WaitForSeconds(delay);

    //    float elapsed = 0f;
    //    float duration = 0.5f;
    //    float from = canvasGroup.alpha;

    //    while (elapsed < duration)
    //    {
    //        canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    canvasGroup.alpha = to;
    //}
}
