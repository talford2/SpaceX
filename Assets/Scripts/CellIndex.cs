using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CellIndex
{
	public int X;

	public int Y;

	public int Z;

	public CellIndex() { }

	public CellIndex(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}
}
