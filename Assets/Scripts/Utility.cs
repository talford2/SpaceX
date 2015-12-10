using UnityEngine;

public class Utility
{
	public static Transform FindOrCreateContainer(string name)
	{
		var existing = GameObject.Find(name);
		if (existing != null)
		{
			if (existing.transform.parent == null)
			{
				return existing.transform;
			}
		}
		return new GameObject(name).transform;
	}

	public static GameObject InstantiateInParent(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent, int layer)
	{
		var instance = (GameObject)Object.Instantiate(gameObject, position, rotation);
		instance.transform.SetParent(parent);
		instance.layer = layer;
		return instance;
	}

	public static GameObject InstantiateInParent(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent)
	{
		if (parent != null)
			return InstantiateInParent(gameObject, position, rotation, parent, parent.gameObject.layer);
		var instance = (GameObject)Object.Instantiate(gameObject, position, rotation);
		instance.transform.SetParent(null);
		return instance;
	}

	public static GameObject InstantiateInParent(GameObject gameObject, Transform parent)
	{
		return InstantiateInParent(gameObject, parent.position, parent.rotation, parent, parent.gameObject.layer);
	}

	public static void SetLayerRecursively(GameObject obj, int layer)
	{
		obj.layer = layer;
		foreach (Transform trans in obj.transform)
		{
			SetLayerRecursively(trans.gameObject, layer);
		}
	}

	public static void MoveParticles(ParticleSystem ps, Vector3 moveAmount)
	{
		if (ps != null)
		{
			var parr = new ParticleSystem.Particle[ps.maxParticles];
			var totalParticles = ps.GetParticles(parr);

			//Debug.LogFormat("{0}: {1} / {2}", ps.name, totalParticles, parr.Length);
			//Debug.Break();

			for (var i = 0; i < totalParticles; i++)
			{
				parr[i].position += moveAmount;
			}
			ps.SetParticles(parr, parr.Length);
		}
	}

	public static Vector2 GetBoundsIntersection(Vector2 point, Rect bounds)
	{
		var anchor = new Vector2(bounds.xMin + (bounds.xMax - bounds.xMin) / 2f, bounds.yMin + (bounds.yMax - bounds.yMin) / 2f);

		var delta = point - anchor;
		var gradient = delta.y / delta.x;

		if (!bounds.Contains(point))
		{
			var result = point - anchor;

			if (result.x < bounds.xMin - anchor.x)
			{
				result.x = bounds.xMin - anchor.x;
				result.y = gradient * result.x;
			}
			if (result.x > bounds.xMax - anchor.x)
			{
				result.x = bounds.xMax - anchor.x;
				result.y = gradient * result.x;
			}
			if (result.y < bounds.yMin - anchor.y)
			{
				result.y = bounds.yMin - anchor.y;
				result.x = result.y / gradient;
			}
			if (result.y > bounds.yMax - anchor.y)
			{
				result.y = bounds.yMax - anchor.y;
				result.x = result.y / gradient;
			}

			result.x = Mathf.Clamp(result.x, bounds.xMin - anchor.x, bounds.xMax - anchor.x);
			result.y = Mathf.Clamp(result.y, bounds.yMin - anchor.y, bounds.yMax - anchor.y);
			return result;
		}
		return point - anchor;
	}

	public static Color GetRandomColor(float? setAlpha = null)
	{
		if (setAlpha.HasValue)
		{
			return Random.ColorHSV(0, 1, 0, 1, 0, 1, setAlpha.Value, setAlpha.Value);
		}
		return Random.ColorHSV();
	}

	public static float WrapFloat(float a, float b)
	{
		float result;
		float large;
		float small;

		if (a > b)
		{
			large = a;
			small = b;
		}
		else
		{
			large = b;
			small = a;
		}

		if (large - small > 0.5f)
		{
			result = large + (small - large + 1f) * Random.Range(0f, 1f);
		}
		else
		{
			result = small + (large - small) * Random.Range(0f, 1f);
		}
		return result % 1f;
	}

	public static Color GetRandomColor(Color c1, Color c2, float? alpha = null)
	{
		// This isn't perfect because it will tend towards the middle of the colour
		// wheel, need to make hues wrap

		var c1Hsv = HSVColor.FromColor(c1);
		var c2Hsv = HSVColor.FromColor(c2);

		float hue = WrapFloat(c1Hsv.H, c2Hsv.H);

		if (alpha.HasValue)
		{
			return Random.ColorHSV(
			hue,
			hue,
			Mathf.Min(c1Hsv.S, c2Hsv.S),
			Mathf.Max(c1Hsv.S, c2Hsv.S),
			Mathf.Min(c1Hsv.V, c2Hsv.V),
			Mathf.Max(c1Hsv.V, c2Hsv.V),
			alpha.Value,
			alpha.Value);
		}

		return Random.ColorHSV(
			hue,
			hue,
			Mathf.Min(c1Hsv.S, c2Hsv.S),
			Mathf.Max(c1Hsv.S, c2Hsv.S),
			Mathf.Min(c1Hsv.V, c2Hsv.V),
			Mathf.Max(c1Hsv.V, c2Hsv.V),
			Mathf.Min(c1.a, c2.a),
			Mathf.Max(c1.a, c2.a)
		);
	}

	public static Color GetRandomColor(Color c1, Color c2, float alpha, float saturation)
	{
		var c1Hsv = HSVColor.FromColor(c1);
		var c2Hsv = HSVColor.FromColor(c2);

		float hue = WrapFloat(c1Hsv.H, c2Hsv.H);

		return Random.ColorHSV(
		hue,
		hue,
		saturation,
		saturation,
		Mathf.Min(c1Hsv.V, c2Hsv.V),
		Mathf.Max(c1Hsv.V, c2Hsv.V),
		alpha,
		alpha);
	}

	public static Texture2D ColouredTexture(int width, int height, Color color)
	{
		var texture = new Texture2D(width, height);
		for (var i = 0; i < width; i++)
		{
			for (var j = 0; j < height; j++)
			{
				texture.SetPixel(i, j, color);
			}
		}
		texture.Apply();
		return texture;
	}

	public static Vector3 GetRandomDirection(Vector3 direction, float angle)
	{
		return Quaternion.Euler(Random.Range(-angle, angle), Random.Range(-angle, angle), Random.Range(-angle, angle)) * -direction;
	}

	public static Vector3 RandomInsideCube
	{
		get
		{
			return new Vector3(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1));
		}
	}

    public static Vector3 GetVehicleExtrapolatedPosition(Vehicle vehicle, Weapon shootWeapon, float timeError)
    {
        var distance = (vehicle.transform.position - shootWeapon.GetShootPointCentre()).magnitude;
        var timeToHit = distance/shootWeapon.MissilePrefab.GetComponent<Missile>().MissileSpeed;
        return vehicle.transform.position + vehicle.GetVelocity()*(timeToHit + timeError);
    }
}

public class HSVColor
{
	#region Public Properties

	public float H { get; set; }

	public float S { get; set; }

	public float V { get; set; }

	#endregion

	#region Constructors

	public HSVColor() { }

	public HSVColor(float h, float s, float v)
	{
		H = h;
		S = s;
		V = v;
	}

	public HSVColor(Color c)
	{
		var h = 0f;
		var s = 0f;
		var v = 0f;

		Color.RGBToHSV(c, out h, out s, out v);

		H = h;
		S = s;
		V = v;
	}

	#endregion

	public Color GetColor()
	{
		return Color.HSVToRGB(H, S, V);
	}

	public static HSVColor FromColor(Color c)
	{
		return new HSVColor(c);
	}
}
