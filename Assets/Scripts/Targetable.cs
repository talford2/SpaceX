using UnityEngine;

public class Targetable : MonoBehaviour
{
    public Team Team;

    private void Start()
    {
        Targeting.AddTargetable(Team, transform);
    }

    private void OnDestroy()
    {
        Targeting.RemoveTargetable(Team, transform);
    }
}
