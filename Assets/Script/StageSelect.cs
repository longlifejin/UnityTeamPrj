using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    public static StageSelect Instance { get; private set; }

    public Button[] stageButtons;
    public Stage currStage;
    private int num = 0;

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

        for (int i = 0; i < stageButtons.Length; ++i)
        {
            int index = i;
            stageButtons[i].onClick.AddListener(() =>
            {
                currStage = (Stage)(index + 3001);
                SceneManager.LoadScene("Puzzle&Battle");
                Debug.Log("stage selected");
            });
        }
    }

    private void Start()
    {
        
    }


    public void OnClickBack()
    {
        //TO-DO : string define에 선언해서 사용하기
        SceneManager.LoadScene("EntranceStorePopUp");
    }


}
