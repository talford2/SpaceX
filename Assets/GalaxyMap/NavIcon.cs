using UnityEngine;

public class NavIcon : MonoBehaviour
{
    public Transform DestinationPoint;
    public Transform Pin;

    public string SceneName;
    public int LevelIndex;

    public void OnClick()
    {
        NavShipController.Current.SetDestination(DestinationPoint.position);
    }

    private void Update()
    {
        Pin.Rotate(Vector3.up, 90f * Time.deltaTime);
    }
}
