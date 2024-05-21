using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public int hp = 0;
    public int atk = 0;
    public string imageId = string.Empty;
    public int gold = 0; 

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
    }
}
