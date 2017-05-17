using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "SpaceX/Level", order = 1)]
public class LevelDefinition : ScriptableObject
{
    public Texture Background;

    public Color LightColour = Color.white;

    public Vector3 LightDirection
    {
        get
        {
            ReadFile();
            return _lightDirection;
        }
        set
        {
            _lightDirection = value;
        }
    }

    public string SystemName;

    public int SlipGateLevelIndex = 0;

    public List<UniverseEventCount> UniverseEvents;

    private Material _mat;
    private Vector3 _lightDirection;

    public Material Material
    {
        get
        {
            if (_mat == null)
            {
                _mat = new Material(LevelManager.Current.CubeMapShader);
                _mat.SetTexture("_Tex", Background);
            }
            return _mat;
        }
    }

    public void WriteFile(LevelFile levelFile)
    {
        levelFile.WriteToFile(FullFilePath());
    }

    public void ReadFile()
    {
        var levelFile = LevelFile.ReadFromFile(FullFilePath());
        _lightDirection = levelFile.SunDirection;
    }

    private string FullFilePath()
    {
        return string.Format("{0}/Environment/Levels/{1}.xml", Application.dataPath, name);
    }
}
