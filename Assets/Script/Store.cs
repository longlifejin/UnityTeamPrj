using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static UnityEditor.Progress;

public class Store : MonoBehaviour
{
    public ScrollRect scrollRect;
    private ItemDataTable itemTable;

    public ItemSlot itemPrefab;
    private List<string> attackItemIds = new List<string> { "41001", "41002", "41003", "41004", "41005" };
    private List<string> hpItemIds = new List<string> { "42001", "42002", "42003", "42004", "42005" };
    private int currentAtkIndex = 0;
    private int currentHpIndex = 0;
    private ItemSlot currentItem;

    private void Start()
    {
        itemTable = DataTableMgr.Get<ItemDataTable>(DataTableIds.ItemTable);
        CreateItem(attackItemIds[currentAtkIndex]);
        CreateItem(hpItemIds[currentHpIndex]);

        //for(int i = 0; i < itemTable.AllIds.Count; ++i)
        //{
        //    string itemId = itemTable.AllIds[i];
        //    ItemData itemData = itemTable.Get(itemId);

        //    var item = Instantiate(itemPrefab, scrollRect.content);

        //    item.itemName.text = itemData.GetName;
        //    item.itemInfo.text = itemData.GetInfo;
        //    item.itemPrice.text = itemData.Gold.ToString();
        //    item.itemImage.texture = itemData.GetImage;

        //    item.purchaseButton.onClick.AddListener(() =>
        //    {
        //        var price = int.Parse(item.itemPrice.text);
        //        if (Player.Instance.Gold < price)
        //        {
        //            Debug.Log("소지한 골드가 부족합니다.");
        //            return;
        //        }
        //        else
        //        {
        //            Player.Instance.Gold -= price;
        //            item.enabled = false;
        //            Player.Instance.atk += itemData.Value;
        //        }
        //    });
        //}
    }

    private void CreateItem(string itemId)
    {
        ItemData itemData = itemTable.Get(itemId);
        currentItem = Instantiate(itemPrefab, scrollRect.content);
        currentItem.itemName.text = itemData.GetName;
        currentItem.itemInfo.text = itemData.GetInfo;
        currentItem.itemPrice.text = itemData.Gold.ToString();
        currentItem.itemImage.texture = itemData.GetImage;

        currentItem.purchaseButton.onClick.AddListener(() => PurchaseAtkItem(itemData));
    }

    private void PurchaseAtkItem(ItemData itemData)
    {
        var price = int.Parse(currentItem.itemPrice.text);

        if (Player.Instance.Gold < price)
        {
            Debug.Log("소지한 골드가 부족합니다.");
            return;
        }
        else
        {
            Player.Instance.Gold -= price;
            Player.Instance.atk += itemData.Value;

            Destroy(currentItem.gameObject);

            if(itemData.Item_type == 1)
            {
                currentAtkIndex++;
                if (currentAtkIndex < attackItemIds.Count)
                {
                    CreateItem(attackItemIds[currentAtkIndex]);
                }
                else
                {
                    currentItem.purchaseButton.interactable = false;
                    Debug.Log("모든 아이템을 구매했습니다.");
                }
            }
            else if(itemData.Item_type == 2)
            {
                currentHpIndex++;
                if (currentHpIndex < attackItemIds.Count)
                {
                    CreateItem(attackItemIds[currentHpIndex]);
                }
                else
                {
                    currentItem.purchaseButton.interactable = false;
                    Debug.Log("모든 아이템을 구매했습니다.");
                }
            }
            
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
