using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector2 _move;
    private float _speed = 5f;

    private bool _zoomTriggered;
    private Vector3 _zoomTo;
    private float _zoomTime = 2f;
    private float _zoomCooldown;

    private void Awake()
    {
    }

    private void Update()
    {
        if (_zoomTriggered)
        {
            if (_zoomCooldown >= 0f)
            {
                _zoomCooldown -= Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, _zoomTo, 1f - (_zoomCooldown / _zoomTime));

                if (_zoomCooldown < 0f)
                {
                    transform.position = _zoomTo;
                }
            }
        }
        else
        {
            _move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            transform.position += new Vector3(_move.x, 0f, _move.y) * _speed * Time.deltaTime;
        }
    }

    public void TriggerZoom(Vector3 toPosition)
    {
        _zoomTo = toPosition;
        _zoomCooldown = _zoomTime;
        _zoomTriggered = true;
    }
}
