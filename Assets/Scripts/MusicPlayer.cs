using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource Source;
    public List<AudioClip> MusicClips;

    private static MusicPlayer _current;
    public static MusicPlayer Current { get { return _current; } }

    private int _currentIndex = 0;

    private float _initialVolume;
    private bool _isFadeOut;
    private float _fadeOutDuration;
    private float _fadeOutCooldown;

    private void Awake()
    {
        _current = this;
    }

    private void Start()
    {
        _currentIndex = 0;
        Source.clip = MusicClips[_currentIndex];
        Source.Play();
    }

    private void Update()
    {
        if (!Source.isPlaying)
        {
            _currentIndex++;
            if (_currentIndex >= MusicClips.Count)
            {
                _currentIndex = 0;
            }
            Source.clip = MusicClips[_currentIndex];
            Source.Play();
        }
        if (_isFadeOut)
        {
            if (_fadeOutCooldown >= 0f)
            {
                _fadeOutCooldown -= Time.deltaTime;
                var fraction = Mathf.Clamp01(_fadeOutCooldown / _fadeOutDuration);
                Source.volume = fraction * _initialVolume;
                if (_fadeOutCooldown < 0f)
                    Source.Stop();
            }
        }
    }

    public void TriggerFadeOut(float time)
    {
        _fadeOutDuration = time;
        _fadeOutCooldown = _fadeOutDuration;
        _initialVolume = Source.volume;
        _isFadeOut = true;
    }
}
