using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntranceScene : MonoBehaviour
{
    public GameObject upgradePopUp;

    public void OnClickGameStart()
    {
        //스테이지 선택 씬으로 수정하기
        SceneManager.LoadScene("Stagebackground");
    }

    public void OnClickUpgrade()
    {
        upgradePopUp.SetActive(true);
    }

    public void OnClickTutorial()
    {
        Debug.Log("튜토리얼 작업중");
    }
}
