using UnityEngine;
using System.Collections;

public class ResourceReseter : MonoBehaviour {

    private ResourcePoolItem _resourcePoolItem;

    public float Cooldown;

    public bool StartOn;

    public void StartCooldown()
    {
        StartOn = true;
    }

    private void Awake()
    {
        _resourcePoolItem = GetComponent<ResourcePoolItem>();
    }

    private void Update()
    {
        if (StartOn)
        {
            if (Cooldown >= 0)
            {
                Cooldown -= Time.deltaTime;
            }

            if (Cooldown < 0)
            {
                _resourcePoolItem.IsAvailable = true;
            }
        }
    }
}
