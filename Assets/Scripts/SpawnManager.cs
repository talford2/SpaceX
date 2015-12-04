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
    }

    public static void RemoveSpawner(PlayerSpawner spawner)
    {
        _spawners.Remove(spawner);
    }

    public static PlayerSpawner FindNearest(UniversePosition universePosition)
    {
        var smallestCellDistance = int.MaxValue;
        var smallestLocalDistance = float.PositiveInfinity;
        PlayerSpawner nearestSpawner = null;
        foreach (var spawner in _spawners)
        {
            var cellDistanceSquared = (spawner.Shiftable.UniverseCellIndex - universePosition.CellIndex).SquareMagnitude();
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
        return nearestSpawner;
    }
}
