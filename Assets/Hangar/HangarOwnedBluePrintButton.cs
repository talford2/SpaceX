using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HangarOwnedBluePrintButton : MonoBehaviour {

    public Image Icon;
    public Text NameText;
    public Button Button;

    private PlayerFile.InventoryItem _item;
    private Action<HangarOwnedBluePrintButton, PlayerFile.InventoryItem> _onBuy;
    private Action<HangarOwnedBluePrintButton, PlayerFile.InventoryItem> _onEquip;

    public void Bind(PlayerFile.InventoryItem item, Action<HangarOwnedBluePrintButton, PlayerFile.InventoryItem> onEquip)
    {
        _item = item;
        var bluePrint = BluePrintPool.ByKey(item.Key);
        var weaponItem = bluePrint.Item as WeaponDefinition;
        if (weaponItem != null)
        {
            var weaponDefinition = weaponItem;
            NameText.text = weaponDefinition.Name;
            Icon.sprite = weaponDefinition.InventorySprite;
            _onEquip = onEquip;
            Button.onClick.RemoveAllListeners();
            if (item.BluePrintsOwned >= bluePrint.RequiredCount)
            {
                /*
                if (item.IsOwned)
                {
                    //ProgressText.text = "OWNED";
                    Button.onClick.AddListener(OnEquip);
                }
                else
                {
                    //ProgressText.text = string.Format("{0:N0}", bluePrint.Price);
                    Button.onClick.AddListener(OnBuy);
                }
                */
                Button.onClick.AddListener(OnEquip);
            }
        }
    }

    public void SetOwned(PlayerFile.InventoryItem item)
    {
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
