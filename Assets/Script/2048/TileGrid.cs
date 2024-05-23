using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows { get; private set; }
    public TileCell[] cells { get; private set; }

    public int Size => cells.Length;
    public int Height => rows.Length;
    public int Width => Size / Height;

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].coordinates = new Vector2Int(i % Width, i / Width);
        }
    }

    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }

    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            return rows[y].cells[x];
        }
        else
        {
            return null;
        }
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }

    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);
        int startingIndex = index;

        while (cells[index].Occupied)
        {
            index++;

            if (index >= cells.Length)
            {
                index = 0;
            }

            // all cells are occupied
            if (index == startingIndex)
            {
                return null;
            }
        }

        return cells[index];
    }

    public bool CheckGridFull()
    {
        for (int i = 0; i < cells.Length; ++i)
        {
            if (cells[i].Occupied == false)
                return false;
        }
        return true;
    }

    public int CountFilledGrid()
    {
        int filledGridCount = 1;

        for (int i = 0; i < cells.Length; ++i)
        {
            if (cells[i].Occupied == true)
            {
                ++filledGridCount;
            }
        }
        return filledGridCount;
    }

    public int GetMaxGridValue()
    {
        int maxValue = 0;

        for (int i = 0; i < cells.Length; ++i)
        {
            if (cells[i].Occupied)
            {
                if (cells[i].tile.state.number > maxValue)
                    maxValue = cells[i].tile.state.number;
            }
        }
        return maxValue;
    }

    public Vector2 GetMaxGridPos()
    {
        Vector2 maxGridPos = Vector2.zero;

        int maxValue = 0;

        for (int i = 0; i < cells.Length; ++i)
        {
            if (cells[i].Occupied)
            {
                if (cells[i].tile.state.number > maxValue)
                {
                    maxValue = cells[i].tile.state.number;
                    maxGridPos = cells[i].gameObject.GetComponent<RectTransform>().position;
                }
            }
        }
        return maxGridPos;
    }

    //public List<Vector2> GetFilledGridPos()
    //{
    //    List<Vector2> filledGridPoses = new List<Vector2>();
    //    for(int i = 0; i < cells.Length; ++i)
    //    {
    //        if (cells[i].Occupied)
    //        {
    //            filledGridPoses.Add(cells[i].gameObject.GetComponent<RectTransform>().position);
    //        }
    //    }
    //    return filledGridPoses;
    //}

    public List<Vector2> GetfilledGridPos()
    {
        List<Vector2> filledGridPosList = new List<Vector2>();

        for(int i = 0; i < cells.Length; ++i)
        {
            if (cells[i].Occupied)
            {
                filledGridPosList.Add(cells[i].transform.position);
            }
        }
        return filledGridPosList;
    }
}
