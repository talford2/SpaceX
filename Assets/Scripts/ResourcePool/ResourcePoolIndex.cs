using UnityEngine;

public class ResourcePoolIndex : MonoBehaviour
{
    public GameObject AnonymousSoundPrefab;

    private static ResourcePoolIndex _current;

    private void Awake()
    {
        _current = this;
    }

    private static AnonymousSound GetAvailableAnonymousSound()
    {
        return ResourcePoolManager.GetAvailable<AnonymousSound>(_current.AnonymousSoundPrefab, Vector3.zero, Quaternion.identity);
    }

    public static void PlayAnonymousSound(AudioClip clip, Vector3 position, float volume = 1f, bool isSpatial = true)
    {
        GetAvailableAnonymousSound().PlayAt(clip, position, volume, isSpatial);
    }

    public static void PlayAnonymousSound(AudioClip clip, Vector3 position, float minDistance, float maxDistance, float volume = 1f, bool isSpatial = true)
    {
        GetAvailableAnonymousSound().PlayAt(clip, position, minDistance, maxDistance, volume, isSpatial);
    }
}
