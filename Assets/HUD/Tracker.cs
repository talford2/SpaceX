using UnityEngine;
using UnityEngine.UI;

public abstract class Tracker : MonoBehaviour
{
    public virtual void Start()
    {
        TrackerManager.Current.AddTracker(this);
    }

    public abstract Image CreateInstance();

    public abstract void UpdateInstance();

    public abstract void DestroyInstance();

    public abstract void SetVisible(bool value);

    public virtual void OnDestroy()
    {
        TrackerManager.Current.RemoveTracker(this);
        DestroyInstance();
    }
	
    public static Vector2 GetBoundsIntersection(Vector2 point, Rect bounds)
    {
        var anchor = new Vector2(bounds.xMin + (bounds.xMax - bounds.xMin) / 2f, bounds.yMin + (bounds.yMax - bounds.yMin) / 2f);

        var delta = point - anchor;
        var gradient = delta.y / delta.x;

        if (!bounds.Contains(point))
        {
            var result = point - anchor;

            if (result.x < bounds.xMin - anchor.x)
            {
                result.x = bounds.xMin - anchor.x;
                result.y = gradient * result.x;
            }
            if (result.x > bounds.xMax - anchor.x)
            {
                result.x = bounds.xMax - anchor.x;
                result.y = gradient * result.x;
            }
            if (result.y < bounds.yMin - anchor.y)
            {
                result.y = bounds.yMin - anchor.y;
                result.x = result.y / gradient;
            }
            if (result.y > bounds.yMax - anchor.y)
            {
                result.y = bounds.yMax - anchor.y;
                result.x = result.y / gradient;
            }

            result.x = Mathf.Clamp(result.x, bounds.xMin - anchor.x, bounds.xMax - anchor.x);
            result.y = Mathf.Clamp(result.y, bounds.yMin - anchor.y, bounds.yMax - anchor.y);
            return result;
        }
        return point - anchor;
    }
}
