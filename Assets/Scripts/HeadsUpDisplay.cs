using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadsUpDisplay : MonoBehaviour
{
	public GameObject Crosshair;
	public Text EnergyText;
    public Text ShieldText;
	public Text HealthText;

	public GameObject SquadronPrompt;
	public Text SquadronNameText;
	public float SquadronPromptTime = 1.5f;
    public GameObject SquadronIconContainer;
    public Image SquadronIcon;

	public Image HitImage;

	private float _squadronPromptCooldown;

	private static HeadsUpDisplay _current;

	public float HitFadeSpeed = 0.5f;

	private float _hitCooldown = 0;

    private List<SquadronIcon> _squadronIcons;

	public static HeadsUpDisplay Current { get { return _current; } }

	private void Awake()
	{
		_current = this;
		HitImage.color = new Color(1, 1, 1, 0);
        _squadronIcons = new List<SquadronIcon>();
	}

	private void Update()
	{
		if (PlayerController.Current.VehicleInstance != null)
		{
			EnergyText.text = string.Format("{0:f0}", PlayerController.Current.VehicleInstance.BoostEnergy);
            ShieldText.text = string.Format("{0:f0}", PlayerController.Current.VehicleInstance.Killable.Shield);
            HealthText.text = string.Format("{0:f0}", PlayerController.Current.VehicleInstance.Killable.Health);
		}
		if (_squadronPromptCooldown >= 0f)
		{
			_squadronPromptCooldown -= Time.deltaTime;
			if (_squadronPromptCooldown < 0f)
			{
				HideSquadronPrompt();
			}
		}
		HitImage.color = new Color(1, 1, 1, _hitCooldown);
		_hitCooldown -= Time.deltaTime * HitFadeSpeed;
		_hitCooldown = Mathf.Max(0, _hitCooldown);
	}

    public void LazyCreateSquadronIcons()
    {
        while (_squadronIcons.Count < PlayerController.Current.Squadron.GetMemberCount())
        {
            var icon = Instantiate(SquadronIcon);
            icon.transform.SetParent(SquadronIconContainer.transform);
            icon.rectTransform.localScale = Vector3.one;
            _squadronIcons.Add(icon.GetComponent<SquadronIcon>());
        }
    }

    public void RefreshSquadronIcon(int index)
    {
        var squadronVehicle = index == PlayerController.Current.Squadron.GetCurrentIndex()
            ? PlayerController.Current.VehicleInstance
            : PlayerController.Current.Squadron.GetMember(index).VehicleInstance;

        var squadronIcon = _squadronIcons[index];
        squadronIcon.SetCallSign(PlayerController.Current.Squadron.GetMember(index).GetComponent<ShipProfile>().CallSign);
        if (squadronVehicle != null && squadronVehicle.Killable.IsAlive)
        {
            squadronIcon.SetSelected(PlayerController.Current.Squadron.GetCurrentIndex() == index);
            squadronIcon.SetFractions(squadronVehicle.Killable.Shield/squadronVehicle.Killable.MaxShield, squadronVehicle.Killable.Health/squadronVehicle.Killable.MaxHealth);
        }
        else
        {
            squadronIcon.SetFractions(0f, 0f);
        }
    }

    public void RefreshSquadronIcons()
    {
        for (var i = 0; i < PlayerController.Current.Squadron.GetMemberCount(); i++)
        {
            RefreshSquadronIcon(i);
        }
    }

	public void Hit()
	{
		_hitCooldown = 1f;
	}

	public void ShowCrosshair()
	{
		Crosshair.SetActive(true);
	}

	public void HideCrosshair()
	{
		Crosshair.SetActive(false);
	}

	public void ShowSquadronPrompt(string message)
	{
		SquadronPrompt.SetActive(true);
		SquadronNameText.text = message;
		_squadronPromptCooldown = SquadronPromptTime;
		SquadronPrompt.GetComponent<Image>().CrossFadeAlpha(1f, 0.1f, false);
		SquadronNameText.CrossFadeAlpha(1f, 0.1f, false);
	}

	public void HideSquadronPrompt()
	{
		//SquadronPrompt.SetActive(false);
		SquadronPrompt.GetComponent<Image>().CrossFadeAlpha(0f, 0.5f, false);
		SquadronNameText.CrossFadeAlpha(0f, 0.5f, false);
	}
}
