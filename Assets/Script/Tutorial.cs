using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    StringTable stringTable;
    public TextMeshProUGUI[] tutorialTexts;

    private void Start()
    {
        stringTable = DataTableMgr.Get<StringTable>(DataTableIds.String);
        int index = 0;

        for(int i = 1; i <= 3; ++i)
        {
            for(int j = 1; j <= 6; ++j)
            {
                if(i ==3 && j == 6)
                {
                    return;
                }
                string Id = string.Format("Tutorial{0}_{1}", i, j);
                var str = stringTable.Get(Id);

                tutorialTexts[index].text = str;
                ++index;
            }
        }
    }
}
