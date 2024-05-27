using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntranceScene : MonoBehaviour
{
    public GameObject upgradePopUp;
    public GameObject tutorialPopUp;

    private void Start()
    {
        upgradePopUp.SetActive(false);
        tutorialPopUp.SetActive(false);
    }

    // 테스트용 메소드
    public void Load()
    {
        SaveLoadSystem.Load();
    }

    public void Save()
    {
        SaveLoadSystem.Save();
    }

    public void OnClickGameStart()
    {
        SceneManager.LoadScene("Stagebackground");
    }

    public void OnClickUpgrade()
    {
        upgradePopUp.SetActive(true);
    }

    public void OnClickTutorial()
    {
        tutorialPopUp.SetActive(true);
    }

    public void OnClickQuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        // 유니티 에디터에서 게임 플레이를 종료합니다
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnClickTutorialBack()
    {
        tutorialPopUp.SetActive(false);
    }
}
