using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntranceScene : MonoBehaviour
{
    public GameObject upgradePopUp;
    public GameObject tutorialPopUp;

    private AudioSource entranceAudioSource;
    public AudioClip entranceBGM;

    private void Start()
    {
        upgradePopUp.SetActive(false);
        tutorialPopUp.SetActive(false);
        entranceAudioSource = GetComponent<AudioSource>();
        entranceAudioSource.loop = true;
        entranceAudioSource.PlayOneShot(entranceBGM);

    }

    public void OnClickGameStart()
    {
        SceneManager.LoadScene("Stagebackground");
        entranceAudioSource.Stop();
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
        SaveLoadSystem.Save();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnClickTutorialBack()
    {
        tutorialPopUp.SetActive(false);
    }
}
