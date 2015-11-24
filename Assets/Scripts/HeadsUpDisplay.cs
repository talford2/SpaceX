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

    private float squadronPromptCooldown;

    private static HeadsUpDisplay _current;

    public static HeadsUpDisplay Current { get { return _current; } }

    private void Awake()
    {
        _current = this;
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
        squadronPromptCooldown = SquadronPromptTime;
    }

    public void HideSquadronPrompt()
    {
        SquadronPrompt.SetActive(false);
    }
}
