using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

public class PlanetGenerator
{
	float radius;
	float noiseIntensity;
	float noiseSmoothing;
	int LOD;

	List<Planet> planets;

	List<Vector3> vertices;
	List<Vector3> normals;
	Dictionary<uint,int> newVectices;
	List<int> indices;

	public List<Planet> GeneratePlanets (GameObject startMesh, int amountOfPlanets, float _radius, float _noiseIntensity, float _noiseSmoothing, int _LOD)
	{
		radius = _radius;
		noiseIntensity = _noiseIntensity;
		noiseSmoothing = _noiseSmoothing;
		LOD = _LOD;

		planets = new List<Planet> ();
	
		Planet sun = GeneratePlanet (startMesh);
		sun.transform.position = Vector3.zero;
		sun.transform.parent = PlanetController.instance.transform;
		sun.name = "Sun";
		planets.Add (sun);

		for (int m = 0; m < amountOfPlanets; m++) {
			planets.Add (GeneratePlanet (startMesh));
		}

		return planets;
	}

	Planet GeneratePlanet (GameObject startMesh)
	{
		Vector3 newPosition = GetNewPosition ();
		GameObject newPlanet = GameObject.Instantiate (startMesh, newPosition, Quaternion.identity) as GameObject;
		newPlanet.transform.parent = PlanetController.instance.transform;

		for (int i = 0; i < LOD; i++) {
			SubdivideMesh (newPlanet);
		}
		//SpherifyMesh ();
		ApplyPerlinNoise (newPlanet);

		return newPlanet.GetComponent<Planet> ();
	}

	Vector3 GetNewPosition ()
	{
		Vector3 newPosition = (new Vector3 (Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f)) * 1000;
		foreach (Planet p in planets) {
			float distance = Vector3.Distance (p.transform.position, newPosition);
			if (distance > 0 && distance < radius * 2) {
				return GetNewPosition ();
			}
		}
		return newPosition;
	}

	void SubdivideMesh (GameObject planet)
	{
		Mesh mesh = planet.GetComponent<MeshFilter> ().mesh;

		newVectices = new Dictionary<uint,int> ();

		vertices = new List<Vector3> (mesh.vertices);
		normals = new List<Vector3> (mesh.normals);
		// [... all other vertex data arrays]
		indices = new List<int> ();

		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length; i += 3) {
			int i1 = triangles [i + 0];
			int i2 = triangles [i + 1];
			int i3 = triangles [i + 2];

			int a = GetNewVertex (i1, i2);
			int b = GetNewVertex (i2, i3);
			int c = GetNewVertex (i3, i1);
			indices.Add (i1);
			indices.Add (a);
			indices.Add (c);
			indices.Add (i2);
			indices.Add (b);
			indices.Add (a);
			indices.Add (i3);
			indices.Add (c);
			indices.Add (b);
			indices.Add (a);
			indices.Add (b);
			indices.Add (c); // center triangle
		}
		mesh.vertices = vertices.ToArray ();
		mesh.normals = normals.ToArray ();
		// [... all other vertex data arrays]
		mesh.triangles = indices.ToArray ();

		// since this is a static function and it uses static variables
		// we should erase the arrays to free them:
		newVectices = null;
		vertices = null;
		normals = null;
		// [... all other vertex data arrays]

		indices = null;
	}

	int GetNewVertex (int i1, int i2)
	{
		// We have to test both directions since the edge
		// could be reversed in another triangle
		uint t1 = ((uint)i1 << 16) | (uint)i2;
		uint t2 = ((uint)i2 << 16) | (uint)i1;
		if (newVectices.ContainsKey (t2))
			return newVectices [t2];
		if (newVectices.ContainsKey (t1))
			return newVectices [t1];
		// generate vertex:
		int newIndex = vertices.Count;
		newVectices.Add (t1, newIndex);

		// calculate new vertex
		vertices.Add ((vertices [i1] + vertices [i2]) * 0.5f);
		normals.Add ((normals [i1] + normals [i2]).normalized);
		// [... all other vertex data arrays]

		return newIndex;
	}

	/*void SpherifyMesh (Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;

		for (var i = 0; i < vertices.Length; i++) {
			vertices [i] = vertices [i].normalized * radius;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}*/

	void ApplyPerlinNoise (GameObject planet)
	{
		Mesh mesh = planet.GetComponent<MeshFilter> ().mesh;

		Vector3[] vertices = mesh.vertices;
		Color[] colors = new Color[vertices.Length];

		Perlin noiseGenerator = new Perlin ();

		for (var i = 0; i < vertices.Length; i++) {
			//float xAngle = Vector3.Angle (Vector3.right, vertices [i]) / noiseSmoothing;
			//float yAngle = Vector3.Angle (Vector3.up, vertices [i]) / noiseSmoothing;
			//float noise = Mathf.PerlinNoise (xAngle, yAngle) * noiseIntensity;
			//float noise = Mathf.PerlinNoise (vertices [i].x + vertices [i].y, vertices [i].z + vertices [i].y);

			Vector3 noiseVector = new Vector3 (vertices [i].x + planet.transform.position.x, vertices [i].y + planet.transform.position.y, vertices [i].z + planet.transform.position.z) / noiseSmoothing;

			float noise = (float)noiseGenerator.GetValue (noiseVector.x, noiseVector.y, noiseVector.z);

			vertices [i] = vertices [i].normalized * (radius + noise);
			colors [i] = noise < 0.3f * noiseIntensity ? Color.blue : Color.green;
		}

		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}
}
