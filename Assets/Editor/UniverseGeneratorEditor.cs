using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UniverseGenerator))]
public class UniverseGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var universeGen = (UniverseGenerator)target;

		universeGen.FlattenToTexture = EditorGUILayout.Toggle("Flatten", universeGen.FlattenToTexture);
		universeGen.FlatResolution = EditorGUILayout.IntField("Flatten Resolution", universeGen.FlatResolution);
		universeGen.UseRandomColours = EditorGUILayout.Toggle("Use Random Colours", universeGen.UseRandomColours);
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

		// Cubemap stuff
		universeGen.BackgroundGenCubmap = EditorExtensions.ObjectField<RenderTexture>("Cube Map", universeGen.BackgroundGenCubmap, false);
		universeGen.BackgroundGenMaterial = EditorExtensions.ObjectField<Material>("Gen Material", universeGen.BackgroundGenMaterial, false);
		EditorGUILayout.Separator();


		EditorGUILayout.LabelField("Nebulae", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Count");
		universeGen.MinNebulas = EditorGUILayout.IntField(universeGen.MinNebulas);
		universeGen.MaxNubulas = EditorGUILayout.IntField(universeGen.MaxNubulas);
		EditorGUILayout.EndHorizontal();
		universeGen.NebulaBrightnessMultiplier = EditorGUILayout.Slider("Brightness Multiplier", universeGen.NebulaBrightnessMultiplier, 0, 1);
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
		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("Universe Events", EditorStyles.boldLabel);
		universeGen.CellRadius = EditorGUILayout.IntField("Cell Radius", universeGen.CellRadius);
		for (var i = 0; i < universeGen.UniverseEvents.Count; i++)
		{
			var ue = universeGen.UniverseEvents[i];

			EditorGUILayout.BeginHorizontal();
			ue.Prefab = EditorExtensions.ObjectField<GameObject>(ue.Prefab, false); //("Prefabe", ue..Prefab, false);
			ue.Count = EditorGUILayout.IntField(ue.Count);
			if (GUILayout.Button("X"))
			{
				universeGen.UniverseEvents.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add"))
		{
			universeGen.UniverseEvents.Add(null);
		}
	}
}
