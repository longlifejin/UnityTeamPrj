using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindowMgr : MonoBehaviour
{
    private static WindowMgr instance;

    public GenericWindow[] windows;
    public Windows defaultWindowId; //맨 처음 열릴 윈도우ID
    private int currentWindowId;

    private void Start()
    {
        foreach (var window in windows)
        {
            window.gameObject.SetActive(false);
            window.SetWindowMgr(this);
        }
        Open(defaultWindowId);
    }

    public GenericWindow Get(int windowId)
    {
        return windows[windowId];
    }

    public GenericWindow Open(Windows windowId)
    {
        return Open((int)windowId);
    }

    public GenericWindow Open(int windowId)
    {
        windows[currentWindowId].Close();
        currentWindowId = windowId;
        windows[currentWindowId].Open();

        return windows[currentWindowId];
    }
}
