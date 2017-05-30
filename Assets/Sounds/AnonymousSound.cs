using System.Collections;
using UnityEngine;

public class AnonymousSound : MonoBehaviour
{
    public AudioSource Source;

    private ResourcePoolItem _resourcePoolItem;
    private float _defaultMinDistance;
    private float _defaultMaxDistance;

    private void Awake()
    {
        _defaultMinDistance = Source.minDistance;
        _defaultMaxDistance = Source.maxDistance;
    }

    private void Start()
    {
        if (_resourcePoolItem == null)
            _resourcePoolItem = GetComponent<ResourcePoolItem>();
        _resourcePoolItem.IsAvailable = true;
    }

    public void PlayAt(AudioClip clip, Vector3 position, float volume = 1f, bool isSpatial = true)
    {
        Source.minDistance = _defaultMinDistance;
        Source.maxDistance = _defaultMaxDistance;
        PlaySound(clip, position, volume, isSpatial);
    }

    public void PlayAt(AudioClip clip, Vector3 position, float minDistance, float maxDistance, float volume = 1f, bool isSpatial = true)
    {
        Source.minDistance = minDistance;
        Source.maxDistance = maxDistance;
        PlaySound(clip, position, volume, isSpatial);
    }

    private void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, bool isSpatial = true)
    {
        Source.spatialize = isSpatial;
        if (_resourcePoolItem == null)
            _resourcePoolItem = GetComponent<ResourcePoolItem>();
        _resourcePoolItem.IsAvailable = false;
        Source.clip = clip;
        Source.volume = volume;
        transform.position = position;
        Source.Play();
        StartCoroutine(StopSound(clip.length + 0.1f));
    }

    private IEnumerator StopSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        Source.Stop();
        _resourcePoolItem.IsAvailable = true;
    }
}
