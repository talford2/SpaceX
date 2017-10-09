using UnityEngine;

public class StationTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var detectable = other.GetComponent<Detectable>();
        if (detectable != null)
        {
            if (Player.Current.VehicleInstance != null)
            {
                if (Player.Current.VehicleInstance.transform == detectable.TargetTransform)
                {
                    ShipProfileScreen.Current.Fighters.Clear();
                    foreach (var member in Player.Current.Squadron.Members)
                    {
                        ShipProfileScreen.Current.Fighters.Add(member);
                    }
                    ShipProfileScreen.Current.Populate(Player.Current.Squadron.GetCurrentIndex());
                    ShipProfileScreen.Current.Show();
                }
            }
        }
    }
}
