using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	public Texture Background;

	public Color LighColour = Color.white;

	public Vector3 LightDirection;

    public string SystemName;

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