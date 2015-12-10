using System.Collections.Generic;
using UnityEngine;

public class FighterIdle : NpcState<Fighter>
{
    private float targetSearchInterval = 3f;
    private float targetSearchCooldown;

    // Neighbors
    private float neighborDetectInterval = 0.2f;
    private float neighborDetectCooldown;
    private List<Transform> neighbors;

    public FighterIdle(Fighter npc) : base(npc)
    {
        Name = "Idle";
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
        CheckSensors();

        Npc.VehicleInstance.RollThrottle = 0f;

        if (targetSearchCooldown >= 0f)
        {
            targetSearchCooldown -= Time.deltaTime;
            if (targetSearchCooldown < 0f)
            {
                Npc.Target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
                targetSearchCooldown = targetSearchInterval;
                if (Npc.Target != null)
                {
                    Npc.State = new FighterChase(Npc);
                }
                else
                {
                    Npc.Target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, -Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
                    if (Npc.Target != null)
                    {
                        Npc.State = new FighterEvade(Npc);
                    }
                }
            }
        }

        // Steering stuff
        var immediateDestination = Npc.VehicleInstance.transform.position + Npc.VehicleInstance.transform.forward*5f + Npc.Steering.GetSeparationForce(neighbors);
        if (Npc.IsFollowIdleDestination)
        {
            //Debug.Log("FOLLOWING FORMATION!");
            var steeringSeekForce = 5f*Npc.Steering.GetSeekForce(Npc.IdleDestination);
            var seekForce = 5f*(Npc.IdleDestination - Npc.VehicleInstance.transform.position).normalized;
            //Debug.Log("LOCAL CALC POS: " + Npc.VehicleInstance.transform.position);

            if ((steeringSeekForce - seekForce).sqrMagnitude > 0f)
            {
                //Debug.Log("SCHRODINGER? - " + (steeringSeekForce - seekForce).sqrMagnitude);
            }

            Debug.DrawLine(Npc.VehicleInstance.transform.position, Npc.IdleDestination, Color.yellow);
            Debug.DrawLine(Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.position + seekForce.normalized*50f, new Color(1f, 0.5f, 0f));

            immediateDestination = Npc.VehicleInstance.transform.position + seekForce; // + Npc.Steering.GetSeparationForce(neighbors);

            // Control brakes, acceleration and boost

            // Roll to maintain orientation
            var dotUpRightDestination = Vector3.Dot(Npc.IdleUpDestination, Npc.VehicleInstance.transform.right);
            if (Mathf.Abs(dotUpRightDestination) > 0.1f)
            {
                Npc.VehicleInstance.RollThrottle = dotUpRightDestination;
            }
            else
            {
                Npc.VehicleInstance.RollThrottle = 0f;
            }
        }
        else
        {
            immediateDestination = Universe.Current.ViewPort.Shiftable.GetWorldPosition();
        }

        var dotDestination = Vector3.Dot(Npc.IdleDestination - Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.forward);
        Npc.VehicleInstance.TriggerBrake = false;
        Npc.VehicleInstance.TriggerAccelerate = false;
        Npc.VehicleInstance.TriggerBoost = false;

        var brakeToIdleDistance = GetSlowdownDistance2(Npc.VehicleInstance.GetVelocity().sqrMagnitude, Npc.VehicleInstance.IdleSpeed, Npc.VehicleInstance.Brake);
        if (dotDestination < brakeToIdleDistance)
        {
            // Not facing 
            Npc.VehicleInstance.TriggerBrake = true;
        }
        if (dotDestination > brakeToIdleDistance + 15f)
        {
            //Debug.Log("I SHOULD CATCH UP?");
            Npc.VehicleInstance.TriggerAccelerate = true;

            // Slow down Distance
            var boostBrakingToIdleDistance = GetSlowdownDistance(Npc.VehicleInstance.MaxBoostSpeed, Npc.VehicleInstance.IdleSpeed, Npc.VehicleInstance.BoostBrake);

            if (dotDestination > boostBrakingToIdleDistance)
            {
                Npc.VehicleInstance.TriggerBoost = true;
                //Debug.Log("BOOST TO CATCH UP!");
            }
        }

        Npc.Destination = immediateDestination;
        var pitchYaw = Npc.GetPitchYawToPoint(Npc.Destination);
        Npc.VehicleInstance.YawThrottle = pitchYaw.y*Time.deltaTime;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x*Time.deltaTime;
    }


    private float GetSlowdownDistance(float currentSpeed, float destinationSpeed, float deceleration)
    {
        return GetSlowdownDistance2(currentSpeed*currentSpeed, destinationSpeed, deceleration);
    }

    private float GetSlowdownDistance2(float currentSpeedSquared, float destinationSpeed, float deceleration)
    {
        return (currentSpeedSquared - destinationSpeed*destinationSpeed)/(2f*deceleration);
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
