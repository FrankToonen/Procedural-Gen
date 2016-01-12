using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetController : MonoBehaviour
{
	public static PlanetController instance;

	public GameObject startMesh;
	public int amountOfPlanets = 5;
	public float radius = 5;
	public float noiseIntensity = .5f;
	public float noiseSmoothing = 5;
	public int LOD = 4;

	public List<Planet> planets;

	void Start ()
	{
		instance = this;
		GeneratePlanets ();
	}

	void GeneratePlanets ()
	{
		if (planets != null) {
			foreach (Planet p in planets) {
				Destroy (p.gameObject);
			}
			planets.Clear ();
		}
		PlanetGenerator generator = new PlanetGenerator ();
		planets = generator.GeneratePlanets (startMesh, amountOfPlanets, radius, noiseIntensity, noiseSmoothing, LOD);
		planets [0].Initialize (Vector3.zero, 20, 0);
		for (int i = 1; i < planets.Count; i++) {
			Planet p = planets [i];

			//float speed = Random.Range (10, 20);
			Vector3 direction = (planets [0].transform.position - p.transform.position).normalized;
			Vector3 velocity = Quaternion.AngleAxis (90, Vector3.up) * direction /** speed*/;

			planets [i].Initialize (velocity, Random.Range (1, 5), i);
		}
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			GeneratePlanets ();
		}

		for (int i = 1; i < planets.Count; i++) {
			planets [i].UpdateVelocity ();
		}

		//Debug.Log (Vector3.Distance (planets [0].transform.position, planets [1].transform.position));
	}

	void OnDrawGizmos ()
	{
		for (int i = 1; i <= 10; i++) {
			float c = 1 - (1f / i);
			Gizmos.color = new Color (c, c, c, .5f);
			Gizmos.DrawWireSphere (Vector3.zero, i * 100);
		}
	}
}
