﻿using UnityEngine;

public class Fighter1 : Npc<Fighter1>
{
    public Vehicle VehiclePrefab;

    private Vehicle _vehicleInstance;

    public Vehicle VehicleInstance { get { return _vehicleInstance; } }

    private void Awake()
    {
        _vehicleInstance = Utility.InstantiateInParent(VehiclePrefab.gameObject, transform).GetComponent<Vehicle>();
        State = new Fighter1Chase(this);
    }

    private void Update()
    {
        UpdateState();
    }

    public Vector2 GetPitchYawToPoint(Vector3 point)
    {
        var toPoint = point - VehicleInstance.transform.position;
        var yawDiff = Vector3.Dot(toPoint, VehicleInstance.transform.right);
        var pitchDiff = Vector3.Dot(toPoint, VehicleInstance.transform.up);

        var yawTolerance = 2f;
        var pitchTolerance = 2f;

        var yawAmount = 0f;
        var pitchAmount = 0f;

        if (yawDiff < -yawTolerance)
        {
            yawAmount = Mathf.Clamp(Mathf.Abs(yawDiff) / yawTolerance, 0f, 1f);
        }
        else if (yawDiff > yawTolerance)
        {
            yawAmount = -Mathf.Clamp(Mathf.Abs(yawDiff) / yawTolerance, 0f, 1f);
        }

        if (pitchDiff < -pitchTolerance)
        {
            pitchAmount = Mathf.Clamp(Mathf.Abs(pitchDiff) / pitchTolerance, 0f, 1f);
        }
        else if (pitchDiff > pitchTolerance)
        {
            pitchAmount = -Mathf.Clamp(Mathf.Abs(pitchDiff) / pitchTolerance, 0f, 1f);
        }

        return new Vector2(pitchAmount, -yawAmount);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 150f, 50f, 100f, 30f), State.Name);
        GUI.Label(new Rect(Screen.width - 150f, 80f, 100f, 30f), string.Format("{0:f2}", VehicleInstance.GetVelocity().magnitude));
    }
}
