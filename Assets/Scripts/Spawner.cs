using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	public Fighter FighterPrefab;

	private Fighter _fighterInst;

	public Shiftable Shifter;

	public bool SpawnOnAwake = false;

	private bool HasSpawned = false;

	private void Awake()
	{
		Shifter.OnCellIndexChange += Shifter_OnCellIndexChange;

		if (SpawnOnAwake)
		{
			Spawn();
		}
	}

	private void Shifter_OnCellIndexChange(CellIndex delta)
	{
		//(Universe.Current.ViewPort.Shiftable.UniverseCellIndex - Shifter.UniverseCellIndex).
		//throw new System.NotImplementedException();
	}

	public void Spawn()
	{
		if (!HasSpawned)
		{
			_fighterInst = Instantiate<Fighter>(FighterPrefab);

			//_fighterInst.SpawnVehicle(_fighterInst.VehiclePrefab);


			_fighterInst.VehicleInstance.Shiftable.SetShiftPosition(new UniversePosition(Shifter.UniverseCellIndex, Shifter.CellLocalPosition + transform.position));

			HasSpawned = true;
		}
	}
}
