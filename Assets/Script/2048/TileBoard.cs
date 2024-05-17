using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private TileState[] tileStates;

    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting;
    private bool gameStart;
    public bool isGridFull;

    private Vector2 touchStartPosition = Vector2.zero;
    private const float MinSwipeDistance = 10.0f;

    private float limitTime = 30f;
    private float timer;

    public Image timeBar;

    public Button puzzleStartButton; 
    
    public GameObject gameManager;
    private GameMgr gameMgr;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        gameMgr = gameManager.GetComponent<GameMgr>();

        tiles = new List<Tile>(16);
        gameStart = false;
        isGridFull = false;

        timer = limitTime;
        Time.timeScale = 0f;
    }

    private void Start()
    {
        timer = limitTime;
        Time.timeScale = 0f;
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells) 
        {
            cell.tile = null;
        }

        foreach (var tile in tiles) 
        {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0]);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    private void Update()
    {
        if (!waiting && gameStart && !gameMgr.isPuzzleOver)
        {
            timeBar.fillAmount = timer / limitTime;
            timer -= Time.deltaTime;
            puzzleStartButton.enabled = false;
            TouchEvents();
            //if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) 
            //{
            //    Move(Vector2Int.up, 0, 1, 1, 1);
            //} 
            //else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) 
            //{
            //    Move(Vector2Int.left, 1, 1, 0, 1);
            //} 
            //else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) 
            //{
            //    Move(Vector2Int.down, 0, 1, grid.Height - 2, -1);
            //} 
            //else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) 
            //{
            //    Move(Vector2Int.right, grid.Width - 2, -1, 0, 1);
            //}
        }

        if (timer <= 0f)
        {
            gameMgr.isTimeOver = true;
            gameMgr.isPuzzleOver = true;
            timer = limitTime;
           
        }
        else if(isGridFull)
        {
            gameMgr.isGridFull = true;
            gameMgr.isPuzzleOver = true;
            gameMgr.filledGridCount = grid.cells.Length;
            timer = limitTime;
            isGridFull = false;
        }
    }

    public void ResetPuzzle()
    {
        gameMgr.isTimeOver = false;
        gameMgr.isPuzzleOver = false;
        gameMgr.isGridFull = false;
        gameMgr.filledGridCount = 0;

        timer = limitTime;
        timeBar.fillAmount = 1f;

        gameStart = false;

        puzzleStartButton.enabled = true;
    }

    private void TouchEvents()
    {
        if (Input.touchCount == 0)
        {
            return;
        }
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchStartPosition = Input.GetTouch(0).position;
        }
        if (Input.GetTouch(0).phase != TouchPhase.Ended) return;
        var swipeDelta = (Input.GetTouch(0).position - touchStartPosition);
        if (swipeDelta.magnitude < MinSwipeDistance)
        {
            return;
        }
        swipeDelta.Normalize();
        if (swipeDelta.y > 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f)
        {
            Move(Vector2Int.up, 0, 1, 1, 1);
        }
        else if (swipeDelta.y < 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f)
        {
            Move(Vector2Int.down, 0, 1, grid.Height - 2, -1);
        }
        else if (swipeDelta.x > 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f)
        {
            Move(Vector2Int.right, grid.Width - 2, -1, 0, 1);
        }
        else if (swipeDelta.x < 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f)
        {
            Move(Vector2Int.left, 1, 1, 0, 1);
        }
    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied)
                {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        gameMgr.filledGridCount = grid.CountFilledGrid();
        if(gameMgr.filledGridCount == 16)
        {
            isGridFull = true;
        }
        gameMgr.maxValue = grid.GetMaxGridValue();
        Debug.Log("MaxGridPos : " + grid.GetMaxGridPos());

        if (changed) 
        {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        TileState newState = tileStates[index];

        b.SetState(newState);
        //PuzzleManager.Instance.IncreaseScore(newState.number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i]) 
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        foreach (var tile in tiles) {
            tile.locked = false;
        }

        if (tiles.Count != grid.Size) {
            CreateTile();
        }

        //if (CheckForGameOver()) {
        //    PuzzleManager.Instance.GameOver();
        //}
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.Size) {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }
        }

        return true;
    }

    public void OnClickPuzzleStartButton()
    {
        touchStartPosition = Input.GetTouch(0).position;
        Time.timeScale = 1f;
        gameStart = true;
    }

   
}
