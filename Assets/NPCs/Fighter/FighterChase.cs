using System.Collections.Generic;
using UnityEngine;

public class FighterChase : NpcState<Fighter>
{
	// Neighbors
	private float neighborDetectInterval = 0.2f;
	private float neighborDetectCooldown;
	private List<Transform> neighbors;

	// Target Reconsider
	private float reconsiderTargetCooldown;
	private float reconsiderTargetInterval = 3f;

	public FighterChase(Fighter npc) : base(npc)
	{
		Name = "Chase";
		reconsiderTargetCooldown = reconsiderTargetInterval;
	}

	private void CheckSensors()
	{
		if (neighborDetectCooldown >= 0f)
		{
			neighbors = new List<Transform>();
			neighborDetectCooldown += Time.deltaTime;
			if (neighborDetectCooldown > 0f)
			{
				Npc.ProximitySensor.Detect(DetectNeighbor);
				neighborDetectCooldown = neighborDetectInterval;
			}
		}
	}

	public Vector3 GetSteerForce(Vector3 targetDestination)
	{
		var steerForce = Vector3.zero;

		steerForce += 1.8f * Npc.Steering.GetSeparationForce(neighbors);
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
			Npc.State = new FighterIdle(Npc);
			return;
		}
		var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;
		var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);
		//Debug.Log("DOT PROD: " + dotTarget);

		CheckSensors();

		Npc.Destination = GetSteerForce(Npc.Target.position);

		var pitchYaw = Npc.GetPitchYawToPoint(Npc.Destination);
		if (pitchYaw.sqrMagnitude <= 0f)
		{
			Debug.Log("THIS DOES HAPPEN!");
			// Give random value to resolve zero pitchYaw issue.
			pitchYaw = Random.insideUnitCircle;
		}

		Npc.VehicleInstance.YawThrottle = pitchYaw.y * Time.deltaTime;
		Npc.VehicleInstance.PitchThotttle = pitchYaw.x * Time.deltaTime;

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
		if (reconsiderTargetCooldown >= 0f)
		{
			reconsiderTargetCooldown -= Time.deltaTime;
			if (reconsiderTargetCooldown < 0f)
			{
				Npc.Target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
				reconsiderTargetCooldown = reconsiderTargetInterval;
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
			if (!neighbors.Contains(neighbor))
			{
				neighbors.Add(neighbor);
			}
		}
	}
}
