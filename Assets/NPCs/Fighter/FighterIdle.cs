using System;
using System.Collections.Generic;
using UnityEngine;

public class FighterIdle : NpcState<Fighter>
{
	private float _targetSearchInterval = 3f;
	private float _targetSearchCooldown;

	// Neighbors
	private float _neighborDetectInterval = 0.2f;
	private float _neighborDetectCooldown;
	private List<Transform> _neighbors;

	public FighterIdle(Fighter npc) : base(npc)
	{
		Name = "Idle";
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

	public override void Update()
	{
		CheckSensors();

		Npc.VehicleInstance.RollThrottle = 0f;

		if (_targetSearchCooldown >= 0f)
		{
			_targetSearchCooldown -= Time.deltaTime;
			if (_targetSearchCooldown < 0f)
			{
				Npc.Target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
				_targetSearchCooldown = _targetSearchInterval;
				if (Npc.Target != null)
				{
					Npc.SetState(Npc.Chase);
					return;
				}
				Npc.Target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, -Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
				if (Npc.Target != null)
				{
					var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;
					var dotTargetFacing = Vector3.Dot(toTarget, Npc.Target.forward);
					if (dotTargetFacing > 0f)
					{
						// Target isn't looking at me!
						Npc.SetState(Npc.Chase);
						return;
					}

					Npc.SetState(Npc.Evade);
					return;
				}
			}
		}

		// Steering stuff
		var immediateDestination = Npc.VehicleInstance.transform.position + Npc.VehicleInstance.transform.forward * 5f + Npc.Steering.GetSeparationForce(_neighbors);
		if (Npc.IsFollowIdleDestination)
		{
			//Debug.Log("FOLLOWING FORMATION!");
			var steeringSeekForce = 5f * Npc.Steering.GetSeekForce(Npc.IdleDestination);
			var seekForce = 5f * (Npc.IdleDestination - Npc.VehicleInstance.transform.position).normalized;
			//Debug.Log("LOCAL CALC POS: " + Npc.VehicleInstance.transform.position);

			if ((steeringSeekForce - seekForce).sqrMagnitude > 0f)
			{
				//Debug.Log("SCHRODINGER? - " + (steeringSeekForce - seekForce).sqrMagnitude);
			}

			Debug.DrawLine(Npc.VehicleInstance.transform.position, Npc.IdleDestination, Color.yellow);
			Debug.DrawLine(Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.position + seekForce.normalized * 50f, new Color(1f, 0.5f, 0f));

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
	}

	private float GetSlowdownDistance(float currentSpeed, float destinationSpeed, float deceleration)
	{
		return GetSlowdownDistance2(currentSpeed * currentSpeed, destinationSpeed, deceleration);
	}

	private float GetSlowdownDistance2(float currentSpeedSquared, float destinationSpeed, float deceleration)
	{
		return (currentSpeedSquared - destinationSpeed * destinationSpeed) / (2f * deceleration);
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
		throw new NotImplementedException();
	}
}
