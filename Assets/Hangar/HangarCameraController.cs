using System;
using UnityEngine;

public class HangarCameraController : MonoBehaviour
{
    private static HangarCameraController current;

    public static HangarCameraController Current { get { return current; } }

    public delegate void OnCameraMove();
    public OnCameraMove OnMove;

    public Vector3 Centre = Vector3.zero;
    public Vector3 InitialOffset;

    public bool AllowFreeMove = true;

    [Header("Pitch")]
    public bool ClampPitch = true;
    public float MinPitch = -60f;
    public float MaxPitch = 30f;

    [Header("Zoom")]
    public float MinDistance = 2f;
    public float DefaultDistance = 25f;
    public float MaxDistance = 25f;

    private Rect dragArea;

    private Quaternion angle;
    private Quaternion smoothAngle;

    private Vector2 lastMousePos;
    private float pitch;
    private float yaw;

    private Quaternion targetAngle;
    private Vector3 targetPos;
    private float targetZoom;

    private float mouseAngle;

    private bool isPanning;
    private Action onFinishPanning;

    private bool isRotating;
    private Action onFinishRotating;

    private bool isDragging;

    private float distance = 25f;

    private void Awake()
    {
        current = this;

        Reset();

        mouseAngle = Mathf.PI / 10f;
        Time.timeScale = 1f;

        dragArea = new Rect(0, 0, Screen.width, Screen.height - 48);
        isDragging = false;
    }

    public void Reset()
    {
        transform.position = Centre + InitialOffset;
        var lookDirection = Quaternion.LookRotation(Centre - InitialOffset);
        yaw = lookDirection.eulerAngles.y;
        pitch = -lookDirection.eulerAngles.x;

        angle = Quaternion.AngleAxis(yaw, Vector3.up);
        angle *= Quaternion.AngleAxis(-pitch, Vector3.right);

        smoothAngle = angle;

        distance = Mathf.Clamp((InitialOffset - Centre).magnitude, MinDistance, MaxDistance);
    }

    public void SetDragArea(Rect area)
    {
        dragArea = area;
    }

    private void Update()
    {
        if (AllowFreeMove)
        {
            var mousePos = Input.mousePosition;
            if (!isPanning && !isRotating)
            {
                if (!isDragging)
                {
                    if (dragArea.Contains(mousePos))
                    {
                        if (Input.GetMouseButton(0))
                        {
                            isDragging = true;
                        }
                    }
                }
                distance = Mathf.Clamp(distance - (0.2f * distance) * Input.GetAxis("Mouse ScrollWheel"), MinDistance, MaxDistance);
            }
            if (!Input.GetMouseButton(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                var delta = new Vector2(mousePos.x - lastMousePos.x, mousePos.y - lastMousePos.y);
                if (ClampPitch)
                {
                    pitch = Mathf.Clamp((pitch + mouseAngle * delta.y) % 360f, MinPitch, MaxPitch);
                }
                else
                {
                    pitch = (pitch + mouseAngle * delta.y) % 360f;
                }
                yaw = (yaw + mouseAngle * delta.x) % 360f;
            }

            lastMousePos = mousePos;
        }
    }

    private void LateUpdate()
    {
        if (!isPanning && !isRotating)
        {
            angle = Quaternion.AngleAxis(yaw, Vector3.up);
            angle *= Quaternion.AngleAxis(-pitch, Vector3.right);

            smoothAngle = Quaternion.Slerp(smoothAngle, angle, 10f * Time.deltaTime);

            transform.position = Centre - smoothAngle * Vector3.forward * distance;
            transform.rotation = smoothAngle;
        }

        UpdatePanning();

        UpdateRotating();

        if (OnMove != null)
            OnMove();
    }

    private void UpdateRotating()
    {
        if (isRotating)
        {
            if (Quaternion.Angle(transform.rotation, targetAngle) < 0.001f)
            {
                pitch = -targetAngle.eulerAngles.x;
                yaw = targetAngle.eulerAngles.y;
                distance = targetZoom;
                isRotating = false;

                angle = Quaternion.AngleAxis(yaw, Vector3.up);
                angle *= Quaternion.AngleAxis(-pitch, Vector3.right);
                smoothAngle = angle;

                if (onFinishRotating != null)
                {
                    onFinishRotating();
                    onFinishRotating = null;
                }
            }
            distance = Mathf.Lerp(distance, targetZoom, 5f * Time.deltaTime);
            angle = Quaternion.Slerp(angle, targetAngle, 5f * Time.deltaTime);
            transform.position = Centre - angle * Vector3.forward * distance;
            transform.rotation = angle;
        }
    }

    private void UpdatePanning()
    {
        if (isPanning)
        {
            var destination = targetPos - targetZoom * transform.forward;
            var toDestination = destination - transform.position;
            if (toDestination.sqrMagnitude < 0.0001f)
            {
                Centre = targetPos;
                distance = targetZoom;
                isPanning = false;
                if (onFinishPanning != null)
                {
                    onFinishPanning();
                    onFinishPanning = null;
                }
            }
            transform.position = Vector3.Lerp(transform.position, destination, 10f * Time.deltaTime);
        }
    }

    public void RotateTo(Vector3 point, float zoom, Action onFinish)
    {
        targetAngle = Quaternion.LookRotation(Centre - point);
        targetZoom = zoom;
        isRotating = true;
        onFinishRotating = onFinish;
    }

    public void PanTo(Vector3 position, float zoom, Action onFinish)
    {
        targetPos = position;
        targetZoom = zoom;
        isPanning = true;
        onFinishPanning = onFinish;
    }

    public void SetPosition(Vector3 position, float zoom)
    {
        targetPos = position;
        targetZoom = zoom;

        Centre = targetPos;
        distance = targetZoom;

        transform.position = targetPos;
    }

    public void SetRotatedTo(Vector3 point, float zoom)
    {
        targetAngle = Quaternion.LookRotation(Centre - point);
        targetZoom = zoom;

        pitch = -targetAngle.eulerAngles.x;
        yaw = targetAngle.eulerAngles.y;

        angle = Quaternion.AngleAxis(yaw, Vector3.up);
        angle *= Quaternion.AngleAxis(-pitch, Vector3.right);
        smoothAngle = angle;

        distance = targetZoom;

        transform.position = Centre - angle * Vector3.forward * distance;
        transform.rotation = angle;
    }

    public Vector3 GetFocus()
    {
        if (isPanning)
            return targetPos;
        return Centre;
    }

    /*
    private void OnGUI()
    {
        // Debug drag area
        var y = Screen.height - dragArea.height - dragArea.y;
        GUI.Box(new Rect(dragArea.x, y, dragArea.width, dragArea.height), string.Empty);
    }
    */
}