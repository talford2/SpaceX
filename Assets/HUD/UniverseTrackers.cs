using UnityEngine;
using System.Collections;

public class UniverseTrackers : MonoBehaviour
{
	public Transform Target;

	private Camera _cam;

    private Vector3 screenCentre;
    private Rect screenBounds;
    private float gradient;

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

        var delta = pointAtPos - screenCentre;
        gradient = delta.y / delta.x;

        if (!screenBounds.Contains(pointAtPos))
        {
            /*
            var angle = Mathf.Atan2(delta.x, -delta.y);

            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);
            */

            Vector2 result = pointAtPos - screenCentre;

            if (result.x < screenBounds.x - screenCentre.x)
            {
                result.x = screenBounds.x - screenCentre.x;
                result.y = gradient*result.x;
            }
            if (result.x > screenBounds.width - screenCentre.x)
            {
                result.x = screenBounds.width - screenCentre.x;
                result.y = gradient*result.x;
            }
            if (result.y < screenBounds.y - screenCentre.y)
            {
                result.y = screenBounds.y - screenCentre.y;
                result.x = result.y/gradient;
            }
            if (result.y > screenBounds.height - screenCentre.y)
            {
                result.y = screenBounds.height - screenCentre.y;
                result.x = result.y/gradient;
            }

            result.x = Mathf.Clamp(result.x, screenBounds.x - screenCentre.x, screenBounds.width - screenCentre.x);
            result.y = Mathf.Clamp(result.y, screenBounds.y - screenCentre.y, screenBounds.height - screenCentre.y);
            return result;
        }
        return pointAtPos - screenCentre;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20f, 50f, 100f, 25f), clampPos.ToString());
        GUI.Label(new Rect(20f, 80f, 100f, 25f), string.Format("{0:f2}", gradient));
    }
}
