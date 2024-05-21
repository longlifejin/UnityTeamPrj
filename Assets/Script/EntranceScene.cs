using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntranceScene : MonoBehaviour
{
    public GameObject upgradePopUp;

    public void OnClickGameStart()
    {
        //�������� ���� ������ �����ϱ�
        SceneManager.LoadScene("Stagebackground");
    }

    public void OnClickUpgrade()
    {
        upgradePopUp.SetActive(true);
    }

    public void OnClickTutorial()
    {
        Debug.Log("Ʃ�丮�� �۾���");
    }
}
