using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [Header("Vehicle Settings")]
    public float IdleSpeed = 10f;

    public float MaxSpeed = 20f;

    public float MinSpeed = 3f;

    public float Acceleration = 5f;

    public float Brake = 7f;

    public float CurrentSpeed = 0f;

    public float PitchSpeed = 5f;

    public float YawSpeed = 5f;


    [Header("Control Settings")]
    public bool IsAccelerating = false;

    public bool IsBraking = false;

    public float HorizontalTurn = 0f;

    public float VerticalTurn = 0f;

    [Header("Weapon")]
    public Weapon CurrentWeapon;

    private Shiftable _shiftable;

    public Shiftable Shiftable
    {
        get
        {
            if (_shiftable == null)
            {
                _shiftable = GetComponent<Shiftable>();
            }
            return _shiftable;
        }
    }

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
    }

    private void Update()
    {
        // Accelerating
        if (IsAccelerating && CurrentSpeed < MaxSpeed)
        {
            CurrentSpeed += Acceleration*Time.deltaTime;
            CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);
        }

        // Braking
        if (IsBraking && CurrentSpeed > MinSpeed)
        {
            CurrentSpeed -= Brake*Time.deltaTime;
            CurrentSpeed = Mathf.Max(CurrentSpeed, MinSpeed);
        }

        // Idling
        if (!IsAccelerating && !IsBraking)
        {
            if (CurrentSpeed > IdleSpeed)
            {
                CurrentSpeed -= Brake*Time.deltaTime;
                CurrentSpeed = Mathf.Max(IdleSpeed, CurrentSpeed);
            }

            if (CurrentSpeed < IdleSpeed)
            {
                CurrentSpeed += Acceleration*Time.deltaTime;
                CurrentSpeed = Mathf.Min(IdleSpeed, CurrentSpeed);
            }
        }

        // Turning
        transform.rotation *= Quaternion.Euler(0, HorizontalTurn*YawSpeed*Time.deltaTime, 0);
        transform.rotation *= Quaternion.Euler(VerticalTurn*PitchSpeed*Time.deltaTime, 0, 0);

        transform.position += transform.forward*CurrentSpeed*Time.deltaTime;
    }
}
