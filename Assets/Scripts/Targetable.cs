using UnityEngine;

public class Targetable : MonoBehaviour
{
    public Team Team;

    public Transform LockedOnBy;

    private void Start()
    {
        Targeting.AddTargetable(Team, transform);
    }

    private void OnDestroy()
    {
        Targeting.RemoveTargetable(Team, transform);
    }

    public void SetEnabled(bool value)
    {
        if (value)
        {
            Targeting.AddTargetable(Team, transform);
        }
        else
        {
            Targeting.RemoveTargetable(Team, transform);
        }
    }
}
