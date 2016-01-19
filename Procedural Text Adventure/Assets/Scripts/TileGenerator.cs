using UnityEngine;
using System.Collections;

public class TileGenerator : MapGenerator
{
	public GameObject tilePrefab;
	public float tileSize;

	GameObject[,] tileObjects;

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

	void VisualizeGrid ()
	{
		if (tileObjects != null) {
			foreach (GameObject obj in tileObjects) {
				Destroy (obj);
			}
		}

		tileObjects = new GameObject[width, height];
		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {
				
				GameObject newTile = Instantiate (tilePrefab);
				newTile.transform.position = new Vector3 (x * tileSize, 0, z * tileSize);

				//newTile.GetComponent<MeshRenderer> ().material.color = grid [x, z].color;
				//newTile.GetComponent<MeshRenderer> ().material.mainTexture = TileTextureGenerator.GetTexture (grid, width, height);
				//if (x == 0 && z == 0) {
				//	Debug.Log ((x + noiseOffset) + " " + (z + noiseOffset));
				newTile.GetComponent<MeshRenderer> ().material.mainTexture = TileTextureGenerator.GetTexture (grid [x, z].type, width, height, noiseSmoothing, new Vector2 (x + noiseOffset, z + noiseOffset));
				//}

				newTile.transform.localScale *= tileSize;
				newTile.transform.parent = transform;
				tileObjects [x, z] = newTile;
			}
		}
	}
}
