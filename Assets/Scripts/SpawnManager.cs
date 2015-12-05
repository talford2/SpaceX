using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpawnManager
{
    private static List<PlayerSpawner> _spawners;

    public static void AddSpawner(PlayerSpawner spawner)
    {
        if (_spawners==null)
            _spawners = new List<PlayerSpawner>();
        if (!_spawners.Contains(spawner))
            _spawners.Add(spawner);
    }

    public static void RemoveSpawner(PlayerSpawner spawner)
    {
        _spawners.Remove(spawner);
    }

    public static PlayerSpawner FindNearest(UniversePosition universePosition)
    {
        Debug.Log("FIND SPAWNER NEAR: " + universePosition.CellIndex);
        var nearestSpawner = _spawners
            .OrderBy(s => (s.Shiftable.CellLocalPosition - universePosition.CellLocalPosition).sqrMagnitude)
            .OrderBy(s => (s.Shiftable.UniverseCellIndex - universePosition.CellIndex).SquareMagnitude()).FirstOrDefault();

        Debug.Log("NEAREST SPAWNER AT: " + nearestSpawner.Shiftable.UniverseCellIndex);
        return nearestSpawner;
    }
}
