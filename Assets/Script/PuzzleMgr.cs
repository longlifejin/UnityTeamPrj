using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMgr : MonoBehaviour
{
    public Sprite[] TileSprites; //Ÿ�Ͽ� ���� ��������Ʈ �迭
    public Camera PuzzleCamera; //���� ī�޶�
    public GameObject gridBG; //Ÿ���� ��ġ�Ǵ� ĵ����

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

    private void Start() //���� ���� �� ī�޶� ��ġ ����, �׸��� �ʱ�ȭ 
    {
        const float center = Size / 2f - 0.5f;
        PuzzleCamera.transform.position = new Vector3(center, center, -10.0f);
        _grid = new Grid(Size, TileSprites, gridBG);
        gameMgr = gameManager.GetComponent<GameMgr>();
        timer = limitTime;
    }

    private void InputEvents() //Ű���� �Է��� �޴� ����
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

    private void TouchEvents() //��ġ �Է�(�������� ó��)
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
        maxValue = _grid.maxValue;
        emtpyCount = _grid.emptyCount;
        if (!gameMgr.isGridFull && !gameMgr.isTimeOver)
        {
            timerBar.value = (timer / limitTime);
            timer -= Time.deltaTime;
            
        }
        InputEvents();
        TouchEvents();
        _grid.Update();

        if (timer <= 0f)
        {
            gameMgr.isTimeOver = true;
        }
        
        if(_grid.Full())
        {
            gameMgr.isGridFull = true;
        }    

    }

    [UsedImplicitly]
    public void ResetLevel()
    {
        _grid.Reset();
        timer = 0f;
    }
}
