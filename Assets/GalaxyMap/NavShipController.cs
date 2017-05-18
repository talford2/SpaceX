using UnityEngine;

public class NavShipController : MonoBehaviour
{
    private Vector3 _destination;
    private float _acceleration = 10f;
    private float _speed;
    private float _maxSpeed = 10f;

    private void Awake()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            var navPlane = new Plane(Vector3.up, Vector3.zero);
            float dist;
            if (navPlane.Raycast(mouseRay, out dist))
            {
                _destination = mouseRay.GetPoint(dist);
            }
        }


        var toDestination = transform.position - _destination;

        if (toDestination.sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, _destination, 2f * Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, -toDestination.normalized, _maxSpeed * Time.deltaTime);
            _speed = 0f;
        }
    }
}
