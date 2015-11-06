using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
	public GameObject Missile;

	public float FireRate = 0.2f;

	private float _fireCooldown = 0f;

	public bool IsTriggered;

	void Update()
	{
		_fireCooldown -= Time.deltaTime;

		//Debug.LogFormat("{0} ===", IsTriggered);

		if (IsTriggered && _fireCooldown < 0)
		{
			//Debug.Log("Shoot!");
			_fireCooldown = FireRate;
			Fire();
		}
	}

	public void Fire()
	{
		//Debug.Log("Fire!");
		var missile = Instantiate(Missile);
		missile.transform.position = transform.position;
		missile.transform.rotation = transform.rotation;
		missile.GetComponent<Shiftable>().UniverseCellIndex = PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex;

		// Squirt
		var lr = missile.GetComponent<LineRenderer>();

		//lr.SetPosition(0, transform.position);
		//lr.SetPosition(1, transform.position + transform.forward * 5);

		Debug.LogFormat(missile.transform.position.ToString());
	}
}
