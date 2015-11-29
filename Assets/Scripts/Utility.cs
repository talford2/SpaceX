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
        if (parent!=null)
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
        var anchor = new Vector2(bounds.xMin + (bounds.xMax - bounds.xMin)/2f, bounds.yMin + (bounds.yMax - bounds.yMin)/2f);

        var delta = point - anchor;
        var gradient = delta.y/delta.x;

        if (!bounds.Contains(point))
        {
            var result = point - anchor;

            if (result.x < bounds.xMin - anchor.x)
            {
                result.x = bounds.xMin - anchor.x;
                result.y = gradient*result.x;
            }
            if (result.x > bounds.xMax - anchor.x)
            {
                result.x = bounds.xMax - anchor.x;
                result.y = gradient*result.x;
            }
            if (result.y < bounds.yMin - anchor.y)
            {
                result.y = bounds.yMin - anchor.y;
                result.x = result.y/gradient;
            }
            if (result.y > bounds.yMax - anchor.y)
            {
                result.y = bounds.yMax - anchor.y;
                result.x = result.y/gradient;
            }

            result.x = Mathf.Clamp(result.x, bounds.xMin - anchor.x, bounds.xMax - anchor.x);
            result.y = Mathf.Clamp(result.y, bounds.yMin - anchor.y, bounds.yMax - anchor.y);
            return result;
        }
        return point - anchor;
    }

    public static Texture2D ColouredTexture(int width, int height, Color color)
    {
        var texture = new Texture2D(width, height);
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                texture.SetPixel(i,j,color);
            }
        }
        texture.Apply();
        return texture;
    }
}
