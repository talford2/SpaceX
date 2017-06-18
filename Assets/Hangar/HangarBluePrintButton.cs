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
    private Action<HangarBluePrintButton, PlayerFile.InventoryItem> _onClick;

    public void Bind(PlayerFile.InventoryItem item, Action<HangarBluePrintButton, PlayerFile.InventoryItem> onClick)
    {
        _item = item;
        var bluePrint = BluePrintPool.ByKey(item.Key);
        var weaponDefinition = bluePrint.Weapon;
        NameText.text = weaponDefinition.Name;
        Icon.sprite = weaponDefinition.InventorySprite;
        _onClick = onClick;
        if (item.BluePrintsOwned >= bluePrint.RequiredCount)
        {
            if (item.IsOwned)
            {
                ProgressText.text = "OWNED";
                Button.onClick.AddListener(OnClick);
            }
            else
            {
                ProgressText.text = string.Format("{0:N0}", bluePrint.Price);
                Button.onClick.AddListener(OnClick);
            }
        }
        else
        {
            ProgressText.text = string.Format("{0} / {1}", item.BluePrintsOwned, bluePrint.RequiredCount);
        }
    }

    public void SetOwned(PlayerFile.InventoryItem item)
    {
        if (item.IsOwned)
        {
            ProgressText.text = "OWNED";
            Button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (_onClick != null)
            _onClick(this, _item);
    }
}