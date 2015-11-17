using UnityEngine;
using System.Collections;

public class UniverseTrackers : MonoBehaviour
{
	public Transform Target;

	private Camera _cam;

    private Vector3 screenCentre;
    private Rect screenBounds;

	public UnityEngine.UI.Image Cursor;

	// Use this for initialization
	void Start()
	{
        screenCentre = new Vector3(Screen.width/2f, Screen.height/2f, 0f);
	    var boundaryPadding = 10f;
        screenBounds = new Rect(0 + boundaryPadding, 0 + boundaryPadding, Screen.width - 2f * boundaryPadding, Screen.height - 2f * +boundaryPadding);

		var ccc = Universe.Current.ViewPort;
		_cam = ccc.GetComponent<Camera>();
		ccc.OnMove += Ccc_OnMove;
	}

    private Vector2 clampPos;

	private void Ccc_OnMove()
	{
		var r = _cam.WorldToScreenPoint(Target.position);

		//var v = new Vector3(r.x - screenCentre.x, r.y - screenCentre.y, 0);
		//var v = new Vector3(Mathf.Clamp(r.x - halfWidth, Screen.width / -2f, Screen.width / 2f), Mathf.Clamp(r.y - halfHeight, Screen.height / -2f, Screen.height / 2f), 0);

	    var v = ClampToScreen(r);
	    clampPos = v;

		Cursor.rectTransform.localPosition = v;
	}

    private Vector2 ClampToScreen(Vector3 pointAtPos)
    {
        if (pointAtPos.z < 0f)
            pointAtPos *= -1f;

        if (!screenBounds.Contains(pointAtPos))
        {
            var delta = pointAtPos - screenCentre;
            var angle = Mathf.Atan2(delta.x, -delta.y);

            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);

            var gradient = cos/sin;

            if (cos > 0f)
            {
                pointAtPos = new Vector2(screenCentre.y/gradient, screenCentre.y);
            }
            else
            {
                pointAtPos = new Vector2(-screenCentre.y/gradient, -screenCentre.y);
            }

            if (pointAtPos.x > screenCentre.x)
            {
                pointAtPos = new Vector2(screenCentre.x, screenCentre.x*gradient);
            }
            else if (pointAtPos.x < -screenCentre.x)
            {
                pointAtPos = new Vector2(-screenCentre.x, -screenCentre.x*gradient);
            }

            return new Vector2(
                Mathf.Clamp(pointAtPos.x , screenBounds.x - screenCentre.x, screenBounds.width - screenCentre.x),
                Mathf.Clamp(Screen.height - pointAtPos.y, screenBounds.y - screenCentre.y, screenBounds.height - screenCentre.y)
                );
        }
        return pointAtPos - screenCentre;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20f, 50f, 100f, 25f), clampPos.ToString());
    }
}
