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
            .ThenBy(s => (s.Shiftable.UniverseCellIndex - universePosition.CellIndex).SquareMagnitude())
            .FirstOrDefault();

        Debug.LogFormat(
            "NEAREST SPAWNER: {0} @ [{1}, {2}, {3}] - ({4:f2}, {5:f2}, {6:f2})",
            nearestSpawner,
            nearestSpawner.Shiftable.UniverseCellIndex.X,
            nearestSpawner.Shiftable.UniverseCellIndex.Y,
            nearestSpawner.Shiftable.UniverseCellIndex.Z,
            nearestSpawner.Shiftable.CellLocalPosition.x,
            nearestSpawner.Shiftable.CellLocalPosition.y,
            nearestSpawner.Shiftable.CellLocalPosition.z
            );
        return nearestSpawner;
    }
}
