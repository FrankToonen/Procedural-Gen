using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour
{
	public Vector3 velocity;
	public float mass;

	public void Initialize (Vector3 _velocity, float _mass)
	{
		velocity = _velocity * 15;
		mass = _mass * 8;
	}

	void LateUpdate ()
	{
		transform.position += velocity * Time.deltaTime;
	}

	public void UpdateVelocity ()
	{
		foreach (Planet p in PlanetController.instance.planets) {
			float distance = Vector3.Distance (transform.position, p.transform.position);
			if (distance != 0) {
				float gPull = (mass * p.mass) / Mathf.Pow (distance, 2);
				Vector3 direction = (p.transform.position - transform.position).normalized;
				velocity += (direction * gPull);
			}
		}

		/*
		Planet sun = PlanetController.instance.planets [0];
		float gPull = (mass * sun.mass) / Mathf.Pow (Vector3.Distance (sun.transform.position, transform.position), 2);
		Vector3 direction = (PlanetController.instance.planets [0].transform.position - transform.position).normalized;
		velocity += (direction * gPull);*/
	}
}
