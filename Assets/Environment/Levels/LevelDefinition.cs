using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "SpaceX/Level", order = 1)]
public class LevelDefinition : ScriptableObject
{
    public Texture Background;

    public Color LighColour = Color.white;

    public Vector3 LightDirection;

    public string SystemName;

    public int SlipGateLevelIndex = 0;

    public List<UniverseEventCount> UniverseEvents;

    private Material _mat;

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
}
