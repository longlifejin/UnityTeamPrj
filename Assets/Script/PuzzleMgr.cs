using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMgr : MonoBehaviour
{
    public Sprite[] TileSprites; //타일에 사용될 스프라이트 배열
    public Camera PuzzleCamera; //퍼즐 카메라
    public GameObject gridBG; //타일이 배치되는 캔버스

    private Vector2 _touchStartPosition = Vector2.zero;
    private Grid _grid;

    private const int Size = 4;
    private const float MinSwipeDistance = 10.0f;

    private float limitTime = 30f;
    private float timer;

    public Slider timerBar;

    public GameObject gameManager;
    private GameMgr gameMgr;

    public int maxValue = 0;
    public int emtpyCount = 0;

    private void Start() //게임 시작 시 카메라 위치 설정, 그리드 초기화 
    {
        const float center = Size / 2f - 0.5f;
        PuzzleCamera.transform.position = new Vector3(center, center, -10.0f);
        
        _grid = new Grid(Size, TileSprites, gridBG);
        gameMgr = gameManager.GetComponent<GameMgr>();
        timer = limitTime;
    }

    private void InputEvents() //키보드 입력을 받는 내용
    {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown("r"))
        {
            _grid.Reset();
        }
        if (Input.GetKeyDown("left"))
        {
            _grid.MakeMove(Direction.Left);
        }
        else if (Input.GetKeyDown("right"))
        {
            _grid.MakeMove(Direction.Right);
        }
        if (Input.GetKeyDown("down"))
        {
            _grid.MakeMove(Direction.Down);
        }
        else if (Input.GetKeyDown("up"))
        {
            _grid.MakeMove(Direction.Up);
        }
    }

    private void TouchEvents() //터치 입력(스와이프 처리)
    {
        if (Input.touchCount == 0)
        {
            return;
        }
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            _touchStartPosition = Input.GetTouch(0).position;
        }
        if (Input.GetTouch(0).phase != TouchPhase.Ended) return;
        var swipeDelta = (Input.GetTouch(0).position - _touchStartPosition);
        if (swipeDelta.magnitude < MinSwipeDistance)
        {
            return;
        }
        swipeDelta.Normalize();
        if (swipeDelta.y > 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f)
        {
            _grid.MakeMove(Direction.Up);
        }
        else if (swipeDelta.y < 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f)
        {
            _grid.MakeMove(Direction.Down);
        }
        else if (swipeDelta.x > 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f)
        {
            _grid.MakeMove(Direction.Right);
        }
        else if (swipeDelta.x < 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f)
        {
            _grid.MakeMove(Direction.Left);
        }
    }

    private void Update()
    {
        gameMgr.filledGridCount = (int)Mathf.Pow(_grid._size, 2) - _grid.emptyCount;
        gameMgr.maxValue = _grid.maxValue;

        if (!gameMgr.isPuzzleOver)
        {
            timerBar.value = (timer / limitTime);
            timer -= Time.deltaTime;

            InputEvents();
            TouchEvents();
            _grid.Update();
        }

        if (timer <= 0f)
        {
            gameMgr.isTimeOver = true;
            gameMgr.isPuzzleOver = true;
            timer = limitTime;
        }
        else if(_grid.isGridFull)
        {
            gameMgr.isGridFull = true;
            gameMgr.isPuzzleOver = true;
            gameMgr.filledGridCount = (int)Mathf.Pow(_grid._size, 2) - _grid.emptyCount;
            _grid.isGridFull = false;
            timer = limitTime;
        }
    }

    [UsedImplicitly]
    public void ResetPuzzle()
    {
        _grid.Reset();
        timer = limitTime;
        gameMgr.isPuzzleOver = false;

    }
}
