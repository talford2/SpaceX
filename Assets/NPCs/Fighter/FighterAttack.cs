using System.Collections.Generic;
using UnityEngine;

public class FighterAttack : NpcState<Fighter>
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

	private Vector3 _steerForce;
	public Vector3 GetSteerForce(Vector3 targetDestination)
	{
		_steerForce = Vector3.zero;

		_steerForce += 0.8f * Npc.Steering.GetSeparationForce(_neighbors);
		if (_steerForce.sqrMagnitude > 1f)
			return _steerForce.normalized;

		_steerForce += 0.2f * Npc.Steering.GetSeekForce(targetDestination);
		if (_steerForce.sqrMagnitude > 1f)
			return _steerForce.normalized;

		return _steerForce.normalized;
	}

	// Performance variables
	private Vector3 _toTarget;
	private float _dotTarget;
	private Vector3 _targetDestination;
	private Vehicle _targetVehicle;
	private float _angleToTarget;
	private float _dotTargetFacing;

	public override void Update()
	{
		if (Npc.Target == null)
		{
			if (Npc.VehicleInstance.PrimaryWeaponInstance != null)
				Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
			Npc.State = new FighterIdle(Npc);
			return;
		}

		_toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;

		if (_toTarget.sqrMagnitude > Npc.AttackRange * Npc.AttackRange)
		{
			if (Npc.VehicleInstance.PrimaryWeaponInstance != null)
				Npc.VehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
			Npc.State = new FighterChase(Npc);
			return;
		}

		CheckSensors();

		_dotTarget = Vector3.Dot(_toTarget, Npc.VehicleInstance.transform.forward);

		_targetVehicle = Npc.Target.GetComponent<Vehicle>();
		if (_targetVehicle != null && Npc.VehicleInstance.PrimaryWeaponInstance != null)
		{
			_targetDestination = Utility.GetVehicleExtrapolatedPosition(Npc.Target.GetComponent<Vehicle>(), Npc.VehicleInstance.PrimaryWeaponInstance, _burstTimeoffset);
		}
		else
		{
			_targetDestination = Npc.Target.position;
		}

		if (_burstCooldown >= 0f)
		{
			_burstCooldown -= Time.deltaTime;
			if (_burstCooldown < 0f)
			{
				_allowShoot = true;
				_burstAmount = 0f;
				_burstTimeoffset = Random.Range(-Npc.ExrapolationTimeError, Npc.ExrapolationTimeError);

				if (_dotTarget > Npc.AttackRange)
				{
					_targetOffset = Random.insideUnitSphere * Npc.MaxAimOffsetRadius;
				}
				else
				{
					_targetOffset = Mathf.Max((_dotTarget / Npc.AttackRange) * Npc.MaxAimOffsetRadius, Npc.MinAimOffsetRadius) * Random.insideUnitSphere;
				}
			}
		}

		Npc.Destination = Npc.VehicleInstance.transform.position + GetSteerForce(_targetDestination + _targetOffset);

		Npc.VehicleInstance.TriggerBrake = false;
		Npc.VehicleInstance.TriggerAccelerate = false;

		if (_dotTarget > 10f)
		{
			if (Npc.VehicleInstance.PrimaryWeaponInstance != null)
			{
				if (_allowShoot)
				{
					_angleToTarget = Vector3.Angle(_toTarget.normalized, Npc.VehicleInstance.transform.forward.normalized);
					if (Mathf.Abs(_angleToTarget) < Npc.ShootAngleTolerance)
					{
						Npc.VehicleInstance.SetAimAt(Npc.VehicleInstance.transform.position + Npc.VehicleInstance.transform.forward * 100f);

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

			if (_toTarget.sqrMagnitude < Npc.OvertakeDistance * Npc.OvertakeDistance)
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

			_dotTargetFacing = Vector3.Dot(_toTarget, Npc.Target.forward);
			if (_dotTargetFacing > 0f)
			{
				// Target isn't looking at me!
				Npc.State = new FighterChase(Npc);
				return;
			}

			if (_dotTarget < 0f)
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
