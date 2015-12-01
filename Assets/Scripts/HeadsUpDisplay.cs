using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadsUpDisplay : MonoBehaviour
{
	public GameObject Crosshair;
	public Text EnergyText;
	public Text HealthText;

	public GameObject SquadronPrompt;
	public Text SquadronNameText;
	public float SquadronPromptTime = 1.5f;
    public GameObject SquadronIconContainer;
    public Image SquadronIcon;

	public Image HitImage;

	private float squadronPromptCooldown;

	private static HeadsUpDisplay _current;

	public float HitFadeSpeed = 0.5f;

	private float _hitCooldown = 0;

    private List<SquadronIcon> squadronIcons;

	public static HeadsUpDisplay Current { get { return _current; } }

	private void Awake()
	{
		_current = this;
		HitImage.color = new Color(1, 1, 1, 0);
        squadronIcons = new List<SquadronIcon>();
	}

	private void Update()
	{
		if (PlayerController.Current.VehicleInstance != null)
		{
			EnergyText.text = string.Format("{0:f0}", PlayerController.Current.VehicleInstance.BoostEnergy);
			HealthText.text = string.Format("{0:f0}", PlayerController.Current.VehicleInstance.GetComponent<Killable>().Health);
		}
		if (squadronPromptCooldown >= 0f)
		{
			squadronPromptCooldown -= Time.deltaTime;
			if (squadronPromptCooldown < 0f)
			{
				HideSquadronPrompt();
			}
		}

	    while (squadronIcons.Count < PlayerController.Current.Squadron.Count)
	    {
            var icon = Instantiate(SquadronIcon);
            icon.transform.SetParent(SquadronIconContainer.transform);
	        icon.rectTransform.localScale = Vector3.one;
            squadronIcons.Add(icon.GetComponent<SquadronIcon>());
	    }

	    for (var i = 0; i < PlayerController.Current.Squadron.Count; i++)
	    {
	        var squadronVehicle = !PlayerController.Current.Squadron[i].enabled
                ? PlayerController.Current.VehicleInstance
                : PlayerController.Current.Squadron[i].VehicleInstance;
	        if (squadronVehicle != null)
	        {
	            squadronIcons[i].SetSelected(PlayerController.Current.GetSquadronSelectedIndex() == i);
	            squadronIcons[i].SetHealthFraction(squadronVehicle.GetComponent<Killable>().Health/squadronVehicle.GetComponent<Killable>().MaxHealth);
	        }
	        else
	        {
	            squadronIcons[i].SetHealthFraction(0f);
	        }
	    }

		HitImage.color = new Color(1, 1, 1, _hitCooldown);
		_hitCooldown -= Time.deltaTime * HitFadeSpeed;
		_hitCooldown = Mathf.Max(0, _hitCooldown);
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
		//SquadronPrompt.SetActive(true);
		SquadronNameText.text = message;
		squadronPromptCooldown = SquadronPromptTime;
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
