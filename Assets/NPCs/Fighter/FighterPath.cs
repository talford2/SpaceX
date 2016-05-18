using System;
using UnityEngine;

public class FighterPath : NpcState<Fighter>
{
	public FighterPath(Fighter npc) : base(npc)
	{
		Name = "Path";
		Npc.VehicleInstance.IgnoreCollisions = true;
	}

	public Vector3 GetSteerForce(Vector3 targetDestination)
	{
		var steerForce = Vector3.zero;

		steerForce += 0.2f * Npc.Steering.GetSeekForce(targetDestination);
		if (steerForce.sqrMagnitude > 1f)
			return steerForce.normalized;

		return steerForce.normalized;
	}

	public override void Initialize()
	{
		throw new NotImplementedException();
	}

	public override void Update()
	{
		var pathDestination = Universe.Current.GetWorldPosition(Npc.PathDestination);
		var toDestination = pathDestination - Npc.VehicleInstance.transform.position;
		if (toDestination.sqrMagnitude < 25f)
		{
			Npc.VehicleInstance.TriggerAccelerate = false;
		}
		else
		{
			Npc.VehicleInstance.TriggerAccelerate = true;
		}
		if (toDestination.sqrMagnitude < 9f)
		{
			Npc.VehicleInstance.IgnoreCollisions = false;
			Npc.SetState(Npc.Idle);
			return;
		}
		Npc.Destination = Npc.VehicleInstance.transform.position + GetSteerForce(pathDestination);
	}
}
