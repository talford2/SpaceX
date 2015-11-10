using UnityEngine;

public class DistantScaling : MonoBehaviour
{
    private Shiftable _shiftable;
    private float _maxDistance;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _maxDistance = 1000f;
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
        var scale = Mathf.Clamp(_maxDistance/distance, 0f, 1f);
        transform.localScale = new Vector3(scale, scale, scale);

        // Positioning
        if (distance > _maxDistance)
        {
            transform.position = FollowCamera.Current.transform.position + toCamera.normalized*_maxDistance;
        }
        else
        {
            transform.position = worldDestination;
        }
    }

    private Vector3 GetWorldPosition(CellIndex cellIndex, Vector3 positionInCell)
    {
        var cellDiff = cellIndex - FollowCamera.Current.Shiftable.UniverseCellIndex;
        return cellDiff.ToVector3() * Universe.Current.CellSize + positionInCell;
    }
}
