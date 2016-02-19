using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipProfileScreen : MonoBehaviour
{
    //public Vehicle VehiclePrefab;
    public List<Vehicle> Vehicles;

    public Text ShipNameValue;
    public Text PowerValue;
    public Text ShieldValue;
    public Text HullValue;

    public Text PrimaryWeaponText;
    public Text SecondaryWeaponText;

    private int _curIndex;
    private GameObject _preview;

    private void Start()
    {
        _curIndex = 0;
        Populate(Vehicles[_curIndex]);
    }

    private void Update()
    {
        if (Input.GetButtonUp("SquadronNext"))
        {
            _curIndex++;
            if (_curIndex > Vehicles.Count-1)
                _curIndex = 0;
            Populate(Vehicles[_curIndex]);
        }

        if (Input.GetButtonUp("SquadronPrevious"))
        {
            _curIndex--;
            if (_curIndex < 0)
                _curIndex = Vehicles.Count - 1;
            Populate(Vehicles[_curIndex]);
        }

        if (_preview != null)
        {
            _preview.transform.Rotate(Vector3.up, 15f*Time.deltaTime);
        }
    }

    public void Populate(Vehicle vehicle)
    {
        if (_preview != null)
            Destroy(_preview);
        _preview = (GameObject) Instantiate(vehicle.PreviewPrefab, new Vector3(0, 0.5f, 1f), Quaternion.Euler(25f, 200f, 10f));

        ShipNameValue.text = vehicle.Name;
        PowerValue.text = string.Format("{0:f0}", vehicle.Power);

        var vehicleKillable = vehicle.GetComponent<Killable>();
        ShieldValue.text = string.Format("{0:f0}", vehicleKillable.MaxShield);
        HullValue.text = string.Format("{0:f0}", vehicleKillable.MaxHealth);

        PrimaryWeaponText.text = vehicle.PrimaryWeaponPrefab.Name;
        SecondaryWeaponText.text = vehicle.SecondaryWeaponPrefab.Name;
    }
}
