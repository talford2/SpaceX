using UnityEngine;

public class StationTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var detectable = other.GetComponent<Detectable>();
        if (detectable != null)
        {
            if (PlayerController.Current.VehicleInstance != null)
            {
                if (PlayerController.Current.VehicleInstance.transform == detectable.TargetTransform)
                {
                    ShipProfileScreen.Current.Fighters.Clear();
                    foreach (var member in PlayerController.Current.Squadron.Members)
                    {
                        ShipProfileScreen.Current.Fighters.Add(member);
                    }
                    ShipProfileScreen.Current.Populate(PlayerController.Current.Squadron.GetCurrentIndex());
                    ShipProfileScreen.Current.Show();
                }
            }
        }
    }
}
