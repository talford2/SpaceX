using UnityEngine;

public class DistantScaling : MonoBehaviour
{
    private Shiftable _shiftable;
    private float _thresholdDistance;
    private float _thresholdDistanceSquared;
    private float _distantScale;

    private int _distantLayer;
    private int _defaultLayer;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _thresholdDistance = 1000f;
        _distantScale = 1000f;

        _thresholdDistanceSquared = _thresholdDistance*_thresholdDistance;
        _distantLayer = LayerMask.NameToLayer("Distant");
        _defaultLayer = LayerMask.NameToLayer("Default");
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

        if (toCamera.sqrMagnitude > _thresholdDistanceSquared)
        {
            var distance = toCamera.magnitude;

            // This will only be able to scale objects of a radius up to the _thresholdDistance.
            // Keep the scaled object 1 unit in front of the camera!
            var scaledDistance = Mathf.Abs(distance - _thresholdDistance - 1f) / _distantScale + 1f; // + _thresholdDistance;

            var scale = Mathf.Clamp(scaledDistance / distance, 0f, 1f);
            transform.localScale = new Vector3(scale, scale, scale);

            // Positioning
                transform.position = FollowCamera.Current.transform.position + toCamera.normalized * scaledDistance;
                if (gameObject.layer != _distantLayer)
                    Utility.SetLayerRecursively(gameObject, _distantLayer);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            transform.position = worldDestination;
            if (gameObject.layer != _defaultLayer)
                Utility.SetLayerRecursively(gameObject, _defaultLayer);
        }
    }

    private Vector3 GetWorldPosition(CellIndex cellIndex, Vector3 positionInCell)
    {
        var cellDiff = cellIndex - FollowCamera.Current.Shiftable.UniverseCellIndex;
        return cellDiff.ToVector3()*Universe.Current.CellSize + positionInCell;
    }
}
