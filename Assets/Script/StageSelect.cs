using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    //public static StageSelect Instance { get; private set; }

    public GameObject[] stages;
    public Button[] stageButtons;
    public Button backButton;
    public StageInfo stageInfo;
    private StageData stage;
    private bool isCancel;
    private bool isStart;

    private StageDataTable stageTable;
    public StageInfo stagePrefab;
    public GameObject canvas;

    public Stage currStage;
    private List<bool> stageState = new List<bool>();

    private AudioSource stageSelectAudioSource;
    public AudioClip stageSelectBGM;


    private void Awake()
    {
        stageSelectAudioSource = GetComponent<AudioSource>();
        stageSelectAudioSource.loop = true;

        //stageState.Add(true);
        //for (int i = 1; i < Player.Instance.stageClear.Count; ++i)
        //{
        //    stageState.Add(false);
        //}

        stageState = Player.Instance.StageClear;

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
       
        //SetButtons();


        //isCancel = false;
        //isStart = false;
        //StartCoroutine(SetButtonsAfterSceneLoad());
    }

    private void Start()
    {
        stageInfo = Instantiate(stagePrefab, canvas.transform);
        stageInfo.gameObject.SetActive(false);
        stageTable = DataTableMgr.Get<StageDataTable>(DataTableIds.StageTable);
        stageSelectAudioSource.PlayOneShot(stageSelectBGM);
        SetButtons();
        CheckToggleState();

        
    }

    private void Update()
    {
        
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

            var stageImage = FindChildWithTag(stages[i], "stageImage");
            //var toggle = stageButton.GetComponentInChildren<Toggle>();
            if (Player.Instance.stageClear[i])
            {
                //toggle.isOn = true;
                //var image = FindChildWithTag(stageButton, "lock").GetComponentInChildren<Image>();
                //SetImageAlpha(image, 0);
                //stageButtons[i].interactable = true;
                //toggle.interactable = false;

                var unlocked = FindChildWithTag(stageButton, "lock");
                unlocked.SetActive(true);
                stageImage.SetActive(true);
                stageButtons[i].interactable = true;
            }
            else
            {
                //toggle.isOn = false;
                //var image = FindChildWithTag(stageButton, "lock").GetComponentInChildren<Image>();
                //SetImageAlpha(image, 255);
                //stageButtons[i].interactable = false;
                //toggle.interactable = false;

                var unlocked = FindChildWithTag(stageButton, "lock");
                unlocked.SetActive(false);
                stageImage.SetActive(false);
                stageButtons[i].interactable = false;
            }
        }
    }

    private void PopStageInfo(int index)
    {
        var selectedStage = (Stage)(index + 3001);
        StageData stage = stageTable.Get(((int)selectedStage).ToString());

        stageInfo.gameObject.SetActive(true);
        stageInfo.stageName.text = stage.GetName;
        stageInfo.stageDescription.text = stage.GetInfo;
        stageInfo.stageImage.texture = stage.GetImage;

        stageInfo.cancel.onClick.RemoveAllListeners();
        stageInfo.cancel.onClick.AddListener(() => stageInfo.gameObject.SetActive(false));

        stageInfo.start.onClick.RemoveAllListeners();
        stageInfo.start.onClick.AddListener(() =>
        {
            currStage = selectedStage;
            Player.Instance.currentStage = ((int)currStage).ToString();
            stageSelectAudioSource.Stop();
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
        stageSelectAudioSource.Stop();
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

    private GameObject FindSiblingWithTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }

            GameObject result = FindSiblingWithTag(child.gameObject, tag);
            if (result != null)
            {
                return result;
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
