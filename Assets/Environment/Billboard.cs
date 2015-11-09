using UnityEngine;

public class Billboard : MonoBehaviour
{
	public Camera UseCamera;
	public bool Flip;

	public bool UseCameraRotation;

	void Start()
	{
		if (UseCamera == null)
			UseCamera = Camera.main;

		if (Flip)
		{
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z * -1);
		}
	}

	void Update()
	{
		if (UseCameraRotation)
		{
			transform.rotation = UseCamera.transform.rotation;
			transform.Rotate(180, 0, 180, Space.Self);
		}
		else
		{
			transform.LookAt(UseCamera.transform.position, transform.up);
			//transform.rotation = Quaternion.LookRotation(Camera.main.transform.up, forward);
		}
	}
}
