using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntranceScene : MonoBehaviour
{
    public GameObject upgradePopUp;

    public void OnClickGameStart()
    {
        SceneManager.LoadScene("Puzzle&Battle");
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
