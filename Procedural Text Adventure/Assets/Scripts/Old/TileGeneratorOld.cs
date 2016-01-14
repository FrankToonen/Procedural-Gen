using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGeneratorOld : MonoBehaviour
{
	public int width = 25, height = 25;
	public float noiseSmoothing = 1, noiseIntensity = 1;
	public float meshWidth = 10, meshHeight = 10;

	public GameObject display;
	Texture2D map;
	List<Vector2> villagePoints;

	void Start ()
	{
		GenerateGrid ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			GenerateGrid ();
		}
	}

	void GenerateGrid ()
	{
		map = new Texture2D (width, height);
		float m = Random.Range (0, 10000);

		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {

				float noise = Mathf.PerlinNoise ((x + m) / width / noiseSmoothing, (z + m) / height / noiseSmoothing);
				float r = 0, g = 0, b = 0;

				if (noise < 0.3f) {
					b = 1;
				} else if (noise < 0.4f) {
					r = 1;
					g = 1;
					b = 0.5f;
				} else {
					g = 1 - ((noise - .4f) / .6f);
				}

				Color pixelColor = new Color (r, g, b);
				map.SetPixel (x, z, pixelColor);
			}
		}
		map.Apply ();

		PopulateMap ();

		display.GetComponent<MeshRenderer> ().material.mainTexture = map;
	}

	void PopulateMap ()
	{
		SetVillages ();

	}

	void SetVillages ()
	{
		int amountOfVillages = Random.Range (50, 100);
		int villageSize = Random.Range (20, 20);
		villagePoints = new List<Vector2> ();

		for (int i = 0; i < amountOfVillages; i++) {
		
			villagePoints.Add (GetRandomPosition (0, 0.1f, 0));
		}

		map = BleedGenerator.BleedPoints (map, villagePoints, villageSize, Color.red, 0.3f);

		map.Apply ();
	}

	void CreateBanditCamps ()
	{

	}

	Vector2 GetRandomPosition (float r = 0, float g = 0, float b = 0)
	{
		Vector2 pos = new Vector2 (Random.Range (0, map.width), Random.Range (0, map.height));
		Color c = map.GetPixel ((int)pos.x, (int)pos.y);
		if ((r > 0 && r > c.r) || (g > 0 && g > c.g) || (b > 0 && b > c.b)) {
			return GetRandomPosition (r, g, b);
		} else {
			return pos;
		}
	}

	bool IsBoundary (int i)
	{
		return (i % height == width - 1) || (i % height == 0) || (i < width) || (i > (width - 1) * height);
	}
}
