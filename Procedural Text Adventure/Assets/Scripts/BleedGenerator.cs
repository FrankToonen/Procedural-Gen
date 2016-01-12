using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BleedGenerator : MonoBehaviour
{
	public GameObject plane;
	public int width = 10, height = 10;
	public int numberOfPoints = 3, maxBleed = 5;
	public Texture2D map;

	List<BleedPointInfo> bleedPoints;
	BleedPointInfo[,] grid;

	void Start ()
	{
		map = new Texture2D (width, height);
		bleedPoints = new List<BleedPointInfo> ();
		for (int i = 0; i < numberOfPoints; i++) {
			bleedPoints.Add (GetBleedPoint ());
		}

		GenerateMap ();
	}

	BleedPointInfo GetBleedPoint ()
	{
		Vector2 newPoint = new Vector2 (Random.Range (0, width - 1), Random.Range (0, height - 1));
		foreach (BleedPointInfo bpi in bleedPoints) {
			if (bpi.position == newPoint) {
				return GetBleedPoint ();
			}
		}

		return new BleedPointInfo (newPoint, maxBleed);
	}

	void GenerateMap ()
	{
		grid = new BleedPointInfo[width, height];
		foreach (BleedPointInfo bpi in bleedPoints) {
			grid [(int)bpi.position.x, (int)bpi.position.y] = bpi;
		}

		bool isStillBleeding = true;
		while (isStillBleeding) {
			bool isBleeding = false;

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (grid [x, y] != null && grid [x, y].bloodLeft > 0) {
						if (x - 1 > 0) { // left
							grid [x - 1, y] = new BleedPointInfo (new Vector2 (x - 1, y), grid [x, y].bloodLeft - 1);
						}

						if (x + 1 < width) { // right
							grid [x + 1, y] = new BleedPointInfo (new Vector2 (x + 1, y), grid [x, y].bloodLeft - 1);
						}

						if (y - 1 > 0) { // up
							grid [x, y - 1] = new BleedPointInfo (new Vector2 (x, y - 1), grid [x, y].bloodLeft - 1);
						}

						if (y + 1 < height) { // down
							grid [x, y + 1] = new BleedPointInfo (new Vector2 (x, y + 1), grid [x, y].bloodLeft - 1);
						}

						grid [x, y].bloodLeft--;
						isBleeding = true;
					}
				}
			}

			isStillBleeding = isBleeding;
		}


		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				BleedPointInfo bpi = grid [x, y];
				Color pixelColor = Color.white;
				if (bpi != null) {
					float c = bpi.maxBlood / maxBleed;
					pixelColor = new Color (c, c, c);
				}
				map.SetPixel (x, y, pixelColor);
			}
		}
		map.Apply ();
		plane.GetComponent<MeshRenderer> ().material.mainTexture = map;
	}

	public class BleedPointInfo
	{
		public Vector2 position;
		public int bloodLeft;
		public int maxBlood;

		public BleedPointInfo (Vector2 _position, int max)
		{
			position = _position;
			bloodLeft = max;
			maxBlood = max;
		}
	}
}
