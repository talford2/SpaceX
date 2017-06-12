using System;
using UnityEngine;
using UnityEngine.UI;

public class HangarWeaponButton : MonoBehaviour
{
    public Text NameText;
    public Image Icon;
    public Button Button;

    private WeaponDefinition _weaponDefinition;
    private Action<WeaponDefinition> _onClick;

    public void Bind(WeaponDefinition weaponDefinition, Action<WeaponDefinition> onClick)
    {
        _weaponDefinition = weaponDefinition;
        NameText.text = weaponDefinition.Name;
        Icon.sprite = weaponDefinition.InventorySprite;
        _onClick = onClick;
        Button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (_onClick != null)
            _onClick(_weaponDefinition);
    }
}
