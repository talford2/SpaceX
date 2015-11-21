using System.Collections.Generic;
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
}
