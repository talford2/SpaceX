using System;
using UnityEngine;

[Serializable]
public class CellIndex
{
	public int X;

	public int Y;

	public int Z;

	public CellIndex() { }

	public CellIndex(Vector3 vec)
	{
		X = (int)vec.x;
		Y = (int)vec.y;
		Z = (int)vec.z;
	}

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

	public bool IsEqualTo(CellIndex other)
	{
		return X == other.X && Y == other.Y && Z == other.Z;
	}

	public bool IsZero()
	{
		return X == 0 && Y == 0 && Z == 0;
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

    public static CellIndex operator *(CellIndex c1, float s)
    {
        return new CellIndex(Mathf.CeilToInt(s * c1.X), Mathf.CeilToInt(s * c1.Y), Mathf.CeilToInt(s * c1.Z));
    }

    public static CellIndex operator -(CellIndex c1)
    {
        return new CellIndex(-c1.X, -c1.Y, -c1.Z);
    }
}
