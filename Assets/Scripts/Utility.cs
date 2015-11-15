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
		return InstantiateInParent(gameObject, position, rotation, parent, parent.gameObject.layer);
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
			var parr = new ParticleSystem.Particle[100];
			var totalParticles = ps.GetParticles(parr);
			//Debug.LogFormat("{0}: {1} / {2}", ps.name, totalParticles, parr.Length);
			for (var i = 0; i < totalParticles; i++)
			{
				parr[i].position += moveAmount;
			}
			ps.SetParticles(parr, parr.Length);
		}
	}
}
