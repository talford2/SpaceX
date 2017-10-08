using System.Collections.Generic;

public class SpawnerGroup : Triggerable
{
    public List<ProximitySpawner> Spawners;

    public override void Trigger(float delay = 0)
    {
        foreach (var spawner in Spawners)
        {
            spawner.IsTriggered = true;
        }
    }
}
