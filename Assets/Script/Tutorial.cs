using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    StringTable stringTable;
    public Image[] pages;
    public Button[] rightButtons;
    public Button[] leftButtons;

    private void Start()
    {
        SetNormalState();


        foreach (var page in pages)
        {
            var back = FindChildWithTag(page.gameObject, "back");
            back.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetNormalState();
                gameObject.SetActive(false);
            });
        }


        for (int i = 0; i < rightButtons.Length; ++i)
        {
            int index = i;

            rightButtons[i].onClick.AddListener(() =>
            {
                pages[index].gameObject.SetActive(false);
                int nextIndex = (index + 1) % pages.Length;
                pages[nextIndex].gameObject.SetActive(true);
            });
        }

        for (int i = 0; i < leftButtons.Length; ++i)
        {
            int index = i;

            leftButtons[i].onClick.AddListener(() =>
            {
                pages[index].gameObject.SetActive(false);
                int prevIndex = (index - 1 + pages.Length) % pages.Length;
                pages[prevIndex].gameObject.SetActive(true);
            });
        }
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

    private void SetNormalState()
    {
        pages[0].gameObject.SetActive(true);
        pages[1].gameObject.SetActive(false);
        pages[2].gameObject.SetActive(false);
    }
}
