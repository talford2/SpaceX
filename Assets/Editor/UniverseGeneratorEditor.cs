﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UniverseGenerator))]
public class UniverseGeneratorEditor : Editor
{
    void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
        Debug.Log("On Enabled!");
    }

    public override void OnInspectorGUI()
    {
        var universeGen = (UniverseGenerator)target;

        if (universeGen != null)
        {
            universeGen.Seed = EditorGUILayout.IntField("Seed", universeGen.Seed);
            universeGen.BackgroundLayer = EditorGUILayout.LayerField("Layer", universeGen.BackgroundLayer);
            universeGen.Level = EditorExtensions.ObjectField<LevelDefinition>("Level", universeGen.Level, false);
            universeGen.FlatResolution = EditorExtensions.IntDropdown("Resolution", new List<int> { 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 }, universeGen.FlatResolution);

            universeGen.CubemapShader = EditorExtensions.ObjectField<Shader>("Background Shader", universeGen.CubemapShader, false);
            universeGen.BaseShader = EditorExtensions.ObjectField<Shader>("Base Shader", universeGen.BaseShader, false);
            universeGen.GradientShader = EditorExtensions.ObjectField<Shader>("Gradient Shader", universeGen.GradientShader, false);
            universeGen.BackgroundColor = EditorGUILayout.ColorField("Background Color", universeGen.BackgroundColor);

            // Sun
            EditorGUILayout.Separator();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
            EditorGUILayout.LabelField("Sun", EditorStyles.boldLabel);
            universeGen.SunLight = EditorExtensions.ObjectField<Light>("Sun Light", universeGen.SunLight, true);
            universeGen.SunModel = EditorExtensions.ObjectField<GameObject>("Sun Model", universeGen.SunModel, false);
            universeGen.SunTexture = EditorExtensions.ObjectField<Texture>("Sun Texture", universeGen.SunTexture, false);
            universeGen.SunIntensity = EditorGUILayout.FloatField("Intensity", universeGen.SunIntensity);
            universeGen.SunColour = EditorGUILayout.ColorField("Colour", universeGen.SunColour);
            universeGen.SunAttachPrefab = EditorExtensions.ObjectField<GameObject>("Sun Attach Prefab", universeGen.SunAttachPrefab, false);

            EditorGUILayout.Separator();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
            EditorGUILayout.LabelField(string.Format("Scatter Groups ({0})", universeGen.ScatterObjects.Count), EditorStyles.boldLabel);

            for (var i = 0; i < universeGen.ScatterObjects.Count; i++)
            {
                var so = universeGen.ScatterObjects[i];
                so = ScatterGUI(so);
                EditorGUILayout.Separator();
                if (GUILayout.Button("Remove Scatter Group"))
                {
                    universeGen.ScatterObjects.RemoveAt(i);
                }
                EditorGUILayout.Separator();
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
            }

            EditorGUILayout.Separator();
            if (GUILayout.Button("Add Scatter Group"))
            {
                universeGen.ScatterObjects.Add(new ScatterSettings());
            }
        }

        //EditorGUILayout.TextArea(JsonUtility.ToJson(universeGen), GUILayout.Height(100), GUILayout.MaxWidth(200));
    }

    private ScatterSettings ScatterGUI(ScatterSettings so)
    {
        so.IsActive = EditorGUILayout.BeginToggleGroup(so.Name, so.IsActive);

        so.Name = EditorGUILayout.TextField("Name", so.Name);

        so.Model = EditorExtensions.ObjectField("Model", so.Model, false);

        // Radius
        var radius = EditorExtensions.FloatRange("Radius", so.RadiusMin, so.RadiusMax);
        so.RadiusMin = radius.Min;
        so.RadiusMax = radius.Max;

        // Count
        var count = EditorExtensions.IntRange("Count", so.CountMin, so.CountMax);
        so.CountMin = count.Min;
        so.CountMax = count.Max;

        // Scale
        var scale = EditorExtensions.FloatRange("Scale", so.ScaleMin, so.ScaleMax);
        so.ScaleMin = scale.Min;
        so.ScaleMax = scale.Max;

        so.LookAtCenter = GUILayout.Toggle(so.LookAtCenter, "Look at Centre");

        so.UseClustering = GUILayout.Toggle(so.UseClustering, "Use clustering");
        if (so.UseClustering)
        {
            var clusterCount = EditorExtensions.IntRange("Cluster Count", so.ClusterCountMin, so.ClusterCountMax);
            so.ClusterCountMin = clusterCount.Min;
            so.ClusterCountMax = clusterCount.Max;
            so.ClusterScatter = EditorGUILayout.FloatField("Cluster Scatter", so.ClusterScatter);
        }

        // Materials
        so.UseMaterials = GUILayout.Toggle(so.UseMaterials, "Use materials");
        if (so.UseMaterials)
        {
            so.Materials = EditorExtensions.GameObjectList<Material>("Materials", so.Materials, false);
            EditorGUILayout.Separator();
        }
        else
        {
            // Textures
            so.Textures = EditorExtensions.GameObjectList<Texture>("Textures", so.Textures, false);
            EditorGUILayout.Separator();

            // Colours
            if (so.Colors == null)
            {
                so.Colors = new List<ColorRange>();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Colors");

            EditorGUILayout.BeginVertical();
            for (var j = 0; j < so.Colors.Count; j++)
            {
                var clr = so.Colors[j];
                EditorGUILayout.BeginHorizontal();
                clr.Color1 = EditorGUILayout.ColorField(clr.Color1);
                clr.Color2 = EditorGUILayout.ColorField(clr.Color2);
                if (GUILayout.Button("Remove"))
                {
                    so.Colors.RemoveAt(j);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add"))
            {
                so.Colors.Add(new ColorRange());
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndToggleGroup();
        return so;
    }
}
