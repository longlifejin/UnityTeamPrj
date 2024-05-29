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
    public AudioSource stageSFXAudioSource;
    public AudioClip selectStageSFX;
    public AudioClip stageStartSFX;

    private void Awake()
    {
        stageSelectAudioSource = GetComponent<AudioSource>();
        stageSelectAudioSource.loop = true;
        stageSFXAudioSource.loop = false;

        stageState = Player.Instance.StageClear;
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
                stageSFXAudioSource.PlayOneShot(selectStageSFX);
                PopStageInfo(stagePopupIndex);                
            });

            var stageImage = FindChildWithTag(stages[i], "stageImage");
            if (Player.Instance.stageClear[i])
            {
                var unlocked = FindChildWithTag(stageButton, "lock");
                unlocked.SetActive(true);
                stageImage.SetActive(true);
                stageButtons[i].interactable = true;
            }
            else
            {
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
            stageSFXAudioSource.PlayOneShot(stageStartSFX);
            currStage = selectedStage;
            Player.Instance.currentStage = ((int)currStage).ToString();
            stageSelectAudioSource.Stop();
            SceneManager.LoadScene("Puzzle&Battle");
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
