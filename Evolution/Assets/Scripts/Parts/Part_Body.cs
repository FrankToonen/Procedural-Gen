using UnityEngine;
using System.Collections;

public class Part_Body : CreaturePart
{
	public int bodySize;
	public Color bodyColor;

	public Part_Body (GameObject _parent)
		: base (_parent)
	{
		partName = "body";
		bodySize = Random.Range (100, 500);
		bodyColor = new Color (Random.value, Random.value, Random.value);
	}

	public override Texture2D CreateSprite (Texture2D texture)
	{
		texture = new Texture2D (bodySize, bodySize);

		Color[] pixels = texture.GetPixels ();
		for (int i = 0; i < pixels.Length; i++) {
			pixels [i] = bodyColor;
		}

		texture.SetPixels (pixels);
		texture.Apply ();

		return texture;
	}

	protected override void UpdatePart ()
	{
		base.UpdatePart ();
	}
}
