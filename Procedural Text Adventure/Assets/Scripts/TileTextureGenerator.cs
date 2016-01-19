using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TileTextureGenerator
{
	public static Texture2D GetTexture (Tile.TileType type, int width, int height, float noiseSmoothing, Vector2 offset)
	{
		Texture2D texture = new Texture2D (width, height);

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				float pX = ((float)x / width) / noiseSmoothing + offset.x;
				float pY = ((float)y / height) / noiseSmoothing + offset.y;
				float noise = Mathf.PerlinNoise (pX, pY);
				//Debug.Log (pX + " " + x + " " + ((float)x / width));

				Color c = Color.black;
				if (type == Tile.TileType.Grass) {
					c = new Color (0, 0, noise);
				} else if (type == Tile.TileType.Water) {
					c = new Color (0, noise, 0);
				} else {
					c = new Color (noise, 0, 0);
				}

				//Color c = new Color (0, noise, 0);
				texture.SetPixel (x, y, c);

				//grid [x, y] = new Tile (Tile.TileType.Grass, x, noise, y, Color.white);
			
			
			}
		}

		texture.Apply ();

		return texture;

		/*Texture2D gridTexture = new Texture2D (width, height);

		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {

				gridTexture.SetPixel (x, z, grid [x, z].color);
			}
		}
		gridTexture.Apply ();

		return gridTexture;*/
	}
}
