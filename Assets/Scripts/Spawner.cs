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

	private void Shifter_OnCellIndexChange(Shiftable shiftable, CellIndex delta)
	{
		//(Universe.Current.ViewPort.Shiftable.UniverseCellIndex - Shifter.UniverseCellIndex).
		//throw new System.NotImplementedException();
	}

	public void Spawn()
	{
		if (!HasSpawned)
		{
			_fighterInst = Instantiate<Fighter>(FighterPrefab);

			var universePosition = new UniversePosition(Shifter.UniverseCellIndex, Shifter.CellLocalPosition + transform.position);
			_fighterInst.SpawnVehicle(_fighterInst.VehiclePrefab, universePosition);

			HasSpawned = true;
		}
	}
}
