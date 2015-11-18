using UnityEngine;
using System.Collections;

public class UniversePin : MonoBehaviour
{
	public Transform Target;

	private Canvas _canvas;

	void Awake()
	{
		_canvas = GetComponent<Canvas>();
	}

	void Update()
	{
		if (Target != null)
		{
			_canvas.transform.position = Target.position;
		}
		_canvas.transform.LookAt(Universe.Current.ViewPort.transform);
	}
}
