using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
	public int x, z;
	public float y;

	public void Initialize (int _x, float _y, int _z)
	{
		x = _x;
		y = _y;
		z = _z;

		GetComponent<MeshRenderer> ().material.color = new Color (0, y, 0);
	}
}
