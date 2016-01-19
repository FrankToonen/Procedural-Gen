using UnityEngine;
using System.Collections;

public class Tile
{
    public enum TileType
    {
        Grass,
        Mountain,
        Sand,
        Water,
        Village,
        VillageBorder,
        Road
    }

    public TileType type;
    public int x, z;
    public float y;
    public Color color;

    public Tile(TileType _type, int _x, float _y, int _z, Color _color)
    {
        type = _type;
        x = _x;
        y = _y;
        z = _z;

        color = _color;

    }
}
