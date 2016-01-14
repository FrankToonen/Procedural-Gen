using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureGenerator : MapGenerator
{
	public GameObject display;
	protected Texture2D gridTexture;

	protected override void Start ()
	{
		base.Start ();

		VisualizeGrid ();
	}

	protected override void Update ()
	{
		base.Update ();

		if (Input.GetKeyDown (KeyCode.Space)) {
			VisualizeGrid ();
		}
	}

	protected virtual void VisualizeGrid ()
	{
		gridTexture = new Texture2D (width, height);

		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {

				gridTexture.SetPixel (x, z, grid [x, z].color);
			}
		}
		gridTexture.Apply ();

		display.GetComponent<MeshRenderer> ().material.mainTexture = gridTexture;
	}
}
