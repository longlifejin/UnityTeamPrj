using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenericWindow : MonoBehaviour
{
    public GameObject firstSelected;
    private EventSystem eventSystem;

    protected EventSystem currentEventSystem;
    protected WindowMgr windowMgr;


    public EventSystem CurrentEventSystem
    {
        get
        {
            if (currentEventSystem == null)
                currentEventSystem = EventSystem.current;
            return currentEventSystem;
        }
    }

    public void SetWindowMgr(WindowMgr windowManager)
    {
        windowMgr = windowManager;
    }

    protected virtual void Awake()
    {
    }

    public void Init(WindowMgr mgr)
    {

    }

    public void OnFocus()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
            //지금 씬에 활성화된 이벤트 시스템 넘겨주는 것
        }
        CurrentEventSystem.SetSelectedGameObject(firstSelected);
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
        OnFocus();
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

}
