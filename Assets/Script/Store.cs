using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Store : MonoBehaviour
{
    public ScrollRect scrollRect;
    private ItemDataTable itemTable;

    public ItemSlot itemPrefab;
    //public Player player;


    private void Start()
    {
        itemTable = DataTableMgr.Get<ItemDataTable>(DataTableIds.ItemTable);

        for(int i = 0; i < itemTable.AllIds.Count; ++i)
        {
            string itemId = itemTable.AllIds[i];
            ItemData itemData = itemTable.Get(itemId);

            var item = Instantiate(itemPrefab, scrollRect.content);
            //item.itemImage = FindChildWithTag(item.transform, "ItemImage")?.GetComponentInChildren<RawImage>();
            //item.itemName = FindChildWithTag(item.transform, "ItemName")?.GetComponentInChildren<TextMeshProUGUI>();
            //item.itemInfo = FindChildWithTag(item.transform, "ItemInfo")?.GetComponentInChildren<TextMeshProUGUI>();
            //item.itemPrice = FindChildWithTag(item.transform, "Gold")?.GetComponentInChildren<TextMeshProUGUI>();

            item.itemName.text = itemData.GetName;
            item.itemInfo.text = itemData.GetInfo;
            item.itemPrice.text = itemData.Gold.ToString();
            item.itemImage.texture = itemData.GetImage;

            item.purchaseButton.onClick.AddListener(() =>
            {
                var price = int.Parse(item.itemPrice.text);
                if (Player.Instance.Gold < price)
                {
                    Debug.Log("소지한 골드가 부족합니다.");
                    return;
                }
                else
                {
                    Player.Instance.Gold -= price;
                    item.enabled = false;
                    Player.Instance.atk += itemData.Value;
                }
            });
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
