using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadsUpDisplay : MonoBehaviour
{
	public GameObject Crosshair;
    public Image LeftHeatBar;
    public Image RightHeatBar;
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
    public Text MessageText;

	private float _squadronPromptCooldown;

	private static HeadsUpDisplay _current;

	public float HitFadeSpeed = 0.5f;

	private float _hitCooldown = 0;

	private List<SquadronIcon> _squadronIcons;

	public static HeadsUpDisplay Current { get { return _current; } }

	// Space Junk label pulse
	private float _spaceJunkPulseCooldown;
	private float _spaceJunkPulseDuration = 0.3f;

	// Shield hit effect
	private float _shieldHitDuration = 0.4f;
	private float _shieldHitCooldown;

	// Health hit effect
	private float _healthHitDuration = 0.4f;
	private float _healthHitCooldown;

	// Regenerate Energy effect
	private float _energyRegenDuration = 0.4f;
	private float _energyRegenCooldown;

    // Crosshair pulse effct
    private Image _crosshairImage;
    private float _crosshairPulseDuration = 0.5f;
    private float _crosshairPulseCooldown;

    // Crosshair fade out
    private bool _isCrosshairFadeOut;
    private float _crosshairFadeDuration = 1f;
    private float _crosshairFadeCooldown;

    // Message Prompt
    private bool _isShowingMessage;
    private float _messageCooldown;
    private float _messageFadeOutDuration = 1f;
    private float _messageFadeOutCooldown;

	private float _healthOpacity;
	private float _shieldOpacity;
	private float _boostOpacity;

	private void Awake()
	{
		_current = this;
		HitImage.color = new Color(1, 1, 1, 0);
		_squadronIcons = new List<SquadronIcon>();
        _crosshairImage = Crosshair.GetComponent<Image>();

		_healthOpacity = HealthBar.color.a;
		_shieldOpacity = ShieldBar.color.a;
		_boostOpacity = BoostBar.color.a;

        MessageText.enabled = false;
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

            var primaryWeaponInstance = PlayerController.Current.VehicleInstance.PrimaryWeaponInstance;
            var secondaryWeaponInstance = PlayerController.Current.VehicleInstance.SecondaryWeaponInstance;

            var leftHeatFraction = primaryWeaponInstance != null ? primaryWeaponInstance.GetHeatFraction() : 0f;
            var rightHeatFraction = secondaryWeaponInstance != null ? secondaryWeaponInstance.GetHeatFraction() : 0f;

            ShieldBar.fillAmount = shieldFraction;
            HealthBar.fillAmount = healthFraction;
            BoostBar.fillAmount = energyFraction;
            LeftHeatBar.fillAmount = leftHeatFraction;
            RightHeatBar.fillAmount = rightHeatFraction;
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

        if (_spaceJunkPulseCooldown >= 0f)
        {
            _spaceJunkPulseCooldown -= Time.deltaTime;
            if (_spaceJunkPulseCooldown < 0f)
            {
                SpaceJunkText.fontSize = 30;
            }
            else
            {
                var pulseFraction = _spaceJunkPulseCooldown / _spaceJunkPulseDuration;
                SpaceJunkText.fontSize = Mathf.RoundToInt(Mathf.Lerp(50, 30, 1f - pulseFraction));
            }
        }

        if (_shieldHitCooldown >= 0f)
        {
            _shieldHitCooldown -= Time.deltaTime;
            if (_shieldHitCooldown < 0f)
            {
                ShieldBar.color = Utility.SetColorAlpha(ShieldBar.color, _shieldOpacity);
            }
            else
            {
                var pulseFraction = _shieldHitCooldown / _shieldHitDuration;
                ShieldBar.color = Utility.SetColorAlpha(ShieldBar.color, (1 - _shieldOpacity) * pulseFraction + _shieldOpacity);
            }
        }

        if (_healthHitCooldown >= 0f)
        {
            _healthHitCooldown -= Time.deltaTime;
            if (_healthHitCooldown < 0f)
            {
                HealthBar.color = Utility.SetColorAlpha(HealthBar.color, _healthOpacity);
            }
            else
            {
                var pulseFraction = _healthHitCooldown / _healthHitDuration;
                HealthBar.color = Utility.SetColorAlpha(HealthBar.color, (1 - _healthOpacity) * pulseFraction + _healthOpacity);
            }
        }

        if (_energyRegenCooldown >= 0f)
        {
            _energyRegenCooldown -= Time.deltaTime;
            if (_energyRegenCooldown < 0f)
            {
                BoostBar.color = Utility.SetColorAlpha(BoostBar.color, _boostOpacity);
            }
            else
            {
                var pulseFraction = _energyRegenCooldown / _energyRegenDuration;
                BoostBar.color = Utility.SetColorAlpha(BoostBar.color, (1 - _boostOpacity) * pulseFraction + _boostOpacity);
            }
        }

        if (_crosshairPulseCooldown >= 0f)
        {
            _crosshairPulseCooldown -= Time.deltaTime;
            if (_crosshairPulseCooldown < 0f)
            {
                _crosshairImage.rectTransform.sizeDelta = new Vector2(64f, 64f);
            }
            else
            {
                var pulseFraction = _crosshairPulseCooldown / _crosshairPulseDuration;
                _crosshairImage.rectTransform.sizeDelta = Vector2.Lerp(new Vector2(90f, 90f), new Vector2(64f, 64f), 1f - pulseFraction);
            }
        }

        if (_messageCooldown >= 0f)
        {
            _messageCooldown -= Time.deltaTime;
            if (_messageCooldown < 0f)
            {
                _messageFadeOutCooldown = _messageFadeOutDuration;

            }
        }
        if (_messageFadeOutCooldown >= 0f)
        {
            _messageFadeOutCooldown -= Time.deltaTime;
            var fadeFraction = Mathf.Clamp01(_messageFadeOutCooldown / _messageFadeOutDuration);
            if (_messageFadeOutCooldown < 0f)
            {
                MessageText.enabled = false;
            }
            else
            {
                MessageText.color = Utility.SetColorAlpha(MessageText.color, fadeFraction);
            }
        }
        if (_isCrosshairFadeOut)
        {
            if (_crosshairFadeCooldown >= 0f)
            {
                _crosshairFadeCooldown -= Time.deltaTime;
                if (_crosshairFadeCooldown < 0f)
                {
                    HideCrosshair();
                    _isCrosshairFadeOut = false;
                }
                else
                {
                    var crosshairImage = Crosshair.GetComponent<Image>();
                    var fadeFraction = Mathf.Clamp01(_crosshairFadeCooldown / _crosshairFadeDuration);
                    crosshairImage.color = Utility.SetColorAlpha(crosshairImage.color, fadeFraction);
                }
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
			squadronIcon.SetFractions(squadronVehicle.Killable.Shield / squadronVehicle.Killable.MaxShield, squadronVehicle.Killable.Health / squadronVehicle.Killable.MaxHealth);
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
        var crosshairImage = Crosshair.GetComponent<Image>();
        crosshairImage.color = Utility.SetColorAlpha(crosshairImage.color, 1f);
        _isCrosshairFadeOut = false;
        LeftHeatBar.enabled = true;
        RightHeatBar.enabled = true;
    }

    public void HideCrosshair()
	{
		Crosshair.SetActive(false);
        LeftHeatBar.enabled = false;
        RightHeatBar.enabled = false;
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
		_spaceJunkPulseCooldown = _spaceJunkPulseDuration;
		SpaceJunkText.fontSize = 50;
	}

	public void TriggerShieldHit()
	{
		ShieldBar.color = Utility.SetColorAlpha(ShieldBar.color, 1f);
		_shieldHitCooldown = _shieldHitDuration;
	}

	public void TriggerHealthHit()
	{
		HealthBar.color = Utility.SetColorAlpha(HealthBar.color, 1f);
		_healthHitCooldown = _healthHitDuration;
	}

	public void TriggerPuslEnergy()
	{
		BoostBar.color = Utility.SetColorAlpha(BoostBar.color, 1f);
		_energyRegenCooldown = _energyRegenDuration;
	}

    public void TriggerCrosshairPulse()
    {
        _crosshairImage.rectTransform.sizeDelta = new Vector2(90f, 90f);
        _crosshairPulseCooldown = _crosshairPulseDuration;
    }

    public void TriggerCrosshairFadeOut()
    {
        ShowCrosshair();
        _crosshairFadeCooldown = _crosshairFadeDuration;
        _isCrosshairFadeOut = true;
    }

    public void DisplayMessage(string message, float time)
    {
        MessageText.text = message;
        MessageText.color = Utility.SetColorAlpha(MessageText.color, 1f);
        MessageText.enabled = true;
        _messageCooldown = time;
    }
}
