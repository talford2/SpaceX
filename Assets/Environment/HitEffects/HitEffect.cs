using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour
{
	public GameObject HitEffectPrefab;

	public void Hit(HitEffectParameters hitTransform)
	{
		var instance = Instantiate(HitEffectPrefab);
		instance.transform.position = hitTransform.Position;
		instance.transform.forward = hitTransform.Normal;
	}
}

public class HitEffectParameters
{
	public Vector3 Position;

	public Vector3 Normal;
}