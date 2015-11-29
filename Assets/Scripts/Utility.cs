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

	public static Color GetRandomColor(Color c1, Color c2, float? alpha = null)
	{
		var c1Hsv = HSVColor.FromColor(c1);
		var c2Hsv = HSVColor.FromColor(c2);

		if (alpha.HasValue)
		{
			return Random.ColorHSV(
			Mathf.Min(c1Hsv.H, c2Hsv.H),
			Mathf.Max(c1Hsv.H, c2Hsv.H),
			Mathf.Min(c1Hsv.S, c2Hsv.S),
			Mathf.Max(c1Hsv.S, c2Hsv.S),
			Mathf.Min(c1Hsv.V, c2Hsv.V),
			Mathf.Max(c1Hsv.V, c2Hsv.V),
			alpha.Value,
			alpha.Value
		);
		}
		return Random.ColorHSV(
			Mathf.Min(c1Hsv.H, c2Hsv.H),
			Mathf.Max(c1Hsv.H, c2Hsv.H),
			Mathf.Min(c1Hsv.S, c2Hsv.S),
			Mathf.Max(c1Hsv.S, c2Hsv.S),
			Mathf.Min(c1Hsv.V, c2Hsv.V),
			Mathf.Max(c1Hsv.V, c2Hsv.V),
			Mathf.Min(c1.a, c2.a),
			Mathf.Max(c1.a, c2.a)
		);
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
}

public class HSVColor
{
	public float H { get; set; }

	public float S { get; set; }

	public float V { get; set; }

	public HSVColor() { }

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

	public static HSVColor FromColor(Color c)
	{
		return new HSVColor(c);
	}
}
