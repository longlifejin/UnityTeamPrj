using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private TileState[] tileStates;
    [SerializeField] private HitZone[] hitZones;

    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting;
    private bool gameStart;
    public bool isGridFull;

    private List<Vector2Int> specialPos;

    private Vector2 touchStartPosition = Vector2.zero;
    private const float MinSwipeDistance = 10.0f;

    public Button puzzleStartButton;

    public GameObject gameManager;
    private GameMgr gameMgr;

    public Animator playerAnimator;
    public Animator bossAnimator;

    private List<GameObject> activeParticleImages;

    public GameObject framePrefab;

    public GameObject secretePanel;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        gameMgr = gameManager.GetComponent<GameMgr>();

        tiles = new List<Tile>(16);
        gameStart = false;
        isGridFull = false;
    }

    private void Start()
    {
        activeParticleImages = new List<GameObject>();
        secretePanel.SetActive(false); 

        specialPos = new List<Vector2Int>();

        for (int i = 0; i < hitZones[0].specificPos.Count; ++i)
        {
            specialPos.Add(hitZones[0].specificPos[i]);
        }
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
        secretePanel.SetActive(false);
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.transform.SetSiblingIndex(4);
        tile.SetState(tileStates[0]);
        tile.Spawn(grid.GetRandomEmptyCell());
        RectTransform rectTransform = tile.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, 0.5f);
        tiles.Add(tile);
    }

    private void Update()
    {
        if (!waiting && gameStart && !gameMgr.isPuzzleOver)
        {
            puzzleStartButton.gameObject.SetActive(false);

            if (gameMgr.isReverseSAttack)
            {
                ReverseTouchEvents();
                secretePanel.SetActive(false);
            }
            else if(gameMgr.isStopAttack)
            {
                secretePanel.SetActive(false);

            }
            else if(gameMgr.isSecreteAttack)
            {
                TouchEvents();
                secretePanel.SetActive(true);
            }
            else
            {
                TouchEvents();
                secretePanel.SetActive(false);
            }
        }
        if(isGridFull)
        {
            gameMgr.isGridFull = true;
            isGridFull = false;
        }
    }

    public void ResetPuzzle()
    {
        if (activeParticleImages != null)
        {
            foreach (var image in activeParticleImages)
            {
                Destroy(image);
            }
            activeParticleImages.Clear();
        }
        
        gameMgr.isTimeOver = false;
        gameMgr.isPuzzleOver = false;
        gameMgr.isGridFull = false;
        gameMgr.filledGridCount = 0;
        secretePanel.SetActive(false);

        playerAnimator.SetTrigger(AnimatorIds.playerIdleAni);
        gameStart = false;
        puzzleStartButton.gameObject.SetActive(true);

        UpdateSpecificPos();
        foreach (var pos in specialPos)
        {
            GameObject instance = Instantiate(framePrefab, grid.transform);
            RectTransform rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0, 0.5f);

            instance.transform.position = CoordinatesToPos(pos);

            activeParticleImages.Add(instance);
        }
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

    private void ReverseTouchEvents()
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
            Move(Vector2Int.down, 0, 1, grid.Height - 2, -1);
        }
        else if (swipeDelta.y < 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f)
        {
            Move(Vector2Int.up, 0, 1, 1, 1);
        }
        else if (swipeDelta.x > 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f)
        {
            Move(Vector2Int.left, 1, 1, 0, 1);
        }
        else if (swipeDelta.x < 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f)
        {
            Move(Vector2Int.right, grid.Width - 2, -1, 0, 1);
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

        if (IsTile16AtPosition())
        {
            gameMgr.is16Value = true;
        }

        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }

        gameMgr.filledGridCount = grid.CountFilledGrid();
        if (gameMgr.filledGridCount == 16)
        {
            isGridFull = true;
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
        if (a.state.number >= 16 || b.state.number >= 16)
        {
            return false;
        }
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        if (a.state.number >= 16 || b.state.number >= 16)
        {
            return;
        }

        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        TileState newState = tileStates[index];

        b.SetState(newState);
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

        foreach (var tile in tiles)
        {
            tile.locked = false;
        }

        if (tiles.Count != grid.Size)
        {
            CreateTile();
        }
    }

    public void OnClickPuzzleStartButton()
    {
        touchStartPosition = Input.GetTouch(0).position;
        gameStart = true;
        gameMgr.isGameStart = true;
        playerAnimator.SetTrigger(AnimatorIds.playerChargingAni);
        gameMgr.audioSource.PlayOneShot(gameMgr.puzzleStartSound);
        puzzleStartButton.gameObject.SetActive(false);
    }

    public bool IsTile16AtPosition()
    {
        foreach (var tile in tiles)
        {
            if (specialPos.Contains(tile.cell.coordinates) && tile.state.number == 16)
            {                
                gameMgr.playerParticlePos = tile.cell.transform.position;
                tiles.Remove(tile);
                Destroy(tile.gameObject);
                return true;
            }
        }
        return false;
    }

    public Vector3 CoordinatesToPos(Vector2Int coordinate)
    {
        TileCell cell = grid.GetCell(coordinate);
        if (cell != null)
        {
            return cell.transform.position;
        }
        return Vector3.zero;
    }

    private void UpdateSpecificPos()
    {
        var hitzone = hitZones[(int)gameMgr.currentStage - 3001];
        specialPos.Clear();
        //specialPos = new List<Vector2Int>();
        foreach(var pos in hitzone.specificPos)
        {
            specialPos.Add(pos);
        }
    }
}
