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

    public override void Update()
    {
        if (Npc.Target == null)
        {
            Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
            Npc.State = new FighterIdle(Npc);
            return;
        }

        CheckSensors();

        var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;

        if (toTarget.sqrMagnitude > Npc.AttackRange * Npc.AttackRange)
        {
            Npc.State = new FighterChase(Npc);
            return;
        }

        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);

        var targetDestination = Npc.Target.position;
        Npc.Destination = Vector3.Lerp(Npc.Destination, targetDestination, Time.deltaTime) + Npc.Steering.GetSeparationForce(neighbors);

        var pitchYaw = Npc.GetPitchYawToPoint(Npc.Destination);

        Npc.VehicleInstance.YawThrottle = pitchYaw.y * Time.deltaTime;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x * Time.deltaTime;

        Npc.VehicleInstance.TriggerBrake = false;
        Npc.VehicleInstance.TriggerAccelerate = false;

        if (burstCooldown >= 0f)
        {
            burstCooldown -= Time.deltaTime;
            if (burstCooldown < 0f)
            {
                allowShoot = true;
                burstAmount = 0f;
                burstTimeoffset = Random.Range(-0.2f, 0.2f);
            }
        }

        if (dotTarget > 10f)
        {
            if (allowShoot)
            {
                var angleToTarget = Vector3.Angle(toTarget.normalized, Npc.VehicleInstance.transform.forward.normalized);
                if (Mathf.Abs(angleToTarget) < Npc.ShootAngleTolerance)
                {
                    // Rough Extrapolation
                    var targetVehicle = Npc.Target.GetComponent<Vehicle>();
                    if (targetVehicle != null)
                    {
                        var extrapolatePosition = Utility.GetVehicleExtrapolatedPosition(Npc.Target.GetComponent<Vehicle>(), Npc.VehicleInstance.PrimaryWeaponInstance, burstTimeoffset);
                        Npc.VehicleInstance.SetAimAt(extrapolatePosition);
                    }
                    else
                    {
                        Npc.VehicleInstance.SetAimAt(Npc.Target.position);
                    }
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
