using UnityEngine;
using System.Collections;

public class ScrollUVs : MonoBehaviour
{
	public enum TextureType { _MainTex, _BumpMap, _LightMap, _Cube, _ParticleTexture };
	public readonly string[] TextureTypeNames = { "_MainTex", "_BumpMap", "_LightMap", "_Cube", "_ParticleTexture" };

	public TextureType Type = TextureType._MainTex;
	public int materialID = 0;
	public float scrollSpeedX = 0.5f;
	public float scrollSpeedY = 0.5f;

	private float offsetX = 0.0f;
	private float offsetY = 0.0f;
	private Material mat = null;

	void Start()
	{
		mat = GetComponent<Renderer>().materials[materialID];
	}

	void Update()
	{
		offsetX = Mathf.Repeat(Time.time * scrollSpeedX, 1.0f);
		offsetY = Mathf.Repeat(Time.time * scrollSpeedY, 1.0f);
		mat.SetTextureOffset(TextureTypeNames[(int)Type], new Vector2(offsetX, offsetY));
	}
}
