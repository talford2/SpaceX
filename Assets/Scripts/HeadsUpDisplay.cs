using UnityEngine;
using UnityEngine.UI;

public class HeadsUpDisplay : MonoBehaviour
{
    public GameObject Crosshair;
    public Text EnergyText;
    public Text HealthText;

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
    }

    public void ShowCrosshair()
    {
        Crosshair.SetActive(true);
    }

    public void HideCrosshair()
    {
        Crosshair.SetActive(false);
    }
}
