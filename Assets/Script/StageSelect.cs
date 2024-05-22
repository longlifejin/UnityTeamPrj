using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class StageSelect : MonoBehaviour
{
    //public static StageSelect Instance { get; private set; }

    public Button[] stageButtons;
    public Button backButton;
    private StageInfo stageInfo;
    private StageData stage;
    private bool isCancel;
    private bool isStart;

    private StageDataTable stageTable;
    public StageInfo stagePrefab;
    public GameObject canvas;

    public Stage currStage;
    private List<bool> stageState = new List<bool>();


    private void Awake()
    {
        stageState.Add(true);
        for (int i = 1; i < Player.Instance.stageClear.Count; ++i)
        {
            stageState.Add(false);
        }

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
        stageInfo = Instantiate(stagePrefab, canvas.transform);
        stageInfo.gameObject.SetActive(false);
        stageTable = DataTableMgr.Get<StageDataTable>(DataTableIds.StageTable);
        SetButtons();


        isCancel = false;
        isStart = false;
        //StartCoroutine(SetButtonsAfterSceneLoad());
    }

    private void Update()
    {
        CheckToggleState();
    }


    public IEnumerator SetButtonsAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.1f);
        SetButtons();
    }

    public void SetButtons()
    {
        GameObject buttons = GameObject.FindWithTag("back");
        backButton = buttons.GetComponentInChildren<Button>();
        backButton.onClick.AddListener(() => OnClickBack());

        

        for (int i = 0; i < stageButtons.Length; ++i)
        {
            int index = i;
            int stageNum = i + 1;
            GameObject stageButton = GameObject.FindWithTag("stage" + stageNum.ToString());
            stageButtons[i] = stageButton.GetComponentInChildren<Button>();
            currStage = (Stage)(index + 3001);
            stage = stageTable.Get(((int)currStage).ToString());

            int stagePopupIndex = index;
            stageButtons[i].onClick.AddListener(() =>
            {
                PopStageInfo(stagePopupIndex);                
            });

            var toggle = stageButton.GetComponentInChildren<Toggle>();
            if (Player.Instance.stageClear[i])
            {
                toggle.isOn = true;
                var image = FindChildWithTag(stageButton, "lock").GetComponentInChildren<Image>();
                //Color color = image.color;
                //color.a = 0;
                //image.color = color;
                SetImageAlpha(image, 0);
                stageButtons[i].interactable = true;
                toggle.interactable = false;
            }
            else
            {
                toggle.isOn = false;
                var image = FindChildWithTag(stageButton, "lock").GetComponentInChildren<Image>();
                //Color color = image.color;
                //color.a = 255;
                //image.color = color;
                SetImageAlpha(image, 255);
                stageButtons[i].interactable = false;
                toggle.interactable = false;
            }
        }
    }

    private void PopStageInfo(int index)
    {
        var selectedStage = (Stage)(index + 3001);
        StageData stage = stageTable.Get(((int)selectedStage).ToString());

        stageInfo.gameObject.SetActive(true);
        stageInfo.stageName.text = stage.Stage_Name;
        stageInfo.stageDescription.text = stage.Stage_Info;
        stageInfo.stageImage.texture = stage.GetImage;

        stageInfo.cancel.onClick.RemoveAllListeners();
        stageInfo.cancel.onClick.AddListener(() => stageInfo.gameObject.SetActive(false));

        stageInfo.start.onClick.RemoveAllListeners();
        stageInfo.start.onClick.AddListener(() =>
        {
            currStage = selectedStage;
            Player.Instance.currentStage = ((int)currStage).ToString();
            SceneManager.LoadScene("Puzzle&Battle");
            Debug.Log("Stage " + (index + 1) + " selected");
        });
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    public void OnClickBack()
    {
        //TO-DO : string define에 선언해서 사용하기
        SceneManager.LoadScene("EntranceStorePopUp");
    }

    private GameObject FindChildWithTag(GameObject parent, string tag)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        return null;
    }

   private void CheckToggleState()
    {
        for(int i = 0; i < Player.Instance.stageClear.Count; ++i)
        {
            if (stageState[i] != Player.Instance.stageClear[i])
            {
                stageState[i] = Player.Instance.stageClear[i];

                int stageNum = i + 1;
                GameObject stageButton = GameObject.FindWithTag("stage" + stageNum.ToString());
                var image = FindChildWithTag(stageButton, "lock").GetComponentInChildren<Image>();
                Color color = image.color;
                color.a = 0;
                image.color = color;
                stageButtons[i].interactable = true;

                var toggle = stageButton.GetComponentInChildren<Toggle>();
                toggle.interactable = false;
            }
        }
    }

}
