using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Store : MonoBehaviour
{
    public GameObject atkItemSpace;
    public GameObject hpItemSpace;
    public GameObject criticalItemSpace;
    public TextMeshProUGUI ownGold;
    private ItemDataTable itemTable;

    public ItemSlot itemPrefab;
    private List<string> attackItemIds = new List<string> { "41001", "41002", "41003", "41004", "41005" };
    private List<string> hpItemIds = new List<string> { "42001", "42002", "42003", "42004", "42005" };
    private List<string> criticalItemIds = new List<string> { "43001", "43002", "43003", "43004", "43005" };

    private ItemSlot currentAtkItem;
    private ItemSlot currentHpItem;
    private ItemSlot currentCriticalItem;

    public GameObject lackOfGoldPopUp;

    private AudioSource storeSFXAudioSource;
    public AudioClip purchaseSFX;

    private void Start()
    {
        itemTable = DataTableMgr.Get<ItemDataTable>(DataTableIds.ItemTable);
        storeSFXAudioSource = GetComponent<AudioSource>();

        InitializeItem(Player.Instance.atkItemIndex, attackItemIds, atkItemSpace, ref currentAtkItem, PurchaseAtkItem);
        InitializeItem(Player.Instance.hpItemIndex, hpItemIds, hpItemSpace, ref currentHpItem, PurchaseHpItem);
        InitializeItem(Player.Instance.criticalItemIndex, criticalItemIds, criticalItemSpace, ref currentCriticalItem, PurchaseCriticalItem);

        ownGold.text = Player.Instance.Gold.ToString();
        lackOfGoldPopUp.SetActive(false);
    }

    private void InitializeItem(int playerIndex, List<string> itemIds, GameObject itemSpace, ref ItemSlot currentItem, UnityEngine.Events.UnityAction<ItemData> purchaseAction)
    {
        if (playerIndex >= itemIds.Count)
        {
            playerIndex = itemIds.Count - 1;
        }

        ItemData itemData = itemTable.Get(itemIds[playerIndex]);
        currentItem = Instantiate(itemPrefab, itemSpace.transform);
        currentItem.transform.SetSiblingIndex(0);
        currentItem.itemName.text = itemData.GetName;
        currentItem.itemInfo.text = itemData.GetInfo;
        currentItem.itemPrice.text = itemData.Gold.ToString();
        currentItem.itemImage.texture = itemData.GetImage;
        currentItem.purchaseButton.onClick.AddListener(() => purchaseAction(itemData));

        for (int i = 0; i < playerIndex; ++i)
        {
            currentItem.toggles[i].isOn = true;
        }
        if (playerIndex == itemIds.Count)
        {
            currentItem.toggles[itemIds.Count - 1].isOn = true;
            currentItem.purchaseButton.interactable = false;
        }
    }

    private void PurchaseAtkItem(ItemData itemData)
    {
        PurchaseItem(itemData, attackItemIds, ref Player.Instance.atkItemIndex, ref currentAtkItem, atkItemSpace, Player.Instance.GainedAtk, val => Player.Instance.GainedAtk += val);
    }

    private void PurchaseHpItem(ItemData itemData)
    {
        PurchaseItem(itemData, hpItemIds, ref Player.Instance.hpItemIndex, ref currentHpItem, hpItemSpace, Player.Instance.GainedHp, val => Player.Instance.GainedHp += val);
    }

    private void PurchaseCriticalItem(ItemData itemData)
    {
        PurchaseItem(itemData, criticalItemIds, ref Player.Instance.criticalItemIndex, ref currentCriticalItem, criticalItemSpace, Player.Instance.GainedCritical, val => Player.Instance.GainedCritical += val);
    }

    private void PurchaseItem(ItemData itemData, List<string> itemIds, ref int playerIndex, ref ItemSlot currentItem, GameObject itemSpace, int playerStat, System.Action<int> updateStat)
    {
        var price = int.Parse(currentItem.itemPrice.text);

        if (Player.Instance.Gold < price)
        {
            lackOfGoldPopUp.SetActive(true);
            return;
        }
        else
        {
            storeSFXAudioSource.PlayOneShot(purchaseSFX);

            Player.Instance.Gold -= price;
            ownGold.text = Player.Instance.Gold.ToString();
            updateStat(itemData.Value);

            if (playerIndex < itemIds.Count - 1)
            {
                ++playerIndex;
                Destroy(currentItem.gameObject);
                InitializeItem(playerIndex, itemIds, itemSpace, ref currentItem, itemData =>
                {
                    if (itemIds == attackItemIds) PurchaseAtkItem(itemData);
                    else if (itemIds == hpItemIds) PurchaseHpItem(itemData);
                    else if (itemIds == criticalItemIds) PurchaseCriticalItem(itemData);
                });
            }
            else
            {
                ++playerIndex;
                currentItem.toggles[playerIndex - 1].isOn = true;
                currentItem.purchaseButton.interactable = false;
                Debug.Log("모든 아이템을 구매했습니다.");
            }

            UpdateToggle(currentItem, playerIndex);
        }
    }

    private void UpdateToggle(ItemSlot itemSlot, int index)
    {
        for (int i = 0; i < index; ++i)
        {
            itemSlot.toggles[i].isOn = true;
        }
    }

    public void OnClickLackOfGoldBack()
    {
        lackOfGoldPopUp.SetActive(false);
    }

    public void OnClickBack()
    {
        gameObject.SetActive(false);
    }
}