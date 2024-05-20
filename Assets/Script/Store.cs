using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public GameObject itemSlot;
    private ItemDataTable itemTable;

    public GameObject itemPrefab;


    private void Start()
    {
        itemTable = DataTableMgr.Get<ItemDataTable>(DataTableIds.ItemTable);

        for(int i = 0; i < itemTable.AllIds.Count; ++i)
        {
            string itemId = itemTable.AllIds[i];
            ItemData itemData = itemTable.Get(itemId);

            var item = Instantiate(itemPrefab, itemSlot.transform);
            var itemImage = FindChildWithTag(item.transform, "ItemImage")?.GetComponentInChildren<RawImage>();
            var textComponent1 = FindChildWithTag(item.transform, "ItemName")?.GetComponentInChildren<TextMeshProUGUI>();
            var textComponent2 = FindChildWithTag(item.transform, "ItemInfo")?.GetComponentInChildren<TextMeshProUGUI>();
            var textComponent3 = FindChildWithTag(item.transform, "Gold")?.GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent1 != null)
            {
                textComponent1.text = itemData.GetName;
            }

            if (textComponent2 != null)
            {
                textComponent2.text = itemData.GetInfo;
            }

            if(textComponent3 != null)
            {
                textComponent3.text = itemData.Gold.ToString();
            }

            itemImage.texture = itemData.GetImage;
        }


    }
    private Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
        }
        return null;
    }

    public void OnClickBack()
    {
        gameObject.SetActive(false);
    }
}
