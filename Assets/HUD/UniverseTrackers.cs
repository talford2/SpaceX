using UnityEngine;
using UnityEngine.UI;

public class UniverseTrackers : MonoBehaviour
{
	public Transform Target;

	private Camera _cam;

    private Vector3 screenCentre;
    private float boundaryPadding = 20f;
    private Rect screenBounds;

	public Image Cursor;

    private void Start()
    {
        screenCentre = new Vector3(Screen.width/2f, Screen.height/2f, 0f);
        screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f*boundaryPadding, Screen.height - 2f*boundaryPadding);

        var ccc = Universe.Current.ViewPort;
        _cam = ccc.GetComponent<Camera>();
        ccc.OnMove += Ccc_OnMove;
    }

	private void Ccc_OnMove()
	{
		var r = _cam.WorldToScreenPoint(Target.position);
	    var v = ClampToScreen(r);

		Cursor.rectTransform.localPosition = v;
	}

    private Vector2 ClampToScreen(Vector3 pointAtPos)
    {
        if (pointAtPos.z < 0f)
            pointAtPos *= -1f;

        var delta = pointAtPos - screenCentre;
        var gradient = delta.y / delta.x;

        if (!screenBounds.Contains(pointAtPos))
        {
            /*
            var angle = Mathf.Atan2(delta.x, -delta.y);
            */

            Vector2 result = pointAtPos - screenCentre;

            if (result.x < screenBounds.xMin - screenCentre.x)
            {
                result.x = screenBounds.xMin - screenCentre.x;
                result.y = gradient*result.x;
            }
            if (result.x > screenBounds.xMax - screenCentre.x)
            {
                result.x = screenBounds.xMax - screenCentre.x;
                result.y = gradient*result.x;
            }
            if (result.y < screenBounds.yMin - screenCentre.y)
            {
                result.y = screenBounds.yMin - screenCentre.y;
                result.x = result.y/gradient;
            }
            if (result.y > screenBounds.yMax - screenCentre.y)
            {
                result.y = screenBounds.yMax - screenCentre.y;
                result.x = result.y/gradient;
            }

            result.x = Mathf.Clamp(result.x, screenBounds.xMin - screenCentre.x, screenBounds.xMax - screenCentre.x);
            result.y = Mathf.Clamp(result.y, screenBounds.yMin- screenCentre.y, screenBounds.yMax - screenCentre.y);
            return result;
        }
        return pointAtPos - screenCentre;
    }
}
