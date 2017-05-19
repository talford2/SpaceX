using UnityEngine;
using UnityEngine.SceneManagement;

public class NavShipController : MonoBehaviour
{
    public static NavShipController Current { get { return _current; } }
    private static NavShipController _current;

    private Vector3 _destination;
    private float _acceleration = 30f;
    private float _speed;
    private float _maxSpeed = 30f;
    private int _levelIndex;

    private void Awake()
    {
        _current = this;
        _destination = transform.position;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            /*
            var navPlane = new Plane(Vector3.up, Vector3.zero);
            float dist;
            if (navPlane.Raycast(mouseRay, out dist))
            {
                _speed = 0f;
                SetDestination(mouseRay.GetPoint(dist));
            }
            */
            RaycastHit mouseHit;
            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                var navIcon = mouseHit.collider.GetComponentInParent<NavIcon>();
                if (navIcon != null)
                {
                    _levelIndex = navIcon.LevelIndex;
                    Debug.Log("INDEX: " + _levelIndex);
                    navIcon.OnClick();
                }
            }
        }

        var toDestination = transform.position - _destination;
        var distToStop = (_speed * _speed) / (2f * _acceleration);
        var dotDest = Vector3.Dot(transform.forward, -toDestination.normalized);
        if (dotDest > 0.8f)
        {
            if (toDestination.sqrMagnitude > 1.2f*distToStop * distToStop)
            {
                _speed = Mathf.Clamp(_speed + _acceleration * Time.deltaTime, 0f, _maxSpeed);
            }
            else
            {
                _speed = Mathf.Clamp(_speed - _acceleration * Time.deltaTime, 0f, _maxSpeed);
            }
        }
        if (toDestination.sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination, _speed * Time.deltaTime);
            //Vector3.Lerp(transform.position, _destination, 2f * Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, -toDestination.normalized, 10f * Time.deltaTime);
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                PlayerContext.Current.LevelIndex = _levelIndex;
                SceneManager.LoadScene("Test1");
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }
}
