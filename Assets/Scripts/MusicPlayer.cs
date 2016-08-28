using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public List<AudioSource> MusicList;
    private int _currentIndex = 0;

    void Start()
    {
        MusicList.First().Play();
        _currentIndex = 0;
    }

    void Update()
    {
        if (!MusicList[_currentIndex].isPlaying)
        {
            _currentIndex++;
            if (_currentIndex >= MusicList.Count)
            {
                _currentIndex = 0;
            }
            MusicList[_currentIndex].Play();
        }
    }
}
