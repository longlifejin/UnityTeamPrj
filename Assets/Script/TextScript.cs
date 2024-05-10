using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    void Start()
    {
        var stringTable = DataTableMgr.Get<StringTable>(DataTableIds.String);
        Debug.Log(stringTable.Get("BossName1"));

        var playerTable = DataTableMgr.Get<PlayerDataTable>(DataTableIds.PlayerTable);
        Debug.Log(playerTable.Get("1001"));
    }

    void Update()
    {
        
    }
}
