using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
	public int width = 25, height = 25;
	public float noiseSmoothing = 1, noiseIntensity = 1;
	public float[] possibleYValues;
	public Tile[,] grid;
	public int seed = 100;
	public int interpolateTimes = 1;

	protected virtual void Start ()
	{
		Random.seed = seed;

		GenerateGrid ();
	}

	protected virtual void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {

			GenerateGrid ();
		}
	}

	protected void GenerateGrid ()
	{
		grid = new Tile[width, height];
		float noiseOffset = Random.Range (0, 10000);

		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {

				float noise = Mathf.PerlinNoise ((x + noiseOffset) / width / noiseSmoothing, (z + noiseOffset) / height / noiseSmoothing);

				float y = util.ClampToNearest (noise, possibleYValues);

				Color c = GetColor (x, y, z);

				grid [x, z] = new Tile (x, y, z, c);
			}
		}

		for (int i = 0; i < interpolateTimes; i++) {
			InterpolateY ();
		}

		//PopulateMap ();
	}

	Color GetColor (float x, float y, float z)
	{
		/*float r = 0, g = 0, b = 0;

				if (noise < 0.3f) {
					b = 1;
				} else if (noise < 0.4f) {
					r = 1;
					g = 1;
					b = 0.5f;
				} else {
					g = 1 - ((noise - .4f) / .6f);
				}

				Color pixelColor = new Color (r, g, b);*/
		
		float r = 0;
		float g = 0;
		float b = 0;

		if (y < possibleYValues [0]) {
			b = 1;
		} else if (y < possibleYValues [1]) {
			b = .5f;
		} else if (y < possibleYValues [2]) {
			r = 1;
			g = 1;
			b = .5f;
		} else if (y < possibleYValues [3]) {
			g = 1;
		} else if (y < possibleYValues [4]) {
			g = .5f;
		}

		return new Color (r, g, b);
	}

	void InterpolateY ()
	{
		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {

				Tile tile = grid [x, z];
				float average = tile.y;
				int amount = 1;

				for (int u = -1; u <= 1; u++) {
					for (int v = -1; v <= 1; v++) {
						int xI = x + u;
						int yI = z + v;

						if (xI < 0 || xI >= width || yI < 0 || yI >= height) {
							continue;
						}

						average += grid [xI, yI].y;
						amount++;
					}
				}

				float newY = average / amount;
				tile.y = newY;
				tile.color = GetColor (tile.x, tile.y, tile.z);
			}
		}
	}


	/*void PopulateMap ()
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
	}*/

	protected bool IsBoundary (int i)
	{
		return (i % height == width - 1) || (i % height == 0) || (i < width) || (i > (width - 1) * height);
	}
}
