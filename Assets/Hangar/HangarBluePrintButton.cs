using System;
using UnityEngine;
using UnityEngine.UI;

public class HangarBluePrintButton : MonoBehaviour
{
    public Image Icon;
    public Text NameText;
    public Color GreyOut;
    public Text ProgressText;

    public Button BuyButton;
    public Text BuyButtonText;

    private PlayerFile.InventoryItem _item;
    private BluePrint _bluePrint;
    private Action<HangarBluePrintButton, PlayerFile.InventoryItem> _onBuy;
    private Action<HangarBluePrintButton, PlayerFile.InventoryItem> _onEquip;

    public void Bind(PlayerFile.InventoryItem item, Action<HangarBluePrintButton, PlayerFile.InventoryItem> onBuy, Action<HangarBluePrintButton, PlayerFile.InventoryItem> onEquip)
    {
        _item = item;
        _bluePrint = BluePrintPool.ByKey(item.Key);
        var weaponDefinition = _bluePrint.Weapon;
        NameText.text = weaponDefinition.Name;
        Icon.sprite = weaponDefinition.InventorySprite;
        _onBuy = onBuy;
        _onEquip = onEquip;
        BuyButton.onClick.RemoveAllListeners();

        if (item.BluePrintsOwned >= _bluePrint.RequiredCount)
        {
            BuyButtonText.text = string.Format("Buy {0:N0}", _bluePrint.Price);
            NameText.color = Color.white;
            Icon.color = Color.white;
            ProgressText.enabled = false;
            BuyButton.gameObject.SetActive(true);
            BuyButton.onClick.AddListener(OnBuy);
        }
        else
        {
            BuyButton.gameObject.SetActive(false);
            NameText.color = GreyOut;
            Icon.color = GreyOut;
            ProgressText.text = string.Format("{0} / {1}", item.BluePrintsOwned, _bluePrint.RequiredCount);
        }
    }

    public void SetOwned(PlayerFile.InventoryItem item)
    {
        ProgressText.text = "OWNED";
        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(OnEquip);
    }

    public void SetAffordable(bool value)
    {
        if (value)
        {
            BuyButtonText.text = string.Format("Buy {0:N0}", _bluePrint.Price);
            BuyButton.interactable = true;
        }
        else
        {
            BuyButtonText.text = string.Format("<color=#e00>{0:N0}</color>", _bluePrint.Price);
            BuyButton.interactable = false;
        }
    }

    private void OnBuy()
    {
        if (_onBuy != null)
            _onBuy(this, _item);
    }

    private void OnEquip()
    {
        if (_onEquip != null)
            _onEquip(this, _item);
    }
}