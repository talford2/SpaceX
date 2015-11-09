using UnityEngine;

public class DistantScaling : MonoBehaviour
{
    public Vector3 UniversePosition;

    private Shiftable _shiftable;
    private float _maxDistance;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _shiftable.OnShift += Shift;
        _maxDistance = 1000f;
    }

    private void Update()
    {
        UpdatePositionAndScale();
    }

    private void UpdatePositionAndScale()
    {
        // Scaling
        var toCamera = UniversePosition - GetUniversePosition(FollowCamera.Current.Shiftable.UniverseCellIndex, FollowCamera.Current.Shiftable.CellLocalPosition);
        var distance = toCamera.magnitude;
        var scale = Mathf.Clamp(_maxDistance / distance, 0f, 1f);
        transform.localScale = new Vector3(scale, scale, scale);

        // Positioning
        if (distance > _maxDistance)
        {
            transform.position = FollowCamera.Current.transform.position + toCamera.normalized*_maxDistance;
        }
        else
        {
            // Fix tiny offset here.
        }
    }

    private Vector3 GetUniversePosition(CellIndex cellIndex, Vector3 positionInCell)
    {
        return cellIndex.ToVector3()*Universe.Current.CellSize + positionInCell;// + Vector3.one*Universe.Current.HalfCellSize;
    }
    
    private void Shift(Vector3 delta)
    {
        /*
        var toCamera = UniversePosition - GetUniversePosition(FollowCamera.Current.Shiftable.UniverseCellIndex, FollowCamera.Current.transform.position);
        var distance = toCamera.magnitude;
        var scale = Mathf.Clamp(_maxDistance / distance, 0f, 1f);
        transform.localScale = new Vector3(scale, scale, scale);
        if (distance > _maxDistance)
        {
            transform.position = FollowCamera.Current.transform.position + toCamera.normalized * _maxDistance;
        }
        */
        UpdatePositionAndScale();
    }
}
