using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Targeting
{
    private static Dictionary<Team, List<Transform>> targetables;

    public static void AddTargetable(Team team, Transform transform)
    {
        if (targetables == null)
            targetables = new Dictionary<Team, List<Transform>>();
        if (!targetables.ContainsKey(team))
            targetables.Add(team, new List<Transform>());
        targetables[team].Add(transform);
    }

    public static void RemoveTargetable(Team team, Transform transform)
    {
        if (targetables.ContainsKey(team))
            targetables[team].Remove(transform);
    }

    public static Team GetEnemyTeam(Team team)
    {
        if (team == Team.Good)
            return Team.Bad;
        return Team.Good;
    }

    public static Transform FindFacingAngle(Team team, Vector3 fromPosition, Vector3 facing, float maxDistance)
    {
        Transform target = null;
        var targetCandidates = targetables[team];

        if (targetCandidates.Any())
        {
            var smallestAngle = float.PositiveInfinity;
            foreach (var candidate in targetCandidates)
            {
                var toTarget = candidate.position - fromPosition;
                if (toTarget.sqrMagnitude < maxDistance * maxDistance)
                {
                    // Exclude targets that are behind the missile
                    if (Vector3.Dot(candidate.position - fromPosition, facing) > 0f)
                    {
                        // Choose target based smallest angle to
                        var angleTo = Mathf.Abs(Vector3.Angle(facing, candidate.position - fromPosition));
                        if (angleTo < smallestAngle)
                        {
                            smallestAngle = Mathf.Abs(angleTo);
                            target = candidate;
                        }
                    }
                }
            }
        }
        return target;
    }
}
