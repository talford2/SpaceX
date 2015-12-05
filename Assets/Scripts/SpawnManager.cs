using UnityEngine;
using System.Collections.Generic;

public class SpawnManager
{
    private static List<PlayerSpawner> _spawners;

    public static void AddSpawner(PlayerSpawner spawner)
    {
        if (_spawners==null)
            _spawners = new List<PlayerSpawner>();
        if (!_spawners.Contains(spawner))
            _spawners.Add(spawner);
        Debug.Log("ADD SPAWNER AT " + spawner.Shiftable.UniversePosition.CellIndex);
    }

    public static void RemoveSpawner(PlayerSpawner spawner)
    {
        _spawners.Remove(spawner);
    }

    public static PlayerSpawner FindNearest(UniversePosition universePosition)
    {
        Debug.Log("FIND SPAWNER NEAR: " + universePosition.CellIndex);
        var smallestCellDistance = int.MaxValue;
        var smallestLocalDistance = float.PositiveInfinity;
        PlayerSpawner nearestSpawner = null;
        foreach (var spawner in _spawners)
        {
            var cellDistanceSquared = (spawner.Shiftable.UniverseCellIndex - universePosition.CellIndex).SquareMagnitude();
            Debug.Log(spawner.Shiftable.UniverseCellIndex +" CELL DIST: " + cellDistanceSquared);
            if (cellDistanceSquared <= smallestCellDistance)
            {
                smallestCellDistance = cellDistanceSquared;
                var localDistanceSquared = (spawner.Shiftable.CellLocalPosition - universePosition.CellLocalPosition).sqrMagnitude;
                if (localDistanceSquared < smallestLocalDistance)
                {
                    smallestLocalDistance = localDistanceSquared;
                    nearestSpawner = spawner;
                }
            }
        }
        Debug.Log("NEAREST SPAWNER AT: " + nearestSpawner.Shiftable.UniverseCellIndex);
        return nearestSpawner;
    }
}
