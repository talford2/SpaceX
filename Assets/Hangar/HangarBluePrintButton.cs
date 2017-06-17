using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HangarBluePrintButton : MonoBehaviour
{
    public Image Icon;
    public Text NameText;
    public Text ProgressText;
    public Button Button;

    private PlayerFile.InventoryItem _item;
    private Action<PlayerFile.InventoryItem> _onClick;

    public void Bind(PlayerFile.InventoryItem item, Action<PlayerFile.InventoryItem> onClick)
    {
        _item = item;
        var bluePrint = BluePrintPool.ByKey(item.Key);
        var weaponDefinition = bluePrint.Weapon;
        NameText.text = weaponDefinition.Name;
        Icon.sprite = weaponDefinition.InventorySprite;
        ProgressText.text = string.Format("{0} / {1}", item.BluePrintsOwned, bluePrint.RequiredCount);
        _onClick = onClick;
        Button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (_onClick != null)
            _onClick(_item);
    }
}