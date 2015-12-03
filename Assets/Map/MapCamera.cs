using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class MapCamera : MonoBehaviour
{
    public float ZoomSensitivity;
    public float MinCameraDistance;
    public float MaxCameraDistance;
    public float RotateSpeed = 3f;
    public float PanSpeed = 0.1f;

    private Camera controlCamera;
    private float cameraDistance;
    private float yawRotate;
    private float pitchRotate;
    private Vector3 initDirection;

    private Vector3 lookAt = Vector3.zero;
    private const float MaxTurnRotate = Mathf.PI;

    private Vector2 pan;

    private void Awake()
    {
        controlCamera = GetComponent<Camera>();
        initDirection = controlCamera.transform.forward;
        //cameraDistance = controlCamera.transform.position.magnitude;
        cameraDistance = 3f;
        CameraRotateSpace();

        pan = Vector2.zero;
    }

    private void Update()
    {
        if (Map.Current.IsShown())
        {
            cameraDistance = Mathf.Lerp(cameraDistance, cameraDistance - Input.GetAxis("MouseScrollWheel") * ZoomSensitivity, 10f * Time.deltaTime);
            cameraDistance = Mathf.Clamp(cameraDistance, MinCameraDistance, MaxCameraDistance);
            if (Input.GetMouseButton(0))
            {
                //yawRotate += Mathf.Atan(Input.GetAxis("Mouse Steer X") / Mathf.Clamp(cameraDistance, 5f, MaxCameraDistance)) * Mathf.Rad2Deg;
                //pitchRotate += -Mathf.Atan(Input.GetAxis("Mouse Steer Y")/Mathf.Clamp(cameraDistance, 5f, MaxCameraDistance))*Mathf.Rad2Deg;
                yawRotate = Input.GetAxis("MouseHorizontal") * RotateSpeed;
                pitchRotate = -Input.GetAxis("MouseVertical") * RotateSpeed;
            }
            else
            {
                yawRotate = 0f;
                pitchRotate = 0f;
            }
            if (Input.GetMouseButton(1))
            {
                pan.x = Input.GetAxis("MouseHorizontal") * PanSpeed;
                pan.y = Input.GetAxis("MouseVertical") * PanSpeed;
            }
            else
            {
                pan = Vector2.zero;
            }
            CameraRotateSpace();
        }
    }

    private void CameraRotateSpace()
    {
        lookAt = Vector3.Lerp(lookAt, lookAt - (controlCamera.transform.right * pan.x + controlCamera.transform.up * pan.y), 10f * Time.deltaTime);
        controlCamera.transform.RotateAround(lookAt, Vector3.up, yawRotate);
        //controlCamera.transform.RotateAround(lookAt, controlCamera.transform.up, yawRotate);
        controlCamera.transform.RotateAround(lookAt, controlCamera.transform.right, pitchRotate);
        controlCamera.transform.position = lookAt - controlCamera.transform.forward * cameraDistance;
    }

    public void SetLookAt(Vector3 position)
    {
        lookAt = position;
    }
}
