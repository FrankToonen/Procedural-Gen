using UnityEngine;
using System.Collections;

public class Part_Eye : CreaturePart
{
	public float sightDistance;
	public float sightAngle = 90;
	public int eyeSize = 100;
	Color[,] eyeKernel;

	public Part_Eye (GameObject _parent)
		: base (_parent)
	{
		partName = "eyes";
		SetKernel ();
	}

	public override Texture2D CreateSprite (Texture2D texture)
	{
		Color[] pixels = texture.GetPixels ();
		Color[,] pixels2D = new Color[texture.width, texture.height];
		for (int i = 0; i < pixels.Length; i++) {
			
			int x = i % texture.width;
			int y = i / texture.height;

			pixels2D [x, y] = pixels [i];
		}

		int amountOfEyes = (int)(sightAngle / 45);
		for (int i = 0; i < amountOfEyes; i++) {
			
			ApplyKernel (ref pixels2D, texture.width, texture.height, new Vector2 (Random.Range (0, texture.width), Random.Range (0, texture.height)));
		}

		for (int x = 0; x < pixels2D.GetLength (0); x++) {
			for (int y = 0; y < pixels2D.GetLength (1); y++) {
				
				pixels [x + y * texture.width] = pixels2D [x, y];
			}
		}

		texture.SetPixels (pixels);
		texture.Apply ();

		return texture;
	}

	void SetKernel ()
	{
		eyeKernel = new Color[eyeSize, eyeSize];
		for (int x = 0; x < eyeSize; x++) {
			for (int y = 0; y < eyeSize; y++) {
				
				float c = (float)(x + y) / (2 * eyeSize);
				eyeKernel [x, y] = new Color (c, c, c);
			}		
		}
	}

	void ApplyKernel (ref Color[,] pixels, int width, int height, Vector2 position)
	{
		for (int x = 0; x < eyeSize; x++) {
			for (int y = 0; y < eyeSize; y++) {
				
				int xIndex = x + (int)position.x;
				int yIndex = y + (int)position.y;

				if (xIndex < 0 || xIndex >= width || yIndex < 0 || yIndex >= height) {
					continue;
				}

				pixels [xIndex, yIndex] = eyeKernel [x, y];
			}
		}
	}

	protected override void UpdatePart ()
	{
		base.UpdatePart ();
	}
}

