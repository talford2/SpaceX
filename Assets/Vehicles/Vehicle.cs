using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [Header("Description")]
    public string Name;
    public GameObject PreviewPrefab;

    [Header("Vehicle Settings")]
    public Transform MeshTransform;
    public float IdleSpeed = 10f;
    public float MaxSpeed = 20f;
    public float MinSpeed = 3f;
    //public float Acceleration = 5f;
    public float Brake = 7f;
    public float MaxBoostSpeed = 500f;
    public float BoostAcceleration = 500f;
    public float BoostBrake = 150f;
    public float CurrentSpeed = 0f;
    public float PitchSpeed = 5f;
    public float YawSpeed = 5f;
    public float RollAcceleration = 640f;
    public float MaxRollSpeed = 100f;
    public float MaxBankingAngle = 60f;

    [Header("Collisions")]
    public Vector3 CollisionsCentre;
    public float CollisionRadius = 3f;

    [Header("Boost Energy")]
    //public float MaxBoostEnergy = 100f;
    public float BoostEnergy = 100f;
    //public float BoostCost = 50f;
    public float BoostEnergyRegenerateDelay = 5f;
    public float BoostEnergyRegenerateRate = 1f;

    [Header("Control Settings")]
    public GameObject Controller;
    public bool TriggerAccelerate = false;
    public bool TriggerBrake = false;
    public bool TriggerBoost = false;
    public float YawThrottle = 0f;
    public float PitchThotttle = 0f;
    public float RollThrottle = 0f;

    [Header("Camera")]
    public float CameraDistance = 15f;

    [Header("U-Turn")]
    public SplinePath UTurnPath;
    public float UTurnDuration;

    public bool IsAccelerating
    {
        get { return TriggerAccelerate; }
    }

    public bool IsBraking
    {
        get { return TriggerBrake; }
    }

    public bool IsBoosting
    {
        get { return TriggerBoost && BoostEnergy > 0f && _allowBoost; }
    }

    [Header("Primary Weapon")]
    public Weapon PrimaryWeaponPrefab;

    public List<ShootPoint> PrimaryShootPoints;

    [Header("Secondary Weapon")]
    public Weapon SecondaryWeaponPrefab;

    public List<ShootPoint> SecondaryShootPoints;

    [Header("Collisions")]
    public GameObject CollideEffect;

    [Header("Shield")]
    public Shield ShieldPrefab;

    [Header("Engine")]
    public Engine EnginePrefab;

    [Header("Damage")]
    public GameObject WoundEffect;

    [Header("Death")]
    public GameObject CorpsePrefab;
    public GameObject DebrisPrefab;
    public float ExplosiveForce;
    public GameObject ExplodeDeathExplosion;
    public GameObject SpinDeathExplosion;

    [Header("Thrusters")]
    public GameObject ThrusterPrefab;
    public List<Transform> ThrusterTransforms;

    private Shiftable _shiftable;

    private Weapon _primaryWeaponInstance;
    private Weapon _secondaryWeaponInstance;
    private Shield _shieldInstance;
    private Engine _engineInstance;

    private Vector3 _velocity;
    private VelocityReference _velocityReference;

    private readonly float _aimDistance = 1000f;

    // Steering
    private float _rollSpeed;
    private Quaternion _targetRotation;
    private Quaternion _targetBankRotation;

    // Boost
    private float _boostCost = 50f;
    private float _boostEnergyCooldown;
    private bool _boostRegenerate;
    private bool _allowBoost;

    private float _maxFlareBrightness = 30f;

    private int _environmentMask;

    // Barrel Roll
    private bool isBarrelRolling;
    private int barrelRollDir;
    private float barrelRollCooldown;
    private float barrelRollDuration = 0.5f;
    private float barrelStrafeSpeed = 100f;
    private float barrelStrafeVelocity;

    private bool _previousBoost = false;

    // Thrusters
    private List<Thruster> _thrusters;

    // Wound effect
    private GameObject _woundObj;

    // Death effect
    private float _minVelocityMultiplier = 1.5f;
    private float _maxVelocityMultiplier = 2f;

    public Weapon PrimaryWeaponInstance
    {
        get { return _primaryWeaponInstance; }
    }

    public Weapon SecondaryWeaponInstance
    {
        get { return _secondaryWeaponInstance; }
    }

    public Shield ShieldInstance
    {
        get { return _shieldInstance; }
    }

    public Engine EngineInstance
    {
        get { return _engineInstance; }
    }

    public bool IgnoreCollisions;

    public Killable Killable
    {
        get { return _killable; }
    }

    private Killable _killable;
    private Targetable _targetable;

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

    public Vector3 GetAimPosition()
    {
        if (_primaryWeaponInstance != null)
            return _primaryWeaponInstance.GetShootPointCentre() + _aimDistance * transform.forward;
        return transform.position + _aimDistance * transform.forward;
    }

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();

        foreach (var shootPoint in PrimaryShootPoints)
        {
            if (PrimaryWeaponPrefab != null)
                shootPoint.Initialize(PrimaryWeaponPrefab.MuzzlePrefab);
        }
        foreach (var shootPoint in SecondaryShootPoints)
        {
            if (SecondaryWeaponPrefab != null)
                shootPoint.Initialize(SecondaryWeaponPrefab.MuzzlePrefab);
        }
        _thrusters = new List<Thruster>();
        if (ThrusterPrefab != null)
        {
            foreach (var thrusterTransform in ThrusterTransforms)
            {
                var thrusterInstance = Instantiate(ThrusterPrefab).GetComponent<Thruster>();
                thrusterInstance.transform.SetParent(thrusterTransform);
                thrusterInstance.transform.localPosition = Vector3.zero;
                thrusterInstance.transform.localRotation = Quaternion.identity;
                thrusterInstance.transform.localScale = Vector3.one;
                _thrusters.Add(thrusterInstance);
            }
        }
        _velocityReference = new VelocityReference(_velocity);

        _killable = GetComponent<Killable>();
        _killable.OnDamage += VehicleDamage;
        _killable.OnDie += VehicleDie;

        _allowBoost = true;

        _targetRotation = transform.rotation;

        _environmentMask = LayerMask.GetMask("Environment");
    }

    public void Initialize()
    {
        _targetable = GetComponent<Targetable>();

        if (PrimaryWeaponPrefab != null)
            SetPrimaryWeapon(PrimaryWeaponPrefab.gameObject);
        if (SecondaryWeaponPrefab != null)
            SetSecondaryWeapon(SecondaryWeaponPrefab.gameObject);
        if (ShieldPrefab != null)
            SetShield(ShieldPrefab.gameObject);
        if (EnginePrefab != null)
            SetEngine(EnginePrefab.gameObject);
    }

    public void SetPrimaryWeapon(GameObject primaryWeapon)
    {
        if (_primaryWeaponInstance != null)
        {
            _primaryWeaponInstance.IsTriggered = false;
            _primaryWeaponInstance.ClearTargetLock();
            _primaryWeaponInstance.OnShoot -= OnShoot;
            Destroy(_primaryWeaponInstance.gameObject);
        }
        if (primaryWeapon != null)
        {
            _primaryWeaponInstance = Utility.InstantiateInParent(primaryWeapon.gameObject, transform).GetComponent<Weapon>();
            _primaryWeaponInstance.Initialize(gameObject, PrimaryShootPoints, _velocityReference, _targetable.Team);
            _primaryWeaponInstance.OnShoot += OnShoot;
        }
    }

    public void SetSecondaryWeapon(GameObject secondaryWeapon)
    {
        if (_secondaryWeaponInstance != null)
        {
            _secondaryWeaponInstance.IsTriggered = false;
            _secondaryWeaponInstance.ClearTargetLock();
            _secondaryWeaponInstance.OnShoot -= OnShoot;
            Destroy(_secondaryWeaponInstance.gameObject);
        }
        if (secondaryWeapon != null)
        {
            _secondaryWeaponInstance = Utility.InstantiateInParent(secondaryWeapon.gameObject, transform).GetComponent<Weapon>();
            _secondaryWeaponInstance.Initialize(gameObject, SecondaryShootPoints, _velocityReference, _targetable.Team);
            _secondaryWeaponInstance.OnShoot += OnShoot;
        }
    }

    public void SetShield(GameObject shield)
    {
        if (_shieldInstance != null)
        {
            Destroy(_shieldInstance.gameObject);
        }
        if (shield != null)
        {
            _shieldInstance = Utility.InstantiateInParent(shield.gameObject, transform).GetComponent<Shield>();
            _killable.MaxShield = _shieldInstance.Capacity;
            if (_killable.Shield > _shieldInstance.Capacity)
                _killable.Shield = _shieldInstance.Capacity;
            var regenerator = GetComponent<ShieldRegenerator>();
            if (regenerator != null)
                regenerator.RegenerationRate = _shieldInstance.RegenerationRate;
        }
    }

    public void SetEngine(GameObject engine)
    {
        if (_engineInstance != null)
        {
            Destroy(_engineInstance.gameObject);
        }
        if (engine != null)
        {
            _engineInstance = Utility.InstantiateInParent(engine.gameObject, transform).GetComponent<Engine>();
        }
    }

    public void SetTargetRotation(Quaternion rotation)
    {
        _targetRotation = rotation;
    }

    private void Start()
    {
        foreach (var thruster in _thrusters)
        {
            thruster.Initialize();
        }
    }

    public void SetAimAt(Vector3 aimAt)
    {
        if (_primaryWeaponInstance != null)
            _primaryWeaponInstance.SetAimAt(aimAt);
        if (_secondaryWeaponInstance != null)
            _secondaryWeaponInstance.SetAimAt(aimAt);
    }

    public void TriggerBarrelRoll(int dir)
    {
        if (!isBarrelRolling)
        {
            isBarrelRolling = true;
            barrelRollDir = dir;
            barrelRollCooldown = barrelRollDuration;
        }
    }

    private bool isUTurning;
    private float uTurnCooldown;

    public void TriggerUTurn()
    {
        if (!isUTurning)
        {
            isUTurning = true;
            uTurnCooldown = UTurnDuration;
            var camPos = Universe.Current.ViewPort.transform.position;
            Universe.Current.ViewPort.SetFree(true);
            Universe.Current.ViewPort.transform.position = camPos;
        }
    }

    private void OnShoot(int shootPointIndex)
    {
    }

    private void Update()
    {
        var acceleration = 0f;

        if (!TriggerBoost)
        {
            // Accelerating
            if (TriggerAccelerate && CurrentSpeed < MaxSpeed)
            {
                acceleration = _engineInstance.Acceleration;
                CurrentSpeed += _engineInstance.Acceleration * Time.deltaTime;
                CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);
            }

            // Braking
            if (TriggerBrake && CurrentSpeed > MinSpeed)
            {
                acceleration = -Brake;
                CurrentSpeed -= Brake * Time.deltaTime;
                CurrentSpeed = Mathf.Max(CurrentSpeed, MinSpeed);
            }

            _allowBoost = true;
        }

        // Restore boost energy
        if (_boostEnergyCooldown > 0f)
        {
            _boostEnergyCooldown -= Time.deltaTime;
            if (_boostEnergyCooldown < 0f)
            {
                _boostRegenerate = true;
            }
        }

        if (_boostRegenerate)
        {
            if (BoostEnergy < _engineInstance.BoostEnergy)
            {
                BoostEnergy += BoostEnergyRegenerateRate * Time.deltaTime;
                if (BoostEnergy > _engineInstance.BoostEnergy)
                    BoostEnergy = _engineInstance.BoostEnergy;
            }
        }

        // Boosting
        if (TriggerBoost)
        {
            if (_allowBoost && BoostEnergy > 0f)
            {
                if (CurrentSpeed < MaxBoostSpeed)
                {
                    acceleration = BoostAcceleration;
                    CurrentSpeed += BoostAcceleration * Time.deltaTime;
                    CurrentSpeed = Mathf.Min(CurrentSpeed, MaxBoostSpeed);
                }
                BoostEnergy -= _boostCost * Time.deltaTime;
                if (BoostEnergy < 0f)
                {
                    BoostEnergy = 0f;
                    _allowBoost = false;
                }
                _boostRegenerate = false;
                _boostEnergyCooldown = BoostEnergyRegenerateDelay;
            }
        }

        if (!IsBoosting)
        {
            if (CurrentSpeed > MaxSpeed)
            {
                CurrentSpeed -= BoostBrake * Time.deltaTime;
            }
        }

        // Idling
        if (!IsAccelerating && !IsBraking && !IsBoosting)
        {
            if (CurrentSpeed > IdleSpeed)
            {
                acceleration = -Brake;
                CurrentSpeed -= Brake * Time.deltaTime;
                CurrentSpeed = Mathf.Max(IdleSpeed, CurrentSpeed);
            }

            if (CurrentSpeed < IdleSpeed)
            {
                acceleration = _engineInstance.Acceleration;
                CurrentSpeed += _engineInstance.Acceleration * Time.deltaTime;
                CurrentSpeed = Mathf.Min(IdleSpeed, CurrentSpeed);
            }
        }

        _rollSpeed += RollAcceleration * Mathf.Clamp(RollThrottle, -1, 1) * Time.deltaTime;

        if (Mathf.Abs(RollThrottle) < 0.01f)
        {
            _rollSpeed = Mathf.Lerp(_rollSpeed, 0f, 10f * Time.deltaTime);
        }

        // Turning
        var pitchYawRoll = new Vector3(PitchSpeed * PitchThotttle, YawSpeed * YawThrottle, -Mathf.Clamp(_rollSpeed, -MaxRollSpeed, MaxRollSpeed));

        _targetRotation *= Quaternion.Euler(pitchYawRoll * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, 5f * Time.deltaTime);

        if (isBarrelRolling)
        {
            barrelRollCooldown -= Time.deltaTime;
            if (barrelRollCooldown < 0f)
                isBarrelRolling = false;
        }

        // Banking
        if (!isBarrelRolling)
        {
            _targetBankRotation = Quaternion.Lerp(_targetBankRotation, Quaternion.AngleAxis(-YawThrottle * MaxBankingAngle, Vector3.forward), 5f * Time.deltaTime);
        }
        else
        {
            var barrelRollFraction = barrelRollCooldown / barrelRollDuration;
            _targetBankRotation = Quaternion.AngleAxis(barrelRollDir * barrelRollFraction * 360f, Vector3.forward);
        }
        MeshTransform.localRotation = Quaternion.Lerp(MeshTransform.localRotation, _targetBankRotation, 5f * Time.deltaTime);

        _velocity = transform.forward * CurrentSpeed;
        var targetBarrelVelocity = 0f;
        if (isBarrelRolling)
            targetBarrelVelocity = barrelRollDir * barrelStrafeSpeed;
        barrelStrafeVelocity = Mathf.Lerp(barrelStrafeVelocity, targetBarrelVelocity, 5f * Time.deltaTime);
        _velocity += transform.right * barrelStrafeVelocity;

        if (!IgnoreCollisions)
            UpdateVelocityFromCollisions();

        if (isUTurning)
        {
            uTurnCooldown -= Time.deltaTime;
            if (uTurnCooldown > 0f)
            {
                var uTurnFraction = Mathf.Clamp01(uTurnCooldown / UTurnDuration);
                transform.localPosition = UTurnPath.GetPoint(uTurnFraction);
            }
            else
            {
                isUTurning = false;
                Universe.Current.ViewPort.SetFree(false);
            }
        }
        else
        {
            _velocityReference.Value = _velocity;
            _shiftable.Translate(_velocity * Time.deltaTime);
        }

        var thrustAmount = acceleration / MaxSpeed;

        if (IsBoosting)
        {
            thrustAmount = 1f;
            if (_previousBoost != IsBoosting)
            {
                StartBoost();
            }
        }
        else
        {
            if (IsAccelerating)
            {
                thrustAmount = 0.08f;
            }
            if (IsBraking)
            {
                thrustAmount = 0f;
            }

            if (_previousBoost != IsBoosting)
            {
                StopBoost();
            }
        }
        if (!IsBoosting && !IsAccelerating && !IsBraking)
        {
            thrustAmount = 0.02f;
        }

        // Reduce flare brightness over distance from camera
        /*
		foreach (var thruster in Thrusters)
		{
			thruster.SetAmount(thrustAmount);
			thruster.UpdateFlare();
		}
        */

        _previousBoost = IsBoosting;
    }

    private void StartBoost()
    {
        _thrusters.ForEach(t => t.TriggerBoost());
    }

    private void StopBoost()
    {
        _thrusters.ForEach(t => t.StopBoost());
    }

    public void TriggerBoostRegeneration()
    {
        _boostRegenerate = true;
        if (_boostEnergyCooldown < 0f)
            _boostEnergyCooldown = BoostEnergyRegenerateDelay;
    }

    private RaycastHit[] _moveHits = new RaycastHit[20];

    private void UpdateVelocityFromCollisions()
    {
        var moveRay = new Ray(transform.position + CollisionsCentre, _velocity);

        var castCount = Physics.SphereCastNonAlloc(moveRay, CollisionRadius, _moveHits, _velocity.magnitude * Time.deltaTime, _environmentMask);
        if (castCount > 0)
        {
            if (CollideEffect != null)
            {
                ResourcePoolManager.GetAvailable(CollideEffect, _moveHits[0].point, Quaternion.LookRotation(_moveHits[0].normal));
            }
            _killable.Damage(_velocity.magnitude * 0.1f, _moveHits[0].point, _moveHits[0].normal, null);
        }
        for (var i = 0; i < castCount; i++)
        {
            var projVel = Vector3.Project(_velocity, _moveHits[i].normal);
            if ((_moveHits[i].point - Vector3.zero).sqrMagnitude > 0.00001f)
                _velocity -= projVel;
        }
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    private void VehicleDamage(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
    {
        if (sender.Health / sender.MaxHealth < 0.5f)
        {
            StartWoundEffect();
        }
    }

    private void StartWoundEffect()
    {
        if (_woundObj == null && WoundEffect != null)
        {
            _woundObj = Instantiate(WoundEffect);
            _woundObj.transform.parent = transform;
            _woundObj.transform.localPosition = Vector3.zero;
        }
    }

    private IEnumerator DelayedWoundEffectStop(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(_woundObj);
    }

    private void VehicleDie(Killable sender, GameObject attacker)
    {
        _killable.OnDie -= VehicleDie;
        if (_woundObj != null)
        {
            _woundObj.transform.parent = null;
            var woundParticles = _woundObj.GetComponent<ParticleSystem>();
            if (woundParticles != null)
                woundParticles.Stop();
            StartCoroutine(DelayedWoundEffectStop(woundParticles.duration + 0.5f));
        }

        if (Random.Range(0, 1f) > 0.5f)
        {
            Explode(SpinDeathExplosion);
            if (CorpsePrefab != null)
            {
                var corpseInstance = (GameObject)Instantiate(CorpsePrefab, transform.position, transform.rotation);
                var rBody = corpseInstance.GetComponent<Rigidbody>();
                rBody.velocity = _velocity * Random.Range(_minVelocityMultiplier, _maxVelocityMultiplier);
                rBody.AddForce(_velocity.normalized * 5000f, ForceMode.Impulse);
                rBody.AddRelativeTorque(new Vector3(0f, 0f, 50000f), ForceMode.Impulse);
                var shiftable = corpseInstance.GetComponentInParent<Shiftable>();
                shiftable.SetShiftPosition(Universe.Current.GetUniversePosition(rBody.position));
            }
        }
        else
        {
            Explode(ExplodeDeathExplosion);
            if (DebrisPrefab != null)
            {
                var debrisInstance = (GameObject)Instantiate(DebrisPrefab, transform.position, transform.rotation);
                var rBodies = debrisInstance.GetComponentsInChildren<Rigidbody>();
                foreach (var rBody in rBodies)
                {
                    rBody.velocity = GetVelocity();
                    rBody.AddExplosionForce(ExplosiveForce, transform.position, 20f, 0f, ForceMode.Impulse);
                    var shiftable = rBody.GetComponent<Shiftable>();
                    shiftable.SetShiftPosition(Universe.Current.GetUniversePosition(rBody.position));
                }
            }
        }
    }

    private void Explode(GameObject explodePrefab)
    {
        if (explodePrefab != null)
        {
            var explodeInstance = ResourcePoolManager.GetAvailable(explodePrefab, transform.position, transform.rotation);
            explodeInstance.transform.localScale = transform.localScale;
            var explodeShiftable = explodeInstance.GetComponent<Shiftable>();
            if (explodeShiftable != null)
                explodeShiftable.SetShiftPosition(Universe.Current.GetUniversePosition(transform.position));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + CollisionsCentre, CollisionRadius);
    }
}