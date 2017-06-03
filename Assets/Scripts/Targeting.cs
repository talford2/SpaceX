using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Targeting
{
    private static Dictionary<Team, List<Transform>> _targetables;
    private static List<Transform> _allTargetables;

    public static void AddTargetable(Team team, Transform transform)
    {
        if (_targetables == null)
        {
            _targetables = new Dictionary<Team, List<Transform>>();
            _allTargetables = new List<Transform>();
        }
        if (!_targetables.ContainsKey(team))
            _targetables.Add(team, new List<Transform>());
        _targetables[team].Add(transform);
        _allTargetables.Add(transform);
    }

    public static void RemoveTargetable(Team team, Transform transform)
    {
        if (_targetables.ContainsKey(team))
        {
            _targetables[team].Remove(transform);
            _allTargetables.Remove(transform);
        }
    }

    public static Team GetEnemyTeam(Team team)
    {
        if (team == Team.Good)
            return Team.Bad;
        return Team.Good;
    }

    public static Transform FindNearestTeam(Team team, Vector3 fromPosition, float maxDistance)
    {
        Transform target = null;
        if (_targetables.ContainsKey(team))
        {
            var targetCandidates = _targetables[team];
            var smallestDistanceSquared = Mathf.Infinity;
            if (targetCandidates.Any())
            {
                foreach (var candidate in targetCandidates)
                {
                    if (candidate != null)
                    {
                        var toTarget = candidate.position - fromPosition;
                        if (toTarget.sqrMagnitude < maxDistance*maxDistance)
                        {
                            if (toTarget.sqrMagnitude < smallestDistanceSquared)
                            {
                                smallestDistanceSquared = toTarget.sqrMagnitude;
                                target = candidate;
                            }
                        }
                    }
                }
            }
            return target;
        }
        return null;
    }

    public static Transform FindFacingAngleTeam(Team team, Vector3 fromPosition, Vector3 facing, float maxDistance, float angleTolerance = 90f)
    {
        if (_targetables.ContainsKey(team))
        {
            var targetCandidates = _targetables[team];
            return FindFacingAngleTarget(targetCandidates, fromPosition, facing, maxDistance, angleTolerance);
        }
        return null;
    }

    public static Transform FindFacingAngleAny(Vector3 fromPosition, Vector3 facing, float maxDistance, float angleTolerance)
    {
        var targetCandidates = _allTargetables;
        return FindFacingAngleTarget(targetCandidates, fromPosition, facing, maxDistance, angleTolerance);
    }

    private static Transform FindFacingAngleTarget(List<Transform> targetCandidates, Vector3 fromPosition, Vector3 facing, float maxDistance, float angleTolerance)
    {
        Transform target = null;
        if (targetCandidates.Any())
        {
            var smallestAngle = angleTolerance;
            foreach (var candidate in targetCandidates)
            {
                if (candidate != null)
                {
                    var toTarget = candidate.position - fromPosition;
                    if (toTarget.sqrMagnitude < maxDistance*maxDistance)
                    {
                        // Exclude targets that are behind the missile
                        if (Vector3.Dot(toTarget, facing) > 0f)
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
        }
        return target;
    }
}
