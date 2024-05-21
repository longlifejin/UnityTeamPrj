using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Button purchaseButton;
    public RawImage itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemInfo;
    public TextMeshProUGUI itemPrice;
    public Toggle[] toggles;

    private void Awake()
    {
        foreach(var toggle in toggles)
        {
            toggle.isOn = false;
        }
    }
}
