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

    // �׽�Ʈ�� �޼ҵ�
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
        // ����Ƽ �����Ϳ��� ���� �÷��̸� �����մϴ�
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnClickTutorialBack()
    {
        tutorialPopUp.SetActive(false);
    }
}
