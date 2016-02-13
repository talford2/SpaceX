using System.Collections.Generic;
using UnityEngine;

public class FighterChase : NpcState<Fighter>
{
    // Neighbors
    private float _neighborDetectInterval = 0.2f;
    private float _neighborDetectCooldown;
    private List<Transform> _neighbors;

    // Target Reconsider
    private float _reconsiderTargetCooldown;
    private float _reconsiderTargetInterval = 3f;

	public FighterChase(Fighter npc) : base(npc)
	{
		Name = "Chase";
        _reconsiderTargetCooldown = _reconsiderTargetInterval;
	    Npc.OnVehicleDamage = OnVehicleDamage;
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

        steerForce += 0.8f*Npc.Steering.GetSeparationForce(_neighbors);
        if (steerForce.sqrMagnitude > 1f)
            return steerForce.normalized;

        steerForce += 0.2f*Npc.Steering.GetSeekForce(targetDestination);
        if (steerForce.sqrMagnitude > 1f)
            return steerForce.normalized;

        return steerForce.normalized;
    }

    public override void Update()
    {
        if (Npc.Target == null)
        {
            Npc.State = new FighterIdle(Npc);
            return;
        }
        CheckSensors();

        var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;
        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);

        Npc.Destination = Npc.VehicleInstance.transform.position + GetSteerForce(Npc.Target.position);

        Npc.VehicleInstance.TriggerBrake = false;
        Npc.VehicleInstance.TriggerAccelerate = false;

        if (dotTarget > 0f)
        {
            if (toTarget.sqrMagnitude > Npc.SightRange * Npc.SightRange)
            {
                Npc.VehicleInstance.TriggerAccelerate = true;
            }
            if (toTarget.sqrMagnitude < Npc.AttackRange * Npc.AttackRange)
            {
                Npc.State = new FighterAttack(Npc);
                return;
            }
        }
        else
        {
            if (toTarget.sqrMagnitude < Npc.EvadeDistance * Npc.EvadeDistance)
            {
                var dotTargetFacing = Vector3.Dot(toTarget, Npc.Target.forward);
                if (dotTargetFacing > 0f)
                {
                    // Target isn't looking at me!
                    Npc.State = new FighterChase(Npc);
                    return;
                }

                Npc.State = new FighterEvade(Npc);
                return;
            }
        }

        // Reconsider Target
        if (_reconsiderTargetCooldown >= 0f)
        {
            _reconsiderTargetCooldown -= Time.deltaTime;
            if (_reconsiderTargetCooldown < 0f)
            {
                Npc.Target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.forward, 500f);
                _reconsiderTargetCooldown = _reconsiderTargetInterval;
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

    private void OnVehicleDamage(Transform attacker)
    {
        Npc.Target = attacker;
    }
}
