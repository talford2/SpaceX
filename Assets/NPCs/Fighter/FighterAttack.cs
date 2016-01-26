using System.Collections.Generic;
using UnityEngine;

public class FighterAttack :NpcState<Fighter>
{
    // Neighbors
    private float neighborDetectInterval = 0.2f;
    private float neighborDetectCooldown;
    private List<Transform> neighbors;

    private bool allowShoot;
    private float burstCooldown;
    private float burstAmount;
    private float burstTimeoffset;

    private Vector3 targetOffset;

    public FighterAttack(Fighter npc) : base(npc)
    {
        Name = "Attack";
        allowShoot = true;
    }

    private void CheckSensors()
    {
        if (neighborDetectCooldown >= 0f)
        {
            neighbors = new List<Transform>();
            neighborDetectCooldown -= Time.deltaTime;
            if (neighborDetectCooldown < 0f)
            {
                Npc.ProximitySensor.Detect(DetectNeighbor);
                neighborDetectCooldown = neighborDetectInterval;
            }
        }
    }

    public Vector3 GetSteerForce(Vector3 targetDestination)
    {
        var steerForce = Vector3.zero;

        steerForce += 0.8f * Npc.Steering.GetSeparationForce(neighbors);
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
            Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
            Npc.State = new FighterIdle(Npc);
            return;
        }

        var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;

        if (toTarget.sqrMagnitude > Npc.AttackRange * Npc.AttackRange)
        {
            Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
            Npc.State = new FighterChase(Npc);
            return;
        }

        CheckSensors();

        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);

        Vector3 targetDestination;
        var targetVehicle = Npc.Target.GetComponent<Vehicle>();
        if (targetVehicle != null)
        {
            var extrapolatePosition = Utility.GetVehicleExtrapolatedPosition(Npc.Target.GetComponent<Vehicle>(), Npc.VehicleInstance.PrimaryWeaponInstance, burstTimeoffset);
            targetDestination = extrapolatePosition;
        }
        else
        {
            targetDestination = Npc.Target.position;
        }

        if (burstCooldown >= 0f)
        {
            burstCooldown -= Time.deltaTime;
            if (burstCooldown < 0f)
            {
                allowShoot = true;
                burstAmount = 0f;
                burstTimeoffset = Random.Range(-Npc.ExrapolationTimeError, Npc.ExrapolationTimeError);
                targetOffset = Random.insideUnitSphere*3f;
            }
        }

        Npc.Destination = Npc.VehicleInstance.transform.position + GetSteerForce(targetDestination + targetOffset);

        Npc.VehicleInstance.TriggerBrake = false;
        Npc.VehicleInstance.TriggerAccelerate = false;

        if (dotTarget > 10f)
        {
            if (allowShoot)
            {
                var angleToTarget = Vector3.Angle(toTarget.normalized, Npc.VehicleInstance.transform.forward.normalized);
                if (Mathf.Abs(angleToTarget) < Npc.ShootAngleTolerance)
                {
                    Npc.VehicleInstance.SetAimAt(Npc.VehicleInstance.transform.position + Npc.VehicleInstance.transform.forward*100f);
                    
                    Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = true;
                    burstAmount += Time.deltaTime;
                    if (burstAmount > Npc.BurstTime)
                    {
                        burstCooldown = Npc.BurstWaitTime;
                        allowShoot = false;
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

            if (toTarget.sqrMagnitude < Npc.OvertakeDistance * Npc.OvertakeDistance)
            {
                Debug.Log("OVERTAKE!");
                Npc.VehicleInstance.TriggerAccelerate = true;
                Npc.State = new FighterEvade(Npc);
            }
        }
        else
        {
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
            if (!neighbors.Contains(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
    }
}
