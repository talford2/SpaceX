using UnityEngine;

public class FrameRateDisplay : MonoBehaviour
{
    private float fps;
    private float fpsFrameSum;
    private float fpsTimeSum;
    private float fpsTime = 0.5f;
    private float fpsCooldown;

    private GUIStyle frameRateStyle;

    private void Awake()
    {
        frameRateStyle = new GUIStyle()
        {
            fontSize = 30,
            alignment = TextAnchor.UpperRight,
            normal = new GUIStyleState { textColor = Color.white }
        };
    }

    private void Update()
    {
        if (fpsCooldown >= 0f)
        {
            fpsCooldown -= Time.deltaTime;
            fpsFrameSum++;
            fpsTimeSum += Time.deltaTime;
            if (fpsCooldown < 0f)
            {
                fps = fpsFrameSum / fpsTimeSum;
                fpsCooldown = fpsTime;
                fpsFrameSum = 0f;
                fpsTimeSum = 0f;
            }
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 200, 20f, 180f, 50f), string.Format("{0:f1} FPS", fps), frameRateStyle);
    }
}
