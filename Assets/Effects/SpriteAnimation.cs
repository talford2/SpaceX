using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
	private Material _material;
	private int _matSize;
	private Texture _texture;

	public int XCells = 2;
	public int YCells = 2;
	public int SpriteIndex = 0;
	public string TexturePropertyName = "_MainTex";

	void Awake()
	{
		_material = GetComponent<Renderer>().material;

		_texture = _material.GetTexture(TexturePropertyName);
		_matSize = System.Math.Min(_texture.width, _texture.height);

		_material.SetTextureScale(TexturePropertyName, new Vector2(1f / ((float)XCells), 1f / ((float)YCells)));

		SetSpriteIndex(0);
	}

	public void SetSpriteIndex(int index)
	{
		SpriteIndex = index;
		var size = new Vector2(1.0f / XCells, 1.0f / YCells);
		var uIndex = index % XCells;
		var vIndex = index / XCells;
		_material.SetTextureOffset(TexturePropertyName, new Vector2(uIndex * size.x, 1.0f - size.y - vIndex * size.y));
	}

	public void NextSpriteIndex()
	{
		SpriteIndex++;
		if (SpriteIndex >= (XCells * YCells))
		{
			SpriteIndex = 0;
		}
		SetSpriteIndex(SpriteIndex);
	}

	public void RandomIndex()
	{
		SetSpriteIndex(Random.Range(0, XCells * YCells - 1));
	}
}
