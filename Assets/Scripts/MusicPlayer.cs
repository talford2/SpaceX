using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource Source;
    public List<AudioClip> MusicClips;
    private int _currentIndex = 0;

    void Start()
    {
        _currentIndex = 0;
        Source.clip = MusicClips[_currentIndex];
        Source.Play();
    }

    void Update()
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
    }
}
