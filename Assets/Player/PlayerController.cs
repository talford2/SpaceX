using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool InvertY;
    public bool HideMouse = false;
    public float MouseMoveClamp = 1f;
    public float DoubleTapTime = 0.5f;

    [Header("Aiming")]
    public float DefaultAimDistance = 500f;
    public float MinAimDistance = 5f;
    public float MaxAimDistance = 1000f;
    public float AimCorrectScreenFraction = 0.01f;

    private bool _cursorLocked;
    private int _aimMask;
    private float _aimDistance;
    private Transform _guessTarget;

    // Barrel Roll Trigger
    private int _lastRollSign;
    private float _lastLeftTime;
    private float _lastRightTime;

    private DroneHive _droneHive;

    private void Awake()
    {
        _aimMask = ~LayerMask.GetMask("Player", "Detectable", "Distant");

        Cursor.visible = !HideMouse;
        if (HideMouse)
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        /* Miscellaneous Controls */
        if (Input.GetKeyDown(KeyCode.LeftControl))
            Debug.Break();

        var playerVehicle = Player.Current.VehicleInstance;
        if (playerVehicle != null)
        {
            // 'God Mode'
            if (Input.GetKeyUp(KeyCode.G))
            {
                playerVehicle.Killable.MaxHealth = 10000f;
                playerVehicle.Killable.Health = playerVehicle.Killable.MaxHealth;
            }

            // Instant Death
            if (Input.GetKeyUp(KeyCode.Z))
            {
                playerVehicle.Killable.Die(Vector3.zero, Vector3.up, gameObject);
            }

            // U-Turn
            if (Input.GetKeyUp(KeyCode.T))
            {
                playerVehicle.TriggerUTurn();
            }

            // No Target mode
            if (Input.GetKeyUp(KeyCode.N))
            {
                Targeting.RemoveTargetable(playerVehicle.Targetable.Team, playerVehicle.Targetable.transform);
            }
        }

        if (Input.GetKey(KeyCode.M))
        {
            Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(0.3f, 0.7f);
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            Cursor.lockState = !_cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            _cursorLocked = !_cursorLocked;
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Map.Current.Toggle();
        }
    }

    public void ControlVehicle(Vehicle vehicle)
    {
        var mouseHorizontal = Input.GetAxis("MouseHorizontal");
        var mouseVertical = Input.GetAxis("MouseVertical");

        var controllerHorizontal = 0f;
        var controllerVertical = 0f;

        var _pitchYaw = Vector2.ClampMagnitude(new Vector2(controllerVertical + mouseVertical, controllerHorizontal + mouseHorizontal), MouseMoveClamp);

        vehicle.PitchThotttle = InvertY ? _pitchYaw.x : _pitchYaw.x * -1;

        vehicle.YawThrottle = _pitchYaw.y;
        vehicle.RollThrottle = Input.GetAxis("Roll") + Input.GetAxis("KeyboardRoll");
        if (vehicle.PrimaryWeaponInstance != null)
        {
            if (!vehicle.PrimaryWeaponInstance.Definition.IsTargetLocking)
                vehicle.PrimaryWeaponInstance.SetMissileTarget(_guessTarget);
            vehicle.PrimaryWeaponInstance.IsTriggered = (Input.GetAxis("FireTrigger") + Input.GetAxis("MouseFireTrigger")) > 0;
        }
        if (vehicle.SecondaryWeaponInstance != null)
        {
            if (!vehicle.SecondaryWeaponInstance.Definition.IsTargetLocking)
                vehicle.SecondaryWeaponInstance.SetMissileTarget(_guessTarget);
            vehicle.SecondaryWeaponInstance.IsTriggered = (Input.GetAxis("AltFireTrigger") + Input.GetAxis("MouseAltFireTrigger")) > 0;
        }

        vehicle.SetAimAt(GetAimAt(vehicle));

        // Barrel roll trigger
        var curRollSign = Mathf.RoundToInt(vehicle.RollThrottle);
        if (curRollSign != _lastRollSign)
        {
            if (_lastRollSign == 0)
            {
                if (curRollSign == -1)
                {
                    if (Time.time - _lastLeftTime < DoubleTapTime)
                        vehicle.TriggerBarrelRoll(curRollSign);
                    _lastLeftTime = Time.time;
                }
                if (curRollSign == 1)
                {
                    if (Time.time - _lastRightTime < DoubleTapTime)
                        vehicle.TriggerBarrelRoll(curRollSign);
                    _lastRightTime = Time.time;
                }
            }
        }
        _lastRollSign = curRollSign;

        vehicle.TriggerAccelerate = false;
        if (Input.GetButton("Accelerate") || Input.GetButton("KeyboardAccelerate"))
        {
            vehicle.TriggerAccelerate = true;
        }

        vehicle.TriggerBrake = false;
        if (Input.GetButton("Brake") || Input.GetButton("KeyboardBrake"))
        {
            vehicle.TriggerBrake = true;
        }

        vehicle.TriggerBoost = false;
        if (Input.GetButton("Boost") || Input.GetButton("KeyboardBoost"))
        {
            vehicle.TriggerBoost = true;
        }

        if (_droneHive == null)
            _droneHive = vehicle.GetComponent<DroneHive>();
        if (_droneHive != null)
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                _droneHive.ReleaseDrones(5);
            }
        }

        if (vehicle.IsBoosting)
        {
            //Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(0.04f, 1f);
        }
    }

    private Vector3 GetAimAt(Vehicle vehicle)
    {
        var viewCentre = new Vector3(0.5f, 0.5f, 0f);
        var mouseRay = Universe.Current.ViewPort.AttachedCamera.ViewportPointToRay(viewCentre);
        RaycastHit aimHit;
        _aimDistance = Mathf.Lerp(_aimDistance, DefaultAimDistance, Time.deltaTime);
        var aimAtPosition = mouseRay.GetPoint(DefaultAimDistance);

        // Fancy System.
        var viewPortPos = Universe.Current.ViewPort.transform.position;
        var viewPortForward = Universe.Current.ViewPort.transform.forward;
        var dotViewPort = vehicle.PrimaryWeaponInstance != null
            ? Vector3.Dot(vehicle.PrimaryWeaponInstance.GetShootPointCentre() - viewPortPos, viewPortForward)
            : Vector3.Dot(vehicle.transform.position - viewPortPos, viewPortForward);
        _guessTarget = Targeting.FindFacingAngleAny(viewPortPos + dotViewPort * viewPortForward, viewPortForward, MaxAimDistance, 5f);
        if (_guessTarget != null)
        {
            var toGuessTarget = _guessTarget.position - viewPortPos;
            if (toGuessTarget.sqrMagnitude < 1000f * 1000f)
            {
                /*
                _aimDistance = Mathf.Clamp(toGuessTarget.magnitude + 0.5f, MinAimDistance, MaxAimDistance);
                var screenPos = Universe.Current.ViewPort.AttachedCamera.WorldToViewportPoint(_guessTarget.position) - viewCentre;
                var v = new Vector2(screenPos.x, screenPos.y);
                if (v.sqrMagnitude < AimCorrectScreenFraction * AimCorrectScreenFraction)
                    return _guessTarget.position;
                //Debug.LogFormat("({0:f3}, {1:f3}) - {2:f3}", v.x, v.y, v.magnitude);
                return mouseRay.GetPoint(_aimDistance);
                */
            }
        }

        if (Physics.Raycast(mouseRay, out aimHit, MaxAimDistance, _aimMask))
        {
            _aimDistance = Mathf.Clamp(aimHit.distance + 0.5f, MinAimDistance, MaxAimDistance);
            aimAtPosition = mouseRay.GetPoint(_aimDistance);
        }
        return aimAtPosition;
    }
}