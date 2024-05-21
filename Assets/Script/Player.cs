using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public int hp = 0;
    public int atk = 0;
    public string imageId = string.Empty;
    public int gold = 0;
    public List<bool> stageClear = new List<bool>();
    public string currentStage;

    public int Gold 
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }

    public int Atk
    {
        get
        {
            return atk;
        }

        set
        {
            atk = value;
        }
    }

    public string CurrentStage
    {
        get
        {
            return currentStage;
        }
        set
        {
            currentStage = value;
        }
    }

    public List<bool> StageClear
    {
        get
        {
            return stageClear;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        stageClear.Add(true);
        for (int i = 1; i < 8; ++i)
        {
            stageClear.Add(false);
        }
        
    }
}
