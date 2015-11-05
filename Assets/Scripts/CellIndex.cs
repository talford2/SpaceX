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

	public override string ToString()
	{
		return string.Format("({0},{1},{2})", X, Y, Z);
	}

	public Vector3 ToVector3()
	{
		return new Vector3(X, Y, Z);
	}

	public static CellIndex operator +(CellIndex c1, CellIndex c2)
	{
		return new CellIndex(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
	}

	public static CellIndex operator -(CellIndex c1, CellIndex c2)
	{
		return new CellIndex(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
	}

    public static CellIndex operator *(float s, CellIndex c1)
    {
        return new CellIndex(Mathf.CeilToInt(s * c1.X), Mathf.CeilToInt(s * c1.Y), Mathf.CeilToInt(s * c1.Z));
    }
}
