using UnityEngine;
using UnityEngine.UI;

public class UniverseTrackers : MonoBehaviour
{
    public Transform Target;

    private Camera _cam;

    private Vector3 screenCentre;
    private float boundaryPadding = 20f;
    private Rect screenBounds;

    private Image cursor;
    public Image ArrowCursor;
    public Image TrackerCurosr;

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
        var boundsPoint = r;
        if (boundsPoint.z < 0f)
            boundsPoint *= -1f;
        var inBounds = screenBounds.Contains(boundsPoint);

        if (inBounds)
        {
            TrackerCurosr.enabled = true;
            ArrowCursor.enabled = false;
            cursor = TrackerCurosr;
        }
        else
        {
            TrackerCurosr.enabled = false;
            ArrowCursor.enabled = true;
            cursor = ArrowCursor;
        }

        cursor.rectTransform.localPosition = Utility.GetBoundsIntersection(r, screenBounds);//, screenCentre);
        
        if (!inBounds)
        {
            cursor.rectTransform.localRotation = Quaternion.AngleAxis(GetScreenAngle(r - ArrowCursor.rectTransform.localPosition), Vector3.forward);
        }
        else
        {
            cursor.rectTransform.localRotation = Quaternion.identity;
        }
    }

    private float GetScreenAngle(Vector3 point)
    {
        if (point.z < 0f)
            point *= -1f;

        var delta = point - screenCentre;
        var angle = Mathf.Rad2Deg*Mathf.Atan2(delta.x, -delta.y) + 180f;
        return angle;
    }
}
