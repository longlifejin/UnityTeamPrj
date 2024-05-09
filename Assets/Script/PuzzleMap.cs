using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMap : MonoBehaviour
{
    private int gridSize = 4;
    private float cellSize = 85;
    private float gridHalfSize;
    private float offset = 1.1f;

    public GameObject CardPrefab;
    public GameObject linePrefab;
    public GameObject gridObject;

    public float lineThickness = 2f;

    private void Start()
    {
        CreateCard();
        //DrawGrid();
        gridHalfSize = offset * ((float)gridSize - 1) / 2f;
        //gridObject = GameObject.FindWithTag("Grid");
    }

    private void CreateCard()
    {
        for (int i = 0; i < gridSize * gridSize; ++i)
        {
            Instantiate(CardPrefab, gameObject.transform);
        }
    }
    void DrawGrid()
    {
        RectTransform gridRect = gridObject.GetComponent<RectTransform>();
        float width = gridRect.rect.width;
        float height = gridRect.rect.height;
        float cellWidth = width / gridSize;
        float cellHeight = height / gridSize;

        // Set pivot to top-left corner (0, 1)
        gridRect.pivot = new Vector2(0, 1);
        gridRect.anchorMin = new Vector2(0, 1);
        gridRect.anchorMax = new Vector2(0, 1);

        // Vertical lines
        for (int i = 0; i <= gridSize; i++)
        {
            GameObject line = Instantiate(linePrefab, gridRect);
            RectTransform lineRt = line.GetComponent<RectTransform>();
            lineRt.sizeDelta = new Vector2(lineThickness, height);
            lineRt.anchoredPosition = new Vector2(i * cellWidth, 0);
            lineRt.pivot = new Vector2(0.5f, 1);
            lineRt.anchorMin = new Vector2(0, 1);
            lineRt.anchorMax = new Vector2(0, 1);
        }

        // Horizontal lines
        for (int i = 0; i <= gridSize; i++)
        {
            GameObject line = Instantiate(linePrefab, gridRect);
            RectTransform lineRt = line.GetComponent<RectTransform>();
            lineRt.sizeDelta = new Vector2(width, lineThickness);
            lineRt.anchoredPosition = new Vector2(0, -i * cellHeight);
            lineRt.pivot = new Vector2(0, 0.5f);
            lineRt.anchorMin = new Vector2(0, 1);
            lineRt.anchorMax = new Vector2(0, 1);
        }
    }
}
