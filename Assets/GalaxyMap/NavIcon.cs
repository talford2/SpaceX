using UnityEngine;

public class NavIcon : MonoBehaviour
{
    public Transform DestinationPoint;
    public int LevelIndex;

    public void OnClick()
    {
        NavShipController.Current.SetDestination(DestinationPoint.position);
    }
}
