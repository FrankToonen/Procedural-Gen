using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGeneratorOld : MonoBehaviour
{
	public int width = 25, height = 25;
	public float noiseSmoothing = 1, noiseIntensity = 1;
	public float meshWidth = 10, meshHeight = 10;

	public GameObject meshHolder;
	List<VertexInfo> vertexInfos;
	List<int> villagePoints;
	public Texture2D map;

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
		map = new Texture2D (width, height);
		vertexInfos = new List<VertexInfo> ();
		float m = Random.Range (0, 10000);

		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {

				float noise = Mathf.PerlinNoise ((x + m) / width / noiseSmoothing, (z + m) / height / noiseSmoothing);
				float r = 0, g = 0, b = 0;

				if (noise < 0.3f) {
					b = 1;
				} else if (noise < 0.4f) {
					r = 1;
					g = 1;
					b = 0.5f;
				} else {
					g = 1 - ((noise - .4f) / .6f);
				}

				Color pixelColor = new Color (r, g, b);
				map.SetPixel (x, z, pixelColor);

				float yValue = noise < 0.3f ? 0.3f * noiseIntensity : noise * noiseIntensity;
				Vector3 position = new Vector3 (x * (meshWidth / width), yValue, z * (meshHeight / height));
				vertexInfos.Add (new VertexInfo (position, pixelColor));
			}
		}

		map.Apply ();

		PopulateMap ();

		CreateMesh ();

		//meshHolder.GetComponent<MeshRenderer> ().material.mainTexture = map;
	}

	void PopulateMap ()
	{
		SetVillages ();
	}

	void SetVillages ()
	{
		int amountOfVillages = Random.Range (5, 10);
		int villageSize = Random.Range (5, 5);
		villagePoints = new List<int> ();

		for (int i = 0; i < amountOfVillages; i++) {

			int villageIndex = GetRandomVertex (0, 0.1f, 0);
			float yValue = vertexInfos [villageIndex].position.y;

			for (int x = -villageSize / 2; x < villageSize / 2; x++) {
				for (int y = -villageSize / 2; y < villageSize / 2; y++) {

					int index = villageIndex + x + y * width;

					if (index < vertexInfos.Count && index >= 0) {
						villagePoints.Add (index);

						// KLOPT NICHT
						map.SetPixel (villageIndex % width + x, villageIndex % width + y * width, Color.black);

						vertexInfos [index].color = Color.black;
						vertexInfos [index].position.y = yValue;
					}
				}
			}
		}
		map.Apply ();
	}

	int GetRandomVertex (float r = 0, float g = 0, float b = 0)
	{
		int index = Random.Range (0, vertexInfos.Count);

		VertexInfo vertex = vertexInfos [index];
		Color c = vertex.color;

		if ((r > 0 && r > c.r) || (g > 0 && g > c.g) || (b > 0 && b > c.b)) {
			return GetRandomVertex (r, g, b);
		} else {
			return index;
		}
	}

	void CreateMesh ()
	{
		Vector3[] vertices = new Vector3[vertexInfos.Count];
		Color[] colors = new Color[vertexInfos.Count];

		for (int i = 0; i < vertices.Length; i++) {
			
			vertices [i] = vertexInfos [i].position;
			colors [i] = vertexInfos [i].color;
		}

		JitterVertices (ref vertices);

		int[] triangles = FindTriangles (vertices.Length, vertices);

		Vector2[] uvs = new Vector2[vertices.Length];
		for (int i = 0; i < uvs.Length; i++) {
			uvs [i] = new Vector2 (vertices [i].x / meshWidth, vertices [i].z / meshHeight);
		}

		Mesh newMesh = new Mesh ();
		newMesh.vertices = vertices;
		newMesh.triangles = triangles;
		newMesh.uv = uvs;
		//newMesh.colors = colors;
		newMesh.RecalculateNormals ();
		newMesh.RecalculateBounds ();

		meshHolder.GetComponent<MeshFilter> ().mesh = newMesh;
		meshHolder.GetComponent<MeshRenderer> ().material.mainTexture = map;
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
