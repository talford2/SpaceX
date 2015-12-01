﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UniverseGenerator))]
public class UniverseGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var universeGen = (UniverseGenerator)target;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Colour Range");
		universeGen.PrimaryColor = EditorGUILayout.ColorField(universeGen.PrimaryColor);
		universeGen.SecondaryColor = EditorGUILayout.ColorField(universeGen.SecondaryColor);
		EditorGUILayout.EndHorizontal();
		universeGen.BackgroundContainer = EditorExtensions.ObjectField<GameObject>("Container", universeGen.BackgroundContainer, true);
		universeGen.SceneRelfectionProbe = EditorExtensions.ObjectField<ReflectionProbe>("Reflection Probe", universeGen.SceneRelfectionProbe, true);
		universeGen.BackgroundCamera = EditorExtensions.ObjectField<Camera>("Camera", universeGen.BackgroundCamera, true);
		universeGen.SunObject = EditorExtensions.ObjectField<GameObject>("Sun", universeGen.SunObject, false);
		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
		universeGen.BackgroundMaterial = EditorExtensions.ObjectField<Material>("Background", universeGen.BackgroundMaterial, false);
		universeGen.DustMaterial = EditorExtensions.ObjectField<Material>("Dust", universeGen.DustMaterial, false);
		universeGen.AddFogMaterial = EditorExtensions.ObjectField<Material>("Fog", universeGen.AddFogMaterial, false);
		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("Nebulae", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Count");
		universeGen.MinNebulas = EditorGUILayout.IntField(universeGen.MinNebulas);
		universeGen.MaxNubulas = EditorGUILayout.IntField(universeGen.MaxNubulas);
		EditorGUILayout.EndHorizontal();
		universeGen.Nebulas = EditorExtensions.GameObjectList("Prefabs", universeGen.Nebulas, false);
		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("Planets", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Count");
		universeGen.MinPlanets = EditorGUILayout.IntField(universeGen.MinPlanets);
		universeGen.MaxPlanets = EditorGUILayout.IntField(universeGen.MaxPlanets);
		EditorGUILayout.EndHorizontal();
		universeGen.Planets = EditorExtensions.GameObjectList("Prefabs", universeGen.Planets, false);

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Star Field", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Count");
		universeGen.Count = EditorGUILayout.IntField(universeGen.Count);
		universeGen.Radius = EditorGUILayout.FloatField(universeGen.Radius);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Size");
		universeGen.MinSize = EditorGUILayout.FloatField(universeGen.MinSize);
		universeGen.MaxSize = EditorGUILayout.FloatField(universeGen.MaxSize);
		EditorGUILayout.EndHorizontal();
	}

	public override void DrawPreview(Rect previewArea)
	{
		base.DrawPreview(previewArea);
	}
}