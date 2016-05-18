using System.Collections.Generic;
using UnityEngine;

public class FighterEvade : NpcState<Fighter>
{
	private float _evadeTimeout = 2f;
	private float _evadeCooldown;

	// Dodge
	private float _dodgeCooldown;
	private Vector3 _dodgeOffset;

	// Neighbors
	private float _neighborDetectInterval = 0.2f;
	private float _neighborDetectCooldown;
	private List<Transform> _neighbors;

	// Target Reconsider
	private float _reconsiderTargetCooldown;
	private float _reconsiderTargetInterval = 3f;

	public FighterEvade(Fighter npc) : base(npc)
	{
		Name = "Evade";
		_reconsiderTargetCooldown = _reconsiderTargetInterval;
		Npc.OnVehicleDamage = OnVehicleDamage;
	}

	private Quaternion GetRandomArc(float angle)
	{
		var halfAngle = angle * 0.5f;
		return Quaternion.Euler(Random.Range(-halfAngle, halfAngle), Random.Range(-halfAngle, halfAngle), Random.Range(-halfAngle, halfAngle));
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
			Npc.SetState(Npc.Idle);
			return;
		}
		var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;

		if (_dodgeCooldown >= 0f)
		{
			_dodgeCooldown -= Time.deltaTime;
			if (_dodgeCooldown < 0f)
			{
				_dodgeOffset = GetRandomArc(Npc.DodgeArcAngle) * -toTarget.normalized * Npc.DodgeRadius;
				_dodgeCooldown = Random.Range(Npc.MinDodgeIntervalTime, Npc.MaxDodgeIntervalTime);
			}
		}

		CheckSensors();

		var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);

		var targetDestination = Npc.Target.position - toTarget.normalized * Npc.TurnAroundDistance + _dodgeOffset;
		Npc.Destination = Npc.VehicleInstance.transform.position + GetSteerForce(targetDestination);

		if (toTarget.sqrMagnitude < Npc.AcclerateDistance * Npc.AcclerateDistance)
		{
			Npc.VehicleInstance.TriggerAccelerate = true;
		}
		else
		{
			Npc.VehicleInstance.TriggerAccelerate = false;
		}

		if (dotTarget < 0f)
		{
			if (dotTarget < -Npc.TurnAroundDistance)
			{
				//Debug.Log("TURN AROUND!");
				//Debug.Log("DOT: " + dotTarget);
				Npc.SetState(Npc.Chase);
				return;
			}
			_evadeCooldown = _evadeTimeout;

			var dotTargetFacing = Vector3.Dot(toTarget, Npc.Target.forward);
			if (dotTargetFacing > 0f)
			{
				// Target isn't looking at me!
				Npc.SetState(Npc.Chase);
				return;
			}
		}

		if (_evadeCooldown > 0f)
		{
			_evadeCooldown -= Time.deltaTime;
			if (_evadeCooldown < 0f)
			{
				Npc.SetState(Npc.Chase);
				return;
			}
		}

		// Reconsider Target
		if (_reconsiderTargetCooldown >= 0f)
		{
			_reconsiderTargetCooldown -= Time.deltaTime;
			if (_reconsiderTargetCooldown < 0f)
			{
				Npc.Target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
				_reconsiderTargetCooldown = _reconsiderTargetInterval;
				if (Npc.Target != null)
				{
					Npc.SetState(Npc.Chase);
					return;
				}
				Npc.Target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, -Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
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

	public override void Initialize()
	{
	}
}
