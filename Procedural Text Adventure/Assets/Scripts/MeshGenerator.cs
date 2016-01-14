using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : TextureGenerator
{
	public float meshWidth = 10, meshHeight = 10;
	List<Vector3> vertexList;

	protected override void VisualizeGrid ()
	{
		base.VisualizeGrid ();

		vertexList = new List<Vector3> ();

		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {

				Tile tile = grid [x, z];

				float yValue = tile.y < possibleYValues [1] ? possibleYValues [1] * noiseIntensity : tile.y * noiseIntensity;

				vertexList.Add (new Vector3 (x * (meshWidth / width), yValue, z * (meshHeight / height)));
			}
		}

		CreateMesh ();
	}

	void CreateMesh ()
	{
		Vector3[] vertices = vertexList.ToArray ();


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
		newMesh.RecalculateNormals ();
		newMesh.RecalculateBounds ();

		display.GetComponent<MeshFilter> ().mesh = newMesh;
		display.GetComponent<MeshRenderer> ().material.mainTexture = gridTexture;
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
}
