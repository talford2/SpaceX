using UnityEngine;
using System.Collections;

public class Nebula : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		transform.position += Vector3.forward * Time.deltaTime * 5f;
	}
}
