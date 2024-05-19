using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameWindow : GenericWindow
{
    public void OnClickOK()
    {
        windowMgr.Open(Windows.Start);
    }
}
