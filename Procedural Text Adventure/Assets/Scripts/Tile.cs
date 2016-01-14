using UnityEngine;
using System.Collections;

public class Tile
{
	public int x, z;
	public float y;
	public Color color;

	public Tile (int _x, float _y, int _z, Color _color)
	{
		x = _x;
		y = _y;
		z = _z;

		color = _color;
	}
}
