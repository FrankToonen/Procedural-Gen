using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour
{
	public int width = 25, height = 25;
	public float noiseSmoothing = 1, noiseIntensity = 1;
	public float meshWidth = 10, meshHeight = 10;

	public GameObject meshHolder;
	Texture2D map;
	List<VertexInfo> vertexInfos;

	void Start ()
	{
		GenerateGrid ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			GenerateGrid ();
		}
	}

	void GenerateGrid ()
	{
		vertexInfos = new List<VertexInfo> ();
		map = new Texture2D (width, height);
		float m = Random.Range (0, 10000);

		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {

				float noise = Mathf.PerlinNoise ((x + m) / width / noiseSmoothing, (z + m) / height / noiseSmoothing);
				float r = 0, g = 0, b = 0;

				if (noise < 0.3f) {
					//b = noise / .3f;
					b = 1;
				} else if (noise < 0.4f) {
					//r = (noise - .3f) / .1f;
					//g = (noise - .3f) / .1f;
					//b = (noise - .3f) / .1f / 2;
					r = 1;
					g = 1;
					b = 0.5f;
				} else {
					g = 1 - ((noise - .4f) / .6f);
					//g = 1;
				}

				Color pixelColor = new Color (r, g, b);
				map.SetPixel (x, z, pixelColor);

				float yValue = noise < 0.3f ? 0.3f * noiseIntensity : noise * noiseIntensity;

				Vector3 position = new Vector3 (x * (meshWidth / width), yValue, z * (meshHeight / height));
				vertexInfos.Add (new VertexInfo (position, pixelColor));
			}
		}
		map.Apply ();
		CreateMesh ();
	}

	void CreateMesh ()
	{
		Vector3[] vertices = new Vector3[vertexInfos.Count];
		Color[] colors = new Color[vertexInfos.Count];
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] = vertexInfos [i].position;
			//colors [i] = vertices [i].y < 0.3f * noiseIntensity ? Color.blue : Color.green;
			colors [i] = vertexInfos [i].color;
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

	public class VertexInfo
	{
		public Vector3 position;
		public Color color;

		public VertexInfo (Vector3 _position, Color _color)
		{
			position = _position;
			color = _color;
		}
	}
}
