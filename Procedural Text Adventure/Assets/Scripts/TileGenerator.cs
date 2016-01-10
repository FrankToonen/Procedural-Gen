using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGenerator : MonoBehaviour
{
	public int width = 25, height = 25;
	public float noiseSmoothing = 1;

	public GameObject display, meshHolder;
	Texture2D map;
	List<Vector3> positions;

	void Start ()
	{
		GenerateGrid ();
		CreateMesh ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			GenerateGrid ();
		}
	}

	void GenerateGrid ()
	{
		positions = new List<Vector3> ();
		map = new Texture2D (width, height);
		float m = Random.Range (0, 10000);

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				float noise = Mathf.PerlinNoise ((x + m) / width / noiseSmoothing, (y + m) / height / noiseSmoothing);
				float r = 0;
				float g = noise >= 0.3f ? noise : 0;
				float b = noise < 0.3f ? noise / .3f : 0;

				Color pixelColor = new Color (r, g, b);
				map.SetPixel (x, y, pixelColor);
				positions.Add (new Vector3 (x, y, noise));
			}
		}
		map.Apply ();
		display.GetComponent<MeshRenderer> ().material.mainTexture = map;
	}

	void CreateMesh ()
	{
		Mesh mesh = meshHolder.GetComponent<MeshFilter> ().mesh;

		Vector3[] vertices = positions.ToArray ();
		Vector2[] vertices2D = new Vector2[width * height];
		for (int i = 0; i < vertices.Length; i++) {
			vertices2D [i] = new Vector2 (vertices [i].x, vertices [i].y);
		}

		Triangulator tr = new Triangulator (vertices2D);

		mesh.vertices = vertices;
		mesh.triangles = tr.Triangulate ();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}
}
