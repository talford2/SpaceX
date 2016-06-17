using System.Collections;
using UnityEngine;

public class AnonymousSound : MonoBehaviour
{
    public AudioSource Source;

    private ResourcePoolItem _resourcePoolItem;

    private void Start()
    {
        if (_resourcePoolItem == null)
            _resourcePoolItem = GetComponent<ResourcePoolItem>();
        _resourcePoolItem.IsAvailable = true;
    }

    public void PlayAt(AudioClip clip, Vector3 position, float volume = 1f, bool isSpatial = true)
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
