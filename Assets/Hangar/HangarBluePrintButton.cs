using System;
using UnityEngine;
using UnityEngine.UI;

public class HangarBluePrintButton : MonoBehaviour
{
    public Image Icon;
    public Text NameText;
    public Text ProgressText;
    public Button Button;

    private PlayerFile.InventoryItem _item;
    private Action<HangarBluePrintButton, PlayerFile.InventoryItem> _onBuy;
    private Action<HangarBluePrintButton, PlayerFile.InventoryItem> _onEquip;

    public void Bind(PlayerFile.InventoryItem item, Action<HangarBluePrintButton, PlayerFile.InventoryItem> onBuy, Action<HangarBluePrintButton, PlayerFile.InventoryItem> onEquip)
    {
        _item = item;
        var bluePrint = BluePrintPool.ByKey(item.Key);
        var weaponDefinition = bluePrint.Weapon;
        NameText.text = weaponDefinition.Name;
        Icon.sprite = weaponDefinition.InventorySprite;
        _onBuy = onBuy;
        _onEquip = onEquip;
        Button.onClick.RemoveAllListeners();
        if (item.BluePrintsOwned >= bluePrint.RequiredCount)
        {
            if (item.IsOwned)
            {
                ProgressText.text = "OWNED";
                Button.onClick.AddListener(OnEquip);
            }
            else
            {
                ProgressText.text = string.Format("{0:N0}", bluePrint.Price);
                Button.onClick.AddListener(OnBuy);
            }
        }
        else
        {
            ProgressText.text = string.Format("{0} / {1}", item.BluePrintsOwned, bluePrint.RequiredCount);
        }
    }

    public void SetOwned(PlayerFile.InventoryItem item)
    {
        ProgressText.text = "OWNED";
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(OnEquip);
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