using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public int width = 25, height = 25;
    public float noiseSmoothing = 1, noiseIntensity = 1;
    public float[] possibleYValues;
    public Tile[,] grid;
    public int seed = 100;
    public int interpolateTimes = 1;
    public int villageCount = 5, villageSize = 5;

    protected float noiseOffset;

    protected virtual void Start()
    {
        Random.seed = seed;

        GenerateGrid();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateGrid();
        }
    }

    protected void GenerateGrid()
    {
        grid = new Tile[width, height];
        noiseOffset = Random.Range(0, 10000);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float noise = Mathf.PerlinNoise((x + noiseOffset) / width / noiseSmoothing, (z + noiseOffset) / height / noiseSmoothing);
                float y = util.ClampToNearest(noise, possibleYValues);
                Tuple t = GetColor(x, y, z);
                Color c = t.color;
                grid[x, z] = new Tile(t.type, x, y, z, c);
            }
        }

        for (int i = 0; i < interpolateTimes; i++)
        {
            InterpolateY();
        }

        PopulateMap();
    }

    Tuple GetColor(float x, float y, float z)
    {
        float r = 0;
        float g = 0;
        float b = 0;
        var type = Tile.TileType.Grass;

        if (y <= possibleYValues[1])
        {
            b = .5f;
            type = Tile.TileType.Water;
        }
        else if (y <= possibleYValues[2])
        {
            b = 1;
            type = Tile.TileType.Water;
        }
        else if (y <= possibleYValues[3])
        {
            r = 1;
            g = 1;
            b = .5f;
            type = Tile.TileType.Sand;
        }
        else if (y <= possibleYValues[4])
        {
            g = 1;
            type = Tile.TileType.Grass;
        }
        else if (y <= possibleYValues[5])
        {
            g = .5f;
            type = Tile.TileType.Mountain;
        }

        return new Tuple(type, new Color(r, g, b));
    }

    void InterpolateY()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Tile tile = grid[x, z];
                float average = tile.y;
                int amount = 1;

                for (int u = -1; u <= 1; u++)
                {
                    for (int v = -1; v <= 1; v++)
                    {
                        int xI = x + u;
                        int yI = z + v;

                        if (xI < 0 || xI >= width || yI < 0 || yI >= height)
                        {
                            continue;
                        }

                        average += grid[xI, yI].y;
                        amount++;
                    }
                }

                float newY = average / amount;
                tile.y = newY;

                Tuple t = GetColor(tile.x, tile.y, tile.z);
                tile.color = t.color;
                tile.type = t.type;
            }
        }
    }

    void PopulateMap()
    {
        SetVillages();
        // Meer???
    }

    void SetVillages()
    {
        int count = Random.Range(villageCount, villageCount * 2);
        int size = Random.Range(villageSize, villageSize * 2);
        List<Vector3> villagePoints = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            villagePoints.Add(GetRandomPosition(0, 0.1f, 0));
        }

        grid = BleedGenerator.BleedPoints(grid, villagePoints, size, Color.gray);
    }

    Vector3 GetRandomPosition(float r = 0, float g = 0, float b = 0)
    {
        Vector3 pos = new Vector3(Random.Range(0, width), 0, Random.Range(0, height));
        Color c = grid[(int)pos.x, (int)pos.z].color;
        if ((r > 0 && r > c.r) || (g > 0 && g > c.g) || (b > 0 && b > c.b))
        {
            return GetRandomPosition(r, g, b);
        }
        else
        {
            pos.y = grid[(int)pos.x, (int)pos.z].y;
            return pos;
        }
    }

    protected bool IsBoundary(int i)
    {
        return (i % height == width - 1) || (i % height == 0) || (i < width) || (i > (width - 1) * height);
    }

    public class Tuple
    {
        public Tile.TileType type;
        public Color color;

        public Tuple(Tile.TileType _type, Color _color)
        {
            type = _type;
            color = _color;
        }
    }
}
