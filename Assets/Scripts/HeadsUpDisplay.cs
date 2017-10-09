using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadsUpDisplay : MonoBehaviour
{
    public CanvasGroup CoverScreen;
    public GameObject Crosshair;
    public bool StartHidden = true;
    public bool DisplayTeamScore;
    public bool DisplayCredits;

    [Header("Crosshair Hit Response")]
    public Image CrosshairHit;
    public float CrosshairHitDuration = 0.5f;

    [Header("Heat Bars")]
    public CanvasGroup HeatBarsContainer;
    public Image LeftHeatBar;
    public Image RightHeatBar;
    public Image PrimaryHeatBar;
    public Image SecondaryHeatBar;
    public Gradient HeatGradient;
    public CanvasGroup OverheatMessage;

    [Header("Text")]
    public Text EnergyText;
    public Text ShieldText;
    public Text HealthText;
    public Text SpaceJunkText;

    public Text KillText;
    private float _killCoolDown = 0f;
    public float DisplayKillTime = 0.5f;
    public float DisplayKillFadeTime = 0.5f;
    public float DisplayKillZDepth = 40f;
    public Vector3 _initKillTextPos;
    public Text KilledText;

    [Header("Panels Bar")]
    public CanvasGroup SquadronPanel;
    public CanvasGroup KillCountPanel;
    public CanvasGroup SpaceJunkPanel;
    public CanvasGroup HealthShieldPanel;
    public CanvasGroup BoostPanel;

    [Header("Bars")]
    public Image ShieldBar;
    public Image HealthBar;
    public Image BoostBar;
    public Gradient HealthGradient;

    [Header("Squadron")]
    public GameObject SquadronPrompt;
    public Text SquadronNameText;
    public float SquadronPromptTime = 1.5f;
    public GameObject SquadronIconContainer;
    public Image SquadronIcon;

    public Image ShieldHitImage;
    public Image HitImage;
    public Text MessageText;

    [Header("Team Score")]
    public Text GoodKills;
    public Text BadKills;

    private float _squadronPromptCooldown;

    private static HeadsUpDisplay _current;

    [Header("Hurt Screen")]
    public float HurtFadeSpeed = 0.5f;
    public float ShieldHurtFadeSpeed = 0.5f;

    private float _crosshairHitCooldown;

    private float _hurtCooldown = 0f;
    private float _shieldHurtCooldown = 0f;
    private bool _isDead;

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
    private Vector2 _crosshairOriginalScale;
    private Vector2 _crosshairPuslseScale;
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

    // Kill counter
    private Dictionary<Team, int> _killCount;

    // Cover Screen
    private float _coverTime = 0.5f;
    private float _coverCooldown;

    // Heat Message
    private bool _isOverheated;
    private float _overheatBlinkCooldown;
    private float _overheatBlinkTime = 0.5f;

    private void Awake()
    {
        _current = this;
        HitImage.color = new Color(1, 1, 1, 0);
        _squadronIcons = new List<SquadronIcon>();
        _crosshairImage = Crosshair.GetComponent<Image>();

        _crosshairOriginalScale = _crosshairImage.rectTransform.sizeDelta;
        _crosshairPuslseScale = 1.5f * _crosshairOriginalScale;

        _healthOpacity = HealthBar.color.a;
        _shieldOpacity = ShieldBar.color.a;
        _boostOpacity = BoostBar.color.a;

        _killCount = new Dictionary<Team, int>();
        _killCount.Add(Team.Bad, 0);
        _killCount.Add(Team.Good, 0);
        _killCount.Add(Team.Neutral, 0);

        _isDead = false;
        UpdateKillsDisplay();

        MessageText.enabled = false;

        KillText.text = "";
        KillText.color = Utility.SetColorAlpha(KillText.color, 0);
        _initKillTextPos = KillText.rectTransform.localPosition;

        SquadronPanel.gameObject.SetActive(Player.Current.Squadron.Members.Count > 1);

        _coverCooldown = _coverTime;

        if (StartHidden)
            Hide();
    }

    public void ShowKillMessage(string message)
    {
        KillText.text = message.ToUpper();
        KillText.color = Utility.SetColorAlpha(KillText.color, 1);
        _killCoolDown = DisplayKillTime + DisplayKillFadeTime;
        KillText.rectTransform.localPosition = _initKillTextPos;
        Debug.Log("Kill message: " + message);
    }

    public void UpdateBars()
    {
        var shieldFraction = Player.Current.VehicleInstance.Killable.Shield / Player.Current.VehicleInstance.Killable.MaxShield;
        var healthFraction = Player.Current.VehicleInstance.Killable.Health / Player.Current.VehicleInstance.Killable.MaxHealth;
        var energyFraction = Player.Current.VehicleInstance.BoostEnergy / Player.Current.VehicleInstance.EngineInstance.BoostEnergy;

        ShieldBar.fillAmount = shieldFraction;
        HealthBar.fillAmount = healthFraction;
        HealthBar.color = Utility.SetColorAlpha(HealthGradient.Evaluate(1f - healthFraction), HealthBar.color.a);
        BoostBar.fillAmount = energyFraction;
        BoostBar.color = Utility.SetColorAlpha(HealthGradient.Evaluate(1f - energyFraction), BoostBar.color.a);
    }

    private void Update()
    {
        if (_coverCooldown > 0f)
        {
            _coverCooldown -= Time.deltaTime;
            var fraction = Mathf.Abs(_coverCooldown / _coverTime);
            CoverScreen.alpha = fraction;
            if (_coverCooldown < 0f)
                CoverScreen.alpha = 0f;
        }
        if (_killCoolDown > 0)
        {
            _killCoolDown -= Time.deltaTime;
            if (_killCoolDown < DisplayKillTime)
            {
                var fade = Mathf.Clamp01(_killCoolDown / DisplayKillFadeTime);
                KillText.color = Utility.SetColorAlpha(KillText.color, fade);
                KillText.rectTransform.localPosition = Vector3.Lerp(_initKillTextPos, new Vector3(_initKillTextPos.x, _initKillTextPos.y, DisplayKillZDepth), 1f - fade);
            }
        }

        if (Player.Current.VehicleInstance != null)
        {
            EnergyText.text = string.Format("{0:f0}", Player.Current.VehicleInstance.BoostEnergy);
            ShieldText.text = string.Format("{0:f0}", Player.Current.VehicleInstance.Killable.Shield);
            HealthText.text = string.Format("{0:f0}", Player.Current.VehicleInstance.Killable.Health);

            var primaryWeaponInstance = Player.Current.VehicleInstance.PrimaryWeaponInstance;
            var secondaryWeaponInstance = Player.Current.VehicleInstance.SecondaryWeaponInstance;

            var leftHeatFraction = primaryWeaponInstance != null ? primaryWeaponInstance.GetHeatFraction() : 0f;
            var rightHeatFraction = secondaryWeaponInstance != null ? secondaryWeaponInstance.GetHeatFraction() : 0f;

            LeftHeatBar.fillAmount = leftHeatFraction;
            RightHeatBar.fillAmount = rightHeatFraction;

            if (leftHeatFraction == 1f || rightHeatFraction == 1f)
            {
                TriggerOverheatMessage();
            }
            else
            {
                ReleaseOverheatMessage();
            }

            PrimaryHeatBar.fillAmount = (120f / 360f) * leftHeatFraction;
            PrimaryHeatBar.color = HeatGradient.Evaluate(leftHeatFraction);
            SecondaryHeatBar.fillAmount = (120f / 360f) * rightHeatFraction;
            SecondaryHeatBar.color = HeatGradient.Evaluate(rightHeatFraction);

            UpdateBars();
        }
        if (_squadronPromptCooldown >= 0f)
        {
            _squadronPromptCooldown -= Time.deltaTime;
            if (_squadronPromptCooldown < 0f)
            {
                HideSquadronPrompt();
            }
        }

        if (!_isDead)
        {
            HitImage.color = new Color(1, 1, 1, _hurtCooldown);
            _hurtCooldown -= Time.deltaTime * HurtFadeSpeed;
            _hurtCooldown = Mathf.Max(0, _hurtCooldown);

            ShieldHitImage.color = new Color(1, 1, 1, _shieldHurtCooldown);
            _shieldHurtCooldown -= Time.deltaTime * ShieldHurtFadeSpeed;
            _shieldHurtCooldown = Mathf.Max(0, _shieldHurtCooldown);
        }
        else
        {
            ShieldHitImage.color = new Color(1f, 1f, 1f, 0f);
            HitImage.color = new Color(1f, 1f, 1f, 1f);
        }

        if (_crosshairHitCooldown >= 0f)
        {
            _crosshairHitCooldown -= Time.deltaTime;
            var fraction = Mathf.Clamp01(_crosshairHitCooldown / CrosshairHitDuration);
            if (_crosshairHitCooldown < 0f)
            {
                fraction = 0;
            }
            CrosshairHit.color = Utility.SetColorAlpha(CrosshairHit.color, fraction);
        }

        if (DisplayCredits)
        {
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

        if (_isOverheated)
        {
            _overheatBlinkCooldown -= Time.deltaTime;
            if (_overheatBlinkCooldown < 0f)
            {
                OverheatMessage.alpha = OverheatMessage.alpha > 0f ? 0f : 1f;
                _overheatBlinkCooldown = _overheatBlinkTime;
            }
        }
        else
        {
            OverheatMessage.alpha = 0f;
        }

        /*
        if (_crosshairPulseCooldown >= 0f)
        {
            _crosshairPulseCooldown -= Time.deltaTime;
            if (_crosshairPulseCooldown < 0f)
            {
                _crosshairImage.rectTransform.sizeDelta = _crosshairOriginalScale;
            }
            else
            {
                var pulseFraction = _crosshairPulseCooldown / _crosshairPulseDuration;
                _crosshairImage.rectTransform.sizeDelta = Vector2.Lerp(_crosshairPuslseScale, _crosshairOriginalScale, 1f - pulseFraction);
            }
        }
        */

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
                    HeatBarsContainer.alpha = fadeFraction;
                }
            }
        }
    }

    public void LazyCreateSquadronIcons()
    {
        while (_squadronIcons.Count < Player.Current.Squadron.GetMemberCount())
        {
            var icon = Instantiate(SquadronIcon);
            icon.transform.SetParent(SquadronIconContainer.transform);
            icon.rectTransform.localScale = Vector3.one;
            icon.rectTransform.localPosition = Vector3.zero;
            _squadronIcons.Add(icon.GetComponent<SquadronIcon>());
        }
    }

    public void RefreshSquadronIcon(int index)
    {
        var squadronVehicle = index == Player.Current.Squadron.GetCurrentIndex()
            ? Player.Current.VehicleInstance
            : Player.Current.Squadron.GetMember(index).VehicleInstance;

        var squadronIcon = _squadronIcons[index];
        squadronIcon.SetCallSign(Player.Current.Squadron.GetMember(index).GetComponent<ShipProfile>().CallSign);
        if (squadronVehicle != null && squadronVehicle.Killable.IsAlive)
        {
            squadronIcon.SetSelected(Player.Current.Squadron.GetCurrentIndex() == index);
            squadronIcon.SetFractions(squadronVehicle.Killable.Shield / squadronVehicle.Killable.MaxShield, squadronVehicle.Killable.Health / squadronVehicle.Killable.MaxHealth);
        }
        else
        {
            squadronIcon.SetFractions(0f, 0f);
        }
    }

    public void RefreshSquadronIcons()
    {
        for (var i = 0; i < Player.Current.Squadron.GetMemberCount(); i++)
        {
            RefreshSquadronIcon(i);
        }
    }

    public void ShieldHit()
    {
        _shieldHurtCooldown = 1f;
    }

    public void Hit()
    {
        _hurtCooldown = 1f;
    }

    public void ShowDead()
    {
        _shieldHurtCooldown = 0f;
        _hurtCooldown = 0f;
        _isDead = true;
    }

    public void ShowAlive()
    {
        _shieldHurtCooldown = 0f;
        _hurtCooldown = 0f;
        _isDead = false;
        KilledText.color = Utility.SetColorAlpha(KilledText.color, 0f);
    }

    public void ShowKilledMessage(string message)
    {
        KilledText.text = message;
        KilledText.color = Utility.SetColorAlpha(KilledText.color, 1f);
    }

    public void ShowCrosshair()
    {
        Crosshair.SetActive(true);
        var crosshairImage = Crosshair.GetComponent<Image>();
        crosshairImage.color = Utility.SetColorAlpha(crosshairImage.color, 1f);
        _isCrosshairFadeOut = false;
        HeatBarsContainer.alpha = 1f;
    }

    public void HideCrosshair()
    {
        Crosshair.SetActive(false);
        HeatBarsContainer.alpha = 0f;
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

    public void UpdateSpaceJunk()
    {
        if (DisplayCredits)
        {
            SpaceJunkText.text = string.Format("{0:N0}", Mission.Current.GetEarnedCredits());
            _spaceJunkPulseCooldown = _spaceJunkPulseDuration;
            SpaceJunkText.fontSize = 50;
        }
    }

    public void TriggerShieldHit()
    {
        HitImage.color = new Color(1f, 1f, 1f, 0f);
        _healthHitCooldown = 0f;

        ShieldBar.color = Utility.SetColorAlpha(ShieldBar.color, 1f);
        _shieldHitCooldown = _shieldHitDuration;
    }

    public void TriggerHealthHit()
    {
        ShieldHitImage.color = new Color(1f, 1f, 1f, 0f);
        _shieldHitCooldown = 0f;

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
        //_crosshairImage.rectTransform.sizeDelta = _crosshairPuslseScale;
        //_crosshairPulseCooldown = _crosshairPulseDuration;

        _crosshairHitCooldown = CrosshairHitDuration;
        CrosshairHit.color = Utility.SetColorAlpha(CrosshairHit.color, Mathf.Clamp01(_crosshairHitCooldown / CrosshairHitDuration));
    }

    public void TriggerCrosshairFadeOut()
    {
        ShowCrosshair();
        _crosshairFadeCooldown = _crosshairFadeDuration;
        _isCrosshairFadeOut = true;
    }

    public void TriggerOverheatMessage()
    {
        _isOverheated = true;
    }

    public void ReleaseOverheatMessage()
    {
        _isOverheated = false;
    }

    public void RecordKill(Team team)
    {
        _killCount[team]++;
        UpdateKillsDisplay();
    }

    private void UpdateKillsDisplay()
    {
        if (DisplayTeamScore)
        {
            GoodKills.text = string.Format("{0}", _killCount[Team.Good]);
            BadKills.text = string.Format("{0}", _killCount[Team.Bad]);
        }
    }

    public void DisplayMessage(string message, float time)
    {
        MessageText.text = message;
        MessageText.color = Utility.SetColorAlpha(MessageText.color, 1f);
        MessageText.enabled = true;
        _messageCooldown = time;
    }

    public void Hide()
    {
        HideCrosshair();
        SquadronPanel.alpha = 0f;
        KillCountPanel.alpha = 0f;
        SpaceJunkPanel.alpha = 0f;
        HealthShieldPanel.alpha = 0f;
        BoostPanel.alpha = 0;
    }

    public void Show()
    {
        ShowCrosshair();
        SquadronPanel.alpha = 1f;
        if (DisplayTeamScore)
            KillCountPanel.alpha = 1f;
        if (DisplayCredits)
            SpaceJunkPanel.alpha = 1f;
        HealthShieldPanel.alpha = 1f;
        BoostPanel.alpha = 1;
    }
}
