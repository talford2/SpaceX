using UnityEngine;

public class DistantScaling : MonoBehaviour
{
    private Shiftable _shiftable;
    private float _thresholdDistance;
    private float _distantScale;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _thresholdDistance = 1000f;
        _distantScale = 1000f;
    }

    private void Start()
    {
        FollowCamera.Current.OnMove += UpdatePositionAndScale;
    }

    private void UpdatePositionAndScale()
    {
        // Scaling
        var worldDestination = GetWorldPosition(_shiftable.UniverseCellIndex, _shiftable.CellLocalPosition);
        var toCamera = worldDestination - FollowCamera.Current.transform.position;

        var distance = toCamera.magnitude;

        var scaledDistance = (distance - _thresholdDistance)/_distantScale; // + _thresholdDistance;

        var scale = Mathf.Clamp(scaledDistance/distance, 0f, 1f);
        transform.localScale = new Vector3(scale, scale, scale);

        // Positioning
        if (distance > _thresholdDistance)
        {
            transform.position = FollowCamera.Current.transform.position + toCamera.normalized*scaledDistance;
        }
        else
        {
            transform.position = worldDestination;
        }
    }

    private Vector3 GetWorldPosition(CellIndex cellIndex, Vector3 positionInCell)
    {
        var cellDiff = cellIndex - FollowCamera.Current.Shiftable.UniverseCellIndex;
        return cellDiff.ToVector3()*Universe.Current.CellSize + positionInCell;
    }
}
