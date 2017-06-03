using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    public bool DebugBuildOnly = true;
    private GUIStyle displayStyle;
    
    private float initTime;
    private float minutes;
    private float seconds;

    private void Awake()
    {
        if (DebugBuildOnly)
        {
            if (!Debug.isDebugBuild)
                Destroy(gameObject);
        }
        displayStyle = new GUIStyle()
        {
            fontSize = 30,
            alignment = TextAnchor.UpperRight,
            normal = new GUIStyleState { textColor = Color.white }
        };
        initTime = Time.time;
    }

    private void Update()
    {
        var sceneTime = Time.time - initTime;
        minutes = Mathf.Floor(sceneTime / 60f);
        seconds = sceneTime - (minutes * 60f);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 200, 60f, 180f, 50f), string.Format("{0:f0}:{1:00}", minutes, seconds), displayStyle);
    }
}
