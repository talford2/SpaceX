using UnityEngine;
using System.Collections;

public class DoubleVector3
{
	public double X { get; set; }

	public double Y { get; set; }

	public double Z { get; set; }

	public DoubleVector3(double x, double y, double z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public DoubleVector3 Clone()
	{
		return new DoubleVector3(X, Y, Z);
	}
}
