using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public bool isTimeOver = false;
    public bool isGridFull = false;


    private void Start()
    {
        
    }
    private void Update()
    {
        PuzzleOver();
    }

    private void PuzzleOver()
    {
        if(isTimeOver)
        {
            Debug.Log("Time Over");
        }

        if(isGridFull)
        {
            Debug.Log("Grid Full");
        }
    }


}
