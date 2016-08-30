using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ShipPart))]
public class ShipPartEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var shipPart = (ShipPart)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Find Connectors"))
        {
            shipPart.Connectors = new List<ShipPartConnector>();
            var childTransforms = shipPart.GetComponentsInChildren<Transform>();
            foreach (var childTransform in childTransforms)
            {
                if (childTransform.name.StartsWith("Connector"))
                {
                    Debug.LogFormat("Found {0}.", childTransform.name);
                    var connector = childTransform.GetComponent<ShipPartConnector>();
                    if (connector == null)
                        connector = childTransform.gameObject.AddComponent<ShipPartConnector>();
                    shipPart.Connectors.Add(connector);
                }
            }
        }
    }
}
