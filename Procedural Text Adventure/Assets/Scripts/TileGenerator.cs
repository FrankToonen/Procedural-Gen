using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGenerator : MonoBehaviour
{
	public int width = 25, height = 25;
	public float noiseSmoothing = 1, noiseIntensity = 1;
	public float meshWidth = 10, meshHeight = 10;

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
			CreateMesh ();
		}
	}

	void GenerateGrid ()
	{
		positions = new List<Vector3> ();
		map = new Texture2D (width, height);
		float m = Random.Range (0, 10000);

		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {

				float noise = Mathf.PerlinNoise ((x + m) / width / noiseSmoothing, (z + m) / height / noiseSmoothing);
				float r = 0;
				float g = noise >= 0.3f ? noise : 0;
				float b = noise < 0.3f ? noise / .3f : 0;

				Color pixelColor = new Color (r, g, b);
				map.SetPixel (x, z, pixelColor);
				positions.Add (new Vector3 (x * (meshWidth / width), noise * noiseIntensity, z * (meshHeight / height)));
			}
		}
		map.Apply ();
		display.GetComponent<MeshRenderer> ().material.mainTexture = map;
	}

	void CreateMesh ()
	{
		Vector3[] vertices = new Vector3[positions.Count];
		Color[] colors = new Color[positions.Count];
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] = positions [i];
			colors [i] = vertices [i].y < 0.3f * noiseIntensity ? Color.blue : Color.green;
		}
		JitterVertices (ref vertices);

		int[] triangles = FindTriangles (vertices.Length, vertices);

		Mesh newMesh = new Mesh ();
		newMesh.vertices = vertices;
		newMesh.triangles = triangles;
		newMesh.colors = colors;
		newMesh.RecalculateNormals ();
		newMesh.RecalculateBounds ();


		meshHolder.GetComponent<MeshFilter> ().mesh = newMesh;
	}

	int[] FindTriangles (int vertices, Vector3[] v)
	{
		int[] triangles = new int[(width - 1) * (height - 1) * 2 * 3];

		int index = 0;
		for (int i = 0; i < vertices - width; i++) {
			// Triangle 1
			if (i % height < width - 1) {
				triangles [index++] = i;
				triangles [index++] = i + 1;
				triangles [index++] = i + width;
			}

			// Triangle 2
			if (i % height > 0) {
				triangles [index++] = i;
				triangles [index++] = i + width;
				triangles [index++] = i + width - 1;
			}
		}

		return triangles;
	}

	void JitterVertices (ref Vector3[] vertices)
	{
		float w = (meshWidth / width) / 4;
		float h = (meshHeight / height) / 4;

		for (int i = 0; i < vertices.Length; i++) {
			if (!IsBoundary (i)) {
				Vector3 v = vertices [i];
				v.x += (Random.value * 2 - 1) * w;
				v.z += (Random.value * 2 - 1) * h;
				vertices [i] = v;
			}
		}
	}

	bool IsBoundary (int i)
	{
		return (i % height == width - 1) || (i % height == 0) || (i < width) || (i > (width - 1) * height);
	}
}
