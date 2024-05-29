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

        if(Player.Instance.atkItemIndex < attackItemIds.Count)
        {
            CreateAtkItem(attackItemIds[Player.Instance.atkItemIndex], Player.Instance.atkItemIndex);
        }
        else
        {
            {
                CreateAtkItem(attackItemIds[Player.Instance.atkItemIndex-1], Player.Instance.atkItemIndex-1);
            }
        }

        if (Player.Instance.hpItemIndex < hpItemIds.Count)
        {
            CreateHpItem(hpItemIds[Player.Instance.hpItemIndex], Player.Instance.hpItemIndex);
        }
        else
        {
            {
                CreateHpItem(hpItemIds[Player.Instance.hpItemIndex - 1], Player.Instance.hpItemIndex - 1);
            }
        }

        if (Player.Instance.criticalItemIndex < criticalItemIds.Count)
        {
            CreateCriticalItem(criticalItemIds[Player.Instance.criticalItemIndex], Player.Instance.criticalItemIndex);
        }
        else
        {
            {
                CreateCriticalItem(criticalItemIds[Player.Instance.criticalItemIndex - 1], Player.Instance.criticalItemIndex - 1);
            }
        }

        ownGold.text = Player.Instance.Gold.ToString();
        lackOfGoldPopUp.SetActive(false);
    }

    private void CreateAtkItem(string itemId, int index)
    {
        ItemData itemData = itemTable.Get(itemId);
        currentAtkItem = Instantiate(itemPrefab, atkItemSpace.transform);
        currentAtkItem.transform.SetSiblingIndex(0);
        currentAtkItem.itemName.text = itemData.GetName;
        currentAtkItem.itemInfo.text = itemData.GetInfo;
        currentAtkItem.itemPrice.text = itemData.Gold.ToString();
        currentAtkItem.itemImage.texture = itemData.GetImage;
        currentAtkItem.purchaseButton.onClick.AddListener(() => PurchaseAtkItem(itemData));
        for(int i = 0; i < index; ++i)
        {
            currentAtkItem.toggles[i].isOn = true;
        }
        if (Player.Instance.atkItemIndex == attackItemIds.Count)
        {
            currentAtkItem.toggles[attackItemIds.Count - 1].isOn = true;
            currentAtkItem.purchaseButton.interactable = false;
        }
    }

    private void CreateHpItem(string itemId, int index)
    {
        ItemData itemData = itemTable.Get(itemId);
        currentHpItem = Instantiate(itemPrefab, hpItemSpace.transform);
        currentHpItem.transform.SetSiblingIndex(0);
        currentHpItem.itemName.text = itemData.GetName;
        currentHpItem.itemInfo.text = itemData.GetInfo;
        currentHpItem.itemPrice.text = itemData.Gold.ToString();
        currentHpItem.itemImage.texture = itemData.GetImage;
        currentHpItem.purchaseButton.onClick.AddListener(() => PurchaseHpItem(itemData)); 
        for (int i = 0; i < index; ++i)
        {
            currentHpItem.toggles[i].isOn = true;
        }
        if (Player.Instance.hpItemIndex == hpItemIds.Count)
        {
            currentHpItem.toggles[hpItemIds.Count - 1].isOn = true;
            currentHpItem.purchaseButton.interactable = false;
        }
    }

    private void CreateCriticalItem(string itemId, int index)
    {
        ItemData itemData = itemTable.Get(itemId);
        currentCriticalItem = Instantiate(itemPrefab, criticalItemSpace.transform);
        currentCriticalItem.transform.SetSiblingIndex(0);
        currentCriticalItem.itemName.text = itemData.GetName;
        currentCriticalItem.itemInfo.text = itemData.GetInfo;
        currentCriticalItem.itemPrice.text = itemData.Gold.ToString();
        currentCriticalItem.itemImage.texture = itemData.GetImage;
        currentCriticalItem.purchaseButton.onClick.AddListener(() => PurchaseCriticalItem(itemData));
        for (int i = 0; i < index; ++i)
        {
            currentCriticalItem.toggles[i].isOn = true;
        }
        if (Player.Instance.criticalItemIndex == criticalItemIds.Count)
        {
            currentCriticalItem.toggles[criticalItemIds.Count - 1].isOn = true;
            currentCriticalItem.purchaseButton.interactable = false;
        }
    }

    private void PurchaseAtkItem(ItemData itemData)
    {
        var price = int.Parse(currentAtkItem.itemPrice.text);

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

            Player.Instance.GainedAtk += itemData.Value;

            if (Player.Instance.atkItemIndex < attackItemIds.Count - 1)
            {
                ++Player.Instance.atkItemIndex;
                Destroy(currentAtkItem.gameObject);
                CreateAtkItem(attackItemIds[Player.Instance.atkItemIndex], Player.Instance.atkItemIndex);
            }
            else
            {
                ++Player.Instance.atkItemIndex;
                currentAtkItem.toggles[Player.Instance.atkItemIndex-1].isOn = true;
                currentAtkItem.purchaseButton.interactable = false;
                Debug.Log("모든 공격 아이템을 구매했습니다.");
            }

            UpdateToggle(currentAtkItem, Player.Instance.atkItemIndex);
        }
    }
    private void PurchaseHpItem(ItemData itemData)
    {
        var price = int.Parse(currentHpItem.itemPrice.text);

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
            Player.Instance.GainedHp += itemData.Value;

            if (Player.Instance.hpItemIndex < hpItemIds.Count - 1)
            {
                ++Player.Instance.hpItemIndex;
                Destroy(currentHpItem.gameObject);
                CreateHpItem(hpItemIds[Player.Instance.hpItemIndex], Player.Instance.hpItemIndex);
            }
            else
            {
                ++Player.Instance.hpItemIndex;
                currentHpItem.toggles[Player.Instance.hpItemIndex - 1].isOn = true;
                currentHpItem.purchaseButton.interactable = false;
                Debug.Log("모든 체력 아이템을 구매했습니다.");
            }

            UpdateToggle(currentHpItem, Player.Instance.hpItemIndex);
        }
    }

    private void PurchaseCriticalItem(ItemData itemData)
    {
        var price = int.Parse(currentCriticalItem.itemPrice.text);

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
            Player.Instance.GainedCritical += itemData.Value;

            if (Player.Instance.criticalItemIndex < criticalItemIds.Count - 1)
            {
                ++Player.Instance.criticalItemIndex;
                Destroy(currentCriticalItem.gameObject);
                CreateCriticalItem(criticalItemIds[Player.Instance.criticalItemIndex], Player.Instance.criticalItemIndex);
            }
            else
            {
                ++Player.Instance.criticalItemIndex;
                currentCriticalItem.toggles[Player.Instance.criticalItemIndex - 1].isOn = true;
                currentCriticalItem.purchaseButton.interactable = false;
                Debug.Log("모든 치명타 확률 아이템을 구매했습니다.");
            }

            UpdateToggle(currentCriticalItem, Player.Instance.criticalItemIndex);
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
