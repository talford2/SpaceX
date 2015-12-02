using UnityEngine;

[System.Serializable]
public class UniversePosition
{
	public CellIndex CellIndex { get; set; }
	public Vector3 CellLocalPosition { get; set; }

	public UniversePosition(CellIndex cellIndex, Vector3 localPosition)
	{
		CellIndex = cellIndex;
		CellLocalPosition = localPosition;
	}
}
