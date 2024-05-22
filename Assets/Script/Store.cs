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
    public GameObject atkItemSpace;
    public GameObject hpItemSpace;
    public TextMeshProUGUI ownGold;
    private ItemDataTable itemTable;


    public ItemSlot itemPrefab;
    private List<string> attackItemIds = new List<string> { "41001", "41002", "41003", "41004", "41005" };
    private List<string> hpItemIds = new List<string> { "42001", "42002", "42003", "42004", "42005" };
    private int currentAtkIndex = 0;
    private int currentHpIndex = 0;
    private ItemSlot currentAtkItem;
    private ItemSlot currentHpItem;

    private void Start()
    {
        itemTable = DataTableMgr.Get<ItemDataTable>(DataTableIds.ItemTable);
        CreateAtkItem(attackItemIds[currentAtkIndex]);
        CreateHpItem(hpItemIds[currentHpIndex]);
        ownGold.text = Player.Instance.Gold.ToString();
    }

    private void CreateAtkItem(string itemId)
    {
        ItemData itemData = itemTable.Get(itemId);
        currentAtkItem = Instantiate(itemPrefab, atkItemSpace.transform);
        currentAtkItem.itemName.text = itemData.GetName;
        currentAtkItem.itemInfo.text = itemData.GetInfo;
        currentAtkItem.itemPrice.text = itemData.Gold.ToString();
        currentAtkItem.itemImage.texture = itemData.GetImage;
        currentAtkItem.purchaseButton.onClick.AddListener(() => PurchaseAtkItem(itemData));
    }

    private void CreateHpItem(string itemId)
    {
        ItemData itemData = itemTable.Get(itemId);
        currentHpItem = Instantiate(itemPrefab, hpItemSpace.transform);
        currentHpItem.itemName.text = itemData.GetName;
        currentHpItem.itemInfo.text = itemData.GetInfo;
        currentHpItem.itemPrice.text = itemData.Gold.ToString();
        currentHpItem.itemImage.texture = itemData.GetImage;
        currentHpItem.purchaseButton.onClick.AddListener(() => PurchaseHpItem(itemData));
    }

    private void PurchaseAtkItem(ItemData itemData)
    {
        var price = int.Parse(currentAtkItem.itemPrice.text);

        if (Player.Instance.Gold < price)
        {
            Debug.Log("소지한 골드가 부족합니다.");
            return;
        }
        else
        {
            Player.Instance.Gold -= price;
            ownGold.text = Player.Instance.Gold.ToString();

            Player.Instance.atk += itemData.Value;

            if (currentAtkIndex < attackItemIds.Count - 1)
            {
                currentAtkIndex++;
                Destroy(currentAtkItem.gameObject);
                CreateAtkItem(attackItemIds[currentAtkIndex]);
            }
            else
            {
                currentAtkItem.toggles[currentAtkIndex].isOn = true;
                currentAtkItem.purchaseButton.interactable = false;
                Debug.Log("모든 공격 아이템을 구매했습니다.");
            }

            UpdateToggle(currentAtkItem, currentAtkIndex);
        }
    }
    private void PurchaseHpItem(ItemData itemData)
    {
        var price = int.Parse(currentHpItem.itemPrice.text);

        if (Player.Instance.Gold < price)
        {
            Debug.Log("소지한 골드가 부족합니다.");
            return;
        }
        else
        {
            Player.Instance.Gold -= price;
            Player.Instance.hp += itemData.Value;

            if (currentHpIndex < hpItemIds.Count - 1)
            {
                currentHpIndex++;
                Destroy(currentHpItem.gameObject);
                CreateHpItem(hpItemIds[currentHpIndex]);
            }
            else
            {
                currentHpItem.toggles[currentHpIndex].isOn = true;
                currentHpItem.purchaseButton.interactable = false;
                Debug.Log("모든 체력 아이템을 구매했습니다.");
            }

            UpdateToggle(currentHpItem, currentHpIndex);
        }
    }

    private void UpdateToggle(ItemSlot itemSlot, int index)
    {
        for (int i = 0; i < index; ++i)
        {
            itemSlot.toggles[i].isOn = i <= index;
        }
    }

    //private Transform FindChildWithTag(Transform parent, string tag)
    //{
    //    foreach (Transform child in parent.GetComponentsInChildren<Transform>())
    //    {
    //        if (child.CompareTag(tag))
    //        {
    //            return child;
    //        }
    //    }
    //    return null;
    //}

    public void OnClickBack()
    {
        gameObject.SetActive(false);
    }
}
