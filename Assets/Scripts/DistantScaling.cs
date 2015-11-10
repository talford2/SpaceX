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
        var cellDifference = _shiftable.UniverseCellIndex - FollowCamera.Current.Shiftable.UniverseCellIndex;
        var universeDestination = GetUniversePosition(cellDifference, _shiftable.CellLocalPosition);
        var toCamera = universeDestination - FollowCamera.Current.transform.position;

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
            // Fix tiny offset here.
            transform.position = universeDestination;
        }
    }

    private Vector3 GetUniversePosition(CellIndex cellIndex, Vector3 positionInCell)
    {
        return cellIndex.ToVector3()*Universe.Current.CellSize + positionInCell;// + Vector3.one*Universe.Current.HalfCellSize;
    }
}
