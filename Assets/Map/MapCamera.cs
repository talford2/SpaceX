using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MapCamera : MonoBehaviour
{
    public float ZoomSensitivity = 2000f;
    public float MinCameraDistance;
    public float MaxCameraDistance;
    public float RotateSpeed = 3f;
    public float PanSpeed = 0.1f;

    public delegate void OnCameraMove();
    public OnCameraMove OnMove;

    private Camera _controlCamera;
    private float _cameraDistance;
    private float _yawRotate;
    private float _pitchRotate;

    private Vector3 _lookAt = Vector3.zero;
    private const float MaxTurnRotate = Mathf.PI;

    private Vector2 _pan;

    private static MapCamera _current;

    public static MapCamera Current
    {
        get { return _current; }
    }

    public float ZoomDistance { get { return _cameraDistance; } }

    private void Awake()
    {
        _controlCamera = GetComponent<Camera>();
        _controlCamera.transform.forward = -Vector3.one;

        _cameraDistance = 100f;
        CameraRotateSpace();

        _pan = Vector2.zero;
        _current = this;
    }

    private void LateUpdate()
    {
        if (Map.Current.IsShown())
        {
            var sensitivity = Mathf.Clamp(ZoomSensitivity * _cameraDistance / MaxCameraDistance, 2f, 100f);
            _cameraDistance = Mathf.Lerp(_cameraDistance, _cameraDistance - Input.GetAxis("MouseScrollWheel") * sensitivity, 10f * Time.unscaledDeltaTime);
            _cameraDistance = Mathf.Clamp(_cameraDistance, MinCameraDistance, MaxCameraDistance);
            if (Input.GetMouseButton(0))
            {
                //yawRotate += Mathf.Atan(Input.GetAxis("Mouse Steer X") / Mathf.Clamp(cameraDistance, 5f, MaxCameraDistance)) * Mathf.Rad2Deg;
                //pitchRotate += -Mathf.Atan(Input.GetAxis("Mouse Steer Y")/Mathf.Clamp(cameraDistance, 5f, MaxCameraDistance))*Mathf.Rad2Deg;
                _yawRotate = Input.GetAxis("MouseHorizontal") * RotateSpeed;
                _pitchRotate = -Input.GetAxis("MouseVertical") * RotateSpeed;
            }
            else
            {
                _yawRotate = 0f;
                _pitchRotate = 0f;
            }
            if (Input.GetMouseButton(1))
            {
                _pan.x = Input.GetAxis("MouseHorizontal") * PanSpeed;
                _pan.y = Input.GetAxis("MouseVertical") * PanSpeed;
            }
            else
            {
                _pan = Vector2.zero;
            }
            CameraRotateSpace();
        }

        if (OnMove != null)
            OnMove();
    }

    private void CameraRotateSpace()
    {
        _lookAt = Vector3.Lerp(_lookAt, _lookAt - (_controlCamera.transform.right * _pan.x + _controlCamera.transform.up * _pan.y), 10f * Time.unscaledDeltaTime);
        _controlCamera.transform.RotateAround(_lookAt, Vector3.up, _yawRotate);
        //controlCamera.transform.RotateAround(lookAt, controlCamera.transform.up, yawRotate);
        _controlCamera.transform.RotateAround(_lookAt, _controlCamera.transform.right, _pitchRotate);
        _controlCamera.transform.position = _lookAt - _controlCamera.transform.forward * _cameraDistance;
    }

    public void SetLookAt(Vector3 position)
    {
        _lookAt = position;
        CameraRotateSpace();
    }
}
