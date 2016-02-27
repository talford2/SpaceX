using UnityEngine;
using UnityEngine.UI;

public class SquadronTracker : VehicleTracker
{
    public Font LabelFont;
    public string CallSign;
    private Text _callsignInstance;

    public override Image CreateInstance()
    {
        var instance = base.CreateInstance();

        // Call Sign Label
        var callsignObj = new GameObject(string.Format("{0}_CallSign", transform.name));
        var callSignText = callsignObj.AddComponent<Text>();
        callSignText.color = Color.white;
        callSignText.fontSize = 15;
        callSignText.font = LabelFont;
        callSignText.alignment = TextAnchor.MiddleCenter;
        callSignText.text = CallSign;

        callsignObj.transform.SetParent(instance.transform);

        _callsignInstance = callSignText;
        _callsignInstance.rectTransform.pivot = new Vector2(0.5f, 0.85f);

        return instance;
    }

    public override void UpdateInstance()
    {
        base.UpdateInstance();

        var distanceSquared = (transform.position - Universe.Current.ViewPort.transform.position).sqrMagnitude;
        if (distanceSquared < 1000000f && InScreenBounds)
        {
            _callsignInstance.enabled = !IsDisabled;
        }
        else
        {
            _callsignInstance.enabled = false;
        }
    }
}
