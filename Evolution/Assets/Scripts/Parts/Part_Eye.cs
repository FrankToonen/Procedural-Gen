using UnityEngine;
using System.Collections;

public class Part_Eye : CreaturePart
{
	public float sightDistance;
	public float sightAngle = 360;
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
			
			int radius = (texture.width - eyeSize) / 2;
			float xPos = Mathf.Sin (Mathf.Deg2Rad * (i * 45));
			float yPos = Mathf.Cos (Mathf.Deg2Rad * (i * 45));
			Vector2 pos = new Vector2 (xPos, yPos) * radius + new Vector2 (texture.width - eyeSize, texture.height - eyeSize) / 2;

			ApplyKernel (ref pixels2D, texture.width, texture.height, pos);
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

		Vector2 center = new Vector2 (eyeSize / 2, eyeSize / 2);
		for (int x = 0; x < eyeSize; x++) {
			for (int y = 0; y < eyeSize; y++) {

				Vector2 pos = new Vector2 (x, y);
				float distance = Vector2.Distance (center, pos);
				float c = distance / (eyeSize / 2);
				float a = 1 - c;

				//float c = (float)(x + y) / (2 * eyeSize);
				eyeKernel [x, y] = new Color (c, c, c, a);
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

