using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadsUpDisplay : MonoBehaviour
{
	public GameObject Crosshair;
	public Text EnergyText;
    public Text ShieldText;
	public Text HealthText;
    public Text SpaceJunkText;

    [Header("Bars")]
    public Image ShieldBar;
    public Image HealthBar;
    public Image BoostBar;

    [Header("Squadron")]
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

    // Space Junk label pulse
    private float spaceJunkPulseCooldown;
    private float spaceJunkPulseDuration = 0.3f;

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

            var shieldFraction = PlayerController.Current.VehicleInstance.Killable.Shield / PlayerController.Current.VehicleInstance.Killable.MaxShield;
            var healthFraction = PlayerController.Current.VehicleInstance.Killable.Health / PlayerController.Current.VehicleInstance.Killable.MaxHealth;
            var energyFraction = PlayerController.Current.VehicleInstance.BoostEnergy / PlayerController.Current.VehicleInstance.MaxBoostEnergy;

            ShieldBar.fillAmount = shieldFraction;
            HealthBar.fillAmount = healthFraction;
            BoostBar.fillAmount = energyFraction;
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

        if (spaceJunkPulseCooldown >= 0f)
        {
            spaceJunkPulseCooldown -= Time.deltaTime;
            if (spaceJunkPulseCooldown < 0f)
            {
                SpaceJunkText.fontSize = 30;
            }
            else
            {
                var pulseFraction = spaceJunkPulseCooldown / spaceJunkPulseDuration;
                SpaceJunkText.fontSize = Mathf.RoundToInt(Mathf.Lerp(50, 30, 1f - pulseFraction));
            }
        }
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

    public void IncreaseSpaceJunk()
    {
        SpaceJunkText.text = string.Format("{0:f0}", PlayerController.Current.SpaceJunkCount);
        spaceJunkPulseCooldown = spaceJunkPulseDuration;
        SpaceJunkText.fontSize = 50;
    }
}
