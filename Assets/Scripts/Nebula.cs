using UnityEngine;

public class Nebula : MonoBehaviour
{	
	void Update()
	{
		transform.position += Vector3.forward * Time.deltaTime * 5f;
	}
}
