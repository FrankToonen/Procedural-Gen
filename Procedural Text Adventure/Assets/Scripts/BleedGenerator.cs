using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class BleedGenerator
{
	/*static BleedPointInfo GetBleedPoint (int width, int height)
	{
		Vector2 newPoint = new Vector2 (Random.Range (0, width - 1), Random.Range (0, height - 1));
		foreach (BleedPointInfo bpi in bleedPoints) {
			if (bpi.position == newPoint) {
				return GetBleedPoint ();
			}
		}

		return new BleedPointInfo (newPoint, maxBleed);
	}*/

	public static Texture2D BleedPoints (Texture2D textureIn, List<Vector2> points, int maxBleed, Color tileColor, float bleedChance = 0.5f)
	{
		List<BleedPointInfo> bleedPoints = new List<BleedPointInfo> ();
		foreach (Vector2 p in points) {
			BleedPointInfo bpi = new BleedPointInfo (p, maxBleed);
			bleedPoints.Add (bpi);
		}

		BleedPointInfo[,] grid = new BleedPointInfo[textureIn.width, textureIn.height];
		foreach (BleedPointInfo bpi in bleedPoints) {
			grid [(int)bpi.position.x, (int)bpi.position.y] = bpi;
		}

		bool isStillBleeding = true;
		while (isStillBleeding) {
			bool isBleeding = false;

			for (int x = 0; x < textureIn.width; x++) {
				for (int y = 0; y < textureIn.height; y++) {
					if (grid [x, y] != null && grid [x, y].bloodLeft > 0) {
						if (x - 1 > 0 && Random.value < bleedChance) { // left
							if (grid [x - 1, y] == null) {
								grid [x - 1, y] = new BleedPointInfo (new Vector2 (x - 1, y), grid [x, y].bloodLeft - 1);
							}
						}

						if (x + 1 < textureIn.width && Random.value < bleedChance) { // right
							if (grid [x + 1, y] == null) {
								grid [x + 1, y] = new BleedPointInfo (new Vector2 (x + 1, y), grid [x, y].bloodLeft - 1);
							}
						}

						if (y - 1 > 0 && Random.value < bleedChance) { // up
							if (grid [x, y - 1] == null) {
								grid [x, y - 1] = new BleedPointInfo (new Vector2 (x, y - 1), grid [x, y].bloodLeft - 1);
							}
						}

						if (y + 1 < textureIn.height && Random.value < bleedChance) { // down
							if (grid [x, y + 1] == null) {
								grid [x, y + 1] = new BleedPointInfo (new Vector2 (x, y + 1), grid [x, y].bloodLeft - 1);
							}
						}

						grid [x, y].bloodLeft--;
						isBleeding = true;
					}
				}
			}

			isStillBleeding = isBleeding;
		}

		for (int x = 0; x < textureIn.width; x++) {
			for (int y = 0; y < textureIn.height; y++) {
				BleedPointInfo bpi = grid [x, y];
				Color pixelColor = textureIn.GetPixel (x, y);
				if (bpi != null) {
					float c = Mathf.Clamp01 ((float)(bpi.maxBlood + 1) / maxBleed + 0.5f);

					Color newColor = tileColor * c;
					newColor.a = 1;
					pixelColor = newColor;
				}
				textureIn.SetPixel (x, y, pixelColor);
			}
		}
		textureIn.Apply ();

		return textureIn;
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
