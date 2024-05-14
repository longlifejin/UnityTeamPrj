using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : GenericWindow
{
    public Button buttonStart;
    public Button buttonStore;

    public void OnClickStart()
    {
        windowMgr.Open(Windows.InGame);
    }

    public void OnClickStore()
    {
        Debug.Log("Please wait next Version");
    }
}
