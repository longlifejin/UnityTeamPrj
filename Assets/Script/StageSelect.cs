using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    //public static StageSelect Instance { get; private set; }

    public Button[] stageButtons;
    public Button backButton;

    public Stage currStage;
    private int num = 0;

    private void Awake()
    {
        //if (Instance != null)
        //{
        //    DestroyImmediate(gameObject);
        //}
        //else
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
    }
    private void OnEnable()
    {
        SetButtons();
        //StartCoroutine(SetButtonsAfterSceneLoad());
    }


    public IEnumerator SetButtonsAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.1f);
        SetButtons();
    }

    public void SetButtons()
    {
        backButton = GameObject.FindWithTag("back").GetComponentInChildren<Button>();
        backButton.onClick.AddListener(() => OnClickBack());

        for (int i = 0; i < stageButtons.Length; ++i)
        {
            int index = i;
            int stageNum = i + 1;
            stageButtons[i] = GameObject.FindWithTag("stage" + stageNum.ToString()).GetComponentInChildren<Button>();
            stageButtons[i].onClick.AddListener(() =>
            {
                currStage = (Stage)(index + 3001);
                Player.Instance.currentStage = ((int)currStage).ToString();
                SceneManager.LoadScene("Puzzle&Battle");
                Debug.Log("stage selected");
            });
        }
    }



    public void OnClickBack()
    {
        //TO-DO : string define에 선언해서 사용하기
        SceneManager.LoadScene("EntranceStorePopUp");
    }


}
