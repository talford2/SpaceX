using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    public PirateSquad EnemySquadron;

    private void Awake()
    {
        EnemySquadron.OnAllDied += TriggerMissionComplete;
    }

    private void TriggerMissionComplete()
    {
        Mission.Current.TriggerFinish(5f);
    }
}
