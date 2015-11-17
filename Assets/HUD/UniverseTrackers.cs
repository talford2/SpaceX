using UnityEngine;
using System.Collections;

public class UniverseTrackers : MonoBehaviour
{
	public Transform Target;

	private Camera _cam;

	public UnityEngine.UI.Image Cursor;

	// Use this for initialization
	void Start()
	{
		var ccc = Universe.Current.ViewPort;
		_cam = ccc.GetComponent<Camera>();
		ccc.OnMove += Ccc_OnMove;
	}

	private void Ccc_OnMove()
	{
		//throw new System.NotImplementedException();
		var r = _cam.WorldToScreenPoint(Target.position);

		var halfWidth = Screen.width / 2f;
		var halfHeight = Screen.height / 2f;

		var v = new Vector3(r.x - halfWidth, r.y - halfHeight, 0);

		Cursor.rectTransform.localPosition = v;
	}

	// Update is called once per frame
	void LateUpdate()
	{
	}
}
