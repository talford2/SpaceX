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

	public Image HitImage;

	private float squadronPromptCooldown;

	private static HeadsUpDisplay _current;

	private float _hitCooldown = 0;

	public static HeadsUpDisplay Current { get { return _current; } }

	private void Awake()
	{
		_current = this;
		HitImage.color = new Color(1, 1, 1, 0);
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
		
		HitImage.color = new Color(1, 1, 1, _hitCooldown);
		_hitCooldown -= Time.deltaTime;
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
