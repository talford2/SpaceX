using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector2 _move;
    private float _speed = 5f;

    private void Awake()
    {
    }

    private void Update()
    {
        _move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.position += new Vector3(_move.x, 0f, _move.y) * _speed * Time.deltaTime;
    }
}
