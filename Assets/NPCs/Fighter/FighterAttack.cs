using System.Collections.Generic;
using UnityEngine;

public class FighterAttack :NpcState<Fighter>
{
    // Neighbors
    private float _neighborDetectInterval = 0.2f;
    private float _neighborDetectCooldown;
    private List<Transform> _neighbors;

    private bool _allowShoot;
    private float _burstCooldown;
    private float _burstAmount;
    private float _burstTimeoffset;

    private Vector3 _targetOffset;

    public FighterAttack(Fighter npc) : base(npc)
    {
        Name = "Attack";
        _allowShoot = true;
    }

    private void CheckSensors()
    {
        if (_neighborDetectCooldown >= 0f)
        {
            _neighbors = new List<Transform>();
            _neighborDetectCooldown -= Time.deltaTime;
            if (_neighborDetectCooldown < 0f)
            {
                Npc.ProximitySensor.Detect(DetectNeighbor);
                _neighborDetectCooldown = _neighborDetectInterval;
            }
        }
    }

    public Vector3 GetSteerForce(Vector3 targetDestination)
    {
        var steerForce = Vector3.zero;

        steerForce += 0.8f * Npc.Steering.GetSeparationForce(_neighbors);
        if (steerForce.sqrMagnitude > 1f)
            return steerForce.normalized;

        steerForce += 0.2f * Npc.Steering.GetSeekForce(targetDestination);
        if (steerForce.sqrMagnitude > 1f)
            return steerForce.normalized;

        return steerForce.normalized;
    }

    public override void Update()
    {
        if (Npc.Target == null)
        {
            if (Npc.VehicleInstance.PrimaryWeaponInstance != null)
                Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
            Npc.State = new FighterIdle(Npc);
            return;
        }

        var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;

        if (toTarget.sqrMagnitude > Npc.AttackRange*Npc.AttackRange)
        {
            if (Npc.VehicleInstance.PrimaryWeaponInstance != null)
                Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
            Npc.State = new FighterChase(Npc);
            return;
        }

        CheckSensors();

        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);

        Vector3 targetDestination;
        var targetVehicle = Npc.Target.GetComponent<Vehicle>();
        if (targetVehicle != null && Npc.VehicleInstance.PrimaryWeaponInstance != null)
        {
            var extrapolatePosition = Utility.GetVehicleExtrapolatedPosition(Npc.Target.GetComponent<Vehicle>(), Npc.VehicleInstance.PrimaryWeaponInstance, _burstTimeoffset);
            targetDestination = extrapolatePosition;
        }
        else
        {
            targetDestination = Npc.Target.position;
        }

        if (_burstCooldown >= 0f)
        {
            _burstCooldown -= Time.deltaTime;
            if (_burstCooldown < 0f)
            {
                _allowShoot = true;
                _burstAmount = 0f;
                _burstTimeoffset = Random.Range(-Npc.ExrapolationTimeError, Npc.ExrapolationTimeError);

                if (dotTarget > Npc.AttackRange)
                {
                    _targetOffset = Random.insideUnitSphere * Npc.MaxAimOffsetRadius;
                }
                else
                {
                    _targetOffset = Mathf.Max((dotTarget/ Npc.AttackRange)* Npc.MaxAimOffsetRadius, Npc.MinAimOffsetRadius)* Random.insideUnitSphere;
                }
            }
        }

        Npc.Destination = Npc.VehicleInstance.transform.position + GetSteerForce(targetDestination + _targetOffset);

        Npc.VehicleInstance.TriggerBrake = false;
        Npc.VehicleInstance.TriggerAccelerate = false;

        if (dotTarget > 10f)
        {
            if (Npc.VehicleInstance.PrimaryWeaponInstance != null)
            {
                if (_allowShoot)
                {
                    var angleToTarget = Vector3.Angle(toTarget.normalized, Npc.VehicleInstance.transform.forward.normalized);
                    if (Mathf.Abs(angleToTarget) < Npc.ShootAngleTolerance)
                    {
                        Npc.VehicleInstance.SetAimAt(Npc.VehicleInstance.transform.position + Npc.VehicleInstance.transform.forward*100f);

                        Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = true;
                        _burstAmount += Time.deltaTime;
                        if (_burstAmount > Npc.BurstTime)
                        {
                            _burstCooldown = Npc.BurstWaitTime;
                            _allowShoot = false;
                        }
                    }
                    else
                    {
                        Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
                    }
                }
                else
                {
                    Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
                }
            }

            if (toTarget.sqrMagnitude < Npc.OvertakeDistance*Npc.OvertakeDistance)
            {
                Debug.Log("OVERTAKE!");
                Npc.VehicleInstance.TriggerAccelerate = true;
                if (Npc.VehicleInstance.PrimaryWeaponInstance != null)
                    Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
                Npc.State = new FighterEvade(Npc);
            }
        }
        else
        {
            if (Npc.VehicleInstance.PrimaryWeaponInstance != null)
                Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;

            var dotTargetFacing = Vector3.Dot(toTarget, Npc.Target.forward);
            if (dotTargetFacing > 0f)
            {
                // Target isn't looking at me!
                Npc.State = new FighterChase(Npc);
                return;
            }

            if (dotTarget < 0f)
                Npc.State = new FighterEvade(Npc);
        }
    }

    private void DetectNeighbor(Transform neighbor)
    {
        if (neighbor != Npc.VehicleInstance.transform)
        {
            if (!_neighbors.Contains(neighbor))
            {
                _neighbors.Add(neighbor);
            }
        }
    }
}
