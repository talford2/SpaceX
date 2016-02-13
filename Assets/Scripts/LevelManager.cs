using UnityEngine;

public class LevelManager : MonoBehaviour
{
	//public List<GameObject> Levels;

	//public List<Material> LevelBackgrounds;

	//private int _levelIndex = 0;

	//void Start()
	//{
	//	LevelBackgrounds = new List<Material>();
	//	//foreach (var level in Levels)
	//	//{
	//	//	var uGen = level.GetComponent<UniverseGenerator>();
	//	//	LevelBackgrounds.Add(uGen.GetMaterial());
	//	//}
	//	//ChangeLevel(0);
	//}

	//int index = 0;

	//private float _systemGenTime = 2;
	//private float _systemGenCooldown = 0;

	////void Update()
	////{
	////	if (index < Levels.Count)
	////	{
	////		_systemGenCooldown -= Time.deltaTime;
	////		if (_systemGenCooldown < 0)
	////		{
	////			Debug.Log("Render system " + index);
	////			_systemGenCooldown = _systemGenTime;
	////			var uGen = Levels[index].GetComponent<UniverseGenerator>();
	////			LevelBackgrounds.Add(uGen.GetMaterial());	
	////			index++;
	////		}
	////	}

	////	if (Input.GetKeyUp(KeyCode.N))
	////	{
	////		_levelIndex++;
	////		if (_levelIndex > Levels.Count - 1)
	////		{
	////			_levelIndex = 0;
	////		}
	////		ChangeLevel(_levelIndex);
	////	}
	////}

	//private void ChangeLevel(int index)
	//{

	//	RenderSettings.skybox = LevelBackgrounds[index];
	//}
}
