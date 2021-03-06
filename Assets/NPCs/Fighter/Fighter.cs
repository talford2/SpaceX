﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fighter : Npc<Fighter>
{
    public Vehicle VehiclePrefab;
    public Team Team;
    public VehicleTrackerValues TrackerOptions;
    public bool IsSquadronMember;

    [Header("Drop Item on Death")]
    public List<Collectible> DropItems;
    public int MaxDropAmount = 5;

    private Vehicle _vehicleInstance;

    [Header("Movement")]
    public Vector3 Destination;

    public Transform Target;
    public float MaxTargetDistance = 2000f;

    public float SteerMultiplier = 0.3f;

    public ProximitySensor ProximitySensor;
    public bool IsDebugSpawn;

    [Header("Idle Destination")]
    public bool IsFollowIdleDestination;
    public Vector3 IdleDestination;
    public Vector3 IdleUpDestination;

    [Header("Attack")]
    public float AttackRange = 100f;
    public float ShootAngleTolerance = 5f;
    public float OvertakeDistance = 50f;
    public float BurstTime = 0.5f;
    public float BurstWaitTime = 0.7f;
    public float ExrapolationTimeError = 0.5f;
    public float MinAimOffsetRadius = 5f;
    public float MaxAimOffsetRadius = 20f;

    [Header("Evade")]
    public float EvadeDistance = 200f;
    public float TurnAroundDistance = 250f;
    public float AcclerateDistance = 100f;
    public float MinDodgeIntervalTime = 1f;
    public float MaxDodgeIntervalTime = 6f;
    public float DodgeRadius = 50f;
    public float DodgeArcAngle = 180f;
    public bool CanBarrelRoll = false;

    [Header("Chase")]
    public float SightRange = 50f;

    public Vehicle VehicleInstance { get { return _vehicleInstance; } }
    public FighterSteering Steering { get; set; }
    public Action<Transform> OnVehicleDamage;

    public UniversePosition PathDestination;

    // States
    public FighterIdle Idle;
    public FighterChase Chase;
    public FighterAttack Attack;
    public FighterEvade Evade;
    public FighterPath Path;

    // Neighbour Detection
    private float _neighborDetectInterval = 0.2f;
    private float _neighborDetectCooldown;
    public List<Detectable> Neighbours;

    private void Awake()
    {
        //_vehicleInstance = Utility.InstantiateInParent(VehiclePrefab.gameObject, transform).GetComponent<Vehicle>();

        if (VehiclePrefab != null)
        {
            var parentShifter = transform.GetComponentInParent<Shiftable>();
            if (parentShifter != null)
            {
                //SpawnVehicle(VehiclePrefab, parentShifter.UniversePosition);
            }
            else
            {
                // This isn't right!
                //SpawnVehicle(VehiclePrefab, new UniversePosition(new CellIndex(0,0,0), new Vector3()));
            }
        }

        GetComponent<ShipProfile>().CallSign = NameGenerator.GetRandomCallSign();

        Idle = new FighterIdle(this);
        Chase = new FighterChase(this);
        Attack = new FighterAttack(this);
        Evade = new FighterEvade(this);
        Path = new FighterPath(this);

        Steering = new FighterSteering(this);
        SetState(Idle);

        if (IsDebugSpawn)
        {
            SpawnVehicle(gameObject, VehiclePrefab, new UniversePosition(new CellIndex(0, 0, 0), new Vector3(0, 0, 0)), Quaternion.identity);
            _vehicleInstance.GetComponent<VehicleTracker>().enabled = false;
        }
    }

    public void SetPath(UniversePosition destPosition)
    {
        PathDestination = destPosition;
        SetState(Path);
    }

    private void Update()
    {
        if (VehicleInstance != null)
            UpdateState();

        if (VehicleInstance != null)
        {
            var pitchYaw = GetPitchYawToPoint(Destination);
            if (pitchYaw.sqrMagnitude <= 0f)
            {
                // Give random value to resolve zero pitchYaw issue.
                pitchYaw = Random.insideUnitCircle;
            }
            VehicleInstance.YawThrottle = pitchYaw.y;
            VehicleInstance.PitchThotttle = pitchYaw.x;
        }
    }

    public void SpawnVehicle(GameObject controller, Vehicle vehiclePrefab, UniversePosition universePosition, Quaternion rotation)
    {
        _vehicleInstance = Instantiate<Vehicle>(vehiclePrefab);
        _vehicleInstance.Targetable.Team = Team;
        _vehicleInstance.Tracker.Options = TrackerOptions;
        var killable = _vehicleInstance.GetComponent<Killable>();
        killable.OnDamage += OnVehicleDamaged;
        killable.OnDie += OnVehicleDestroyed;

        _vehicleInstance.Initialize();

        // Apply power profile
        var powerProfile = GetComponent<ShipProfile>();
        killable.MaxShield = powerProfile.GetShield();
        killable.Shield = killable.MaxShield;

        _vehicleInstance.Shiftable.SetShiftPosition(universePosition);
        _vehicleInstance.transform.position = _vehicleInstance.Shiftable.GetWorldPosition();
        _vehicleInstance.SetTargetRotation(rotation);
        _vehicleInstance.transform.rotation = rotation;
        _vehicleInstance.Controller = controller;
        ProximitySensor = _vehicleInstance.GetComponent<ProximitySensor>();
    }

    public void SetVehicleInstance(Vehicle vehicleInstance)
    {
        _vehicleInstance = vehicleInstance;
        //_vehicleInstance.GetComponent<Targetable>().Team = Team;
        _vehicleInstance.GetComponent<Killable>().OnDie += OnVehicleDestroyed;
        _vehicleInstance.Controller = gameObject;
        ProximitySensor = _vehicleInstance.GetComponent<ProximitySensor>();
    }

    public Vector2 GetPitchYawToPoint(Vector3 point)
    {
        var toPoint = point - VehicleInstance.transform.position;
        var yawAmount = Vector3.Dot(toPoint.normalized, VehicleInstance.transform.right);
        var pitchAmount = Vector3.Dot(-toPoint.normalized, VehicleInstance.transform.up);
        return new Vector2(pitchAmount, yawAmount);
    }

    public void SetTarget(Transform target)
    {
        Target = target;
    }

    private void DetectNeighbor(Detectable neighbour)
    {
        if (neighbour != VehicleInstance.transform)
        {
            if (!Neighbours.Contains(neighbour))
            {
                Neighbours.Add(neighbour);
            }
        }
    }

    public void CheckSensors()
    {
        if (_neighborDetectCooldown >= 0f)
        {
            _neighborDetectCooldown -= Time.deltaTime;
            if (_neighborDetectCooldown < 0f)
            {
                Neighbours = new List<Detectable>();
                ProximitySensor.Detect(DetectNeighbor);
                _neighborDetectCooldown = _neighborDetectInterval;
            }
        }
    }

    private void OnVehicleDamaged(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
    {
        if (attacker != null)
        {
            if (attacker.transform != null)
            {
                if (Target == null)
                    Target = attacker.transform;
                if (OnVehicleDamage != null)
                    OnVehicleDamage(attacker.transform);
            }
        }
    }

    private void OnVehicleDestroyed(Killable sender, Vector3 positon, Vector3 normal, GameObject attacker)
    {
        Target = null;
        if (Universe.Current.KillDropItems && DropItems != null && DropItems.Count > 0)
        {
            var dropAmount = Random.Range(0, MaxDropAmount + 1);
            for (var i = 0; i < dropAmount; i++)
            {
                var dropPosition = VehicleInstance.transform.position + Random.onUnitSphere * 1.5f;
                var collectible = DropItems[Random.Range(0, DropItems.Count)].gameObject;
                var dropItem = Instantiate(collectible, VehicleInstance.transform.position + Random.onUnitSphere * 1.5f, Quaternion.identity).GetComponent<Collectible>();
                dropItem.Shiftable.SetShiftPosition(Universe.Current.GetUniversePosition(dropPosition));
                dropItem.SetVelocity(VehicleInstance.GetVelocity() + Random.onUnitSphere * 5f);
            }
        }
        if (attacker != null)
        {
            var attackerTargetable = attacker.GetComponentInChildren<Targetable>();
            if (attackerTargetable != null)
                HeadsUpDisplay.Current.RecordKill(attackerTargetable.Team);
        }
    }

    private void OnDrawGizmos()
    {
        if (VehicleInstance != null)
        {
            var stateColors = new Dictionary<string, Color> {
                { "Idle", Color.white },
                { "Evade", Color.magenta },
                { "Chase", Color.red },
                { "Attack", Color.yellow },
                { "Path", Color.cyan }
            };
            Gizmos.color = stateColors[GetState().Name];

            /*
            Gizmos.DrawLine(VehicleInstance.transform.position, Destination);
            Gizmos.DrawSphere(Destination, 2f);
            */
            /*
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(VehicleInstance.transform.position, SightRange);

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(VehicleInstance.transform.position, AttackRange);
            */
            //Gizmos.color = Color.green;
            Gizmos.DrawLine(VehicleInstance.transform.position, VehicleInstance.transform.position + VehicleInstance.transform.forward * 100f);
        }
    }
}
