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
    protected List<Vector3> villagePoints;

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
        CreateRoads();
    }

    void SetVillages()
    {
        int count = Random.Range(villageCount, villageCount * 2);
        int size = Random.Range(villageSize, villageSize * 2);
        villagePoints = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            villagePoints.Add(GetRandomPosition(0, 0.1f, 0));
        }

        grid = BleedGenerator.BleedPoints(grid, villagePoints, size, Color.gray);
    }

    void CreateRoads()
    {
        foreach (Vector3 v in villagePoints)
        {
            List<Vector3> sortedByDistance = new List<Vector3>();
            for (int i = 0; i < villagePoints.Count; i++)
            {
                Vector3 nV = villagePoints[i];
                bool added = false;
                for (int j = 0; j < sortedByDistance.Count; j++)
                {
                    Vector3 oV = sortedByDistance[j];
                    if (Vector3.Distance(nV, v) < Vector3.Distance(oV, v))
                    {
                        sortedByDistance.Insert(j, nV);
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    sortedByDistance.Add(nV);
                }
            }

            for (int i = 0; i < 2; i++)
            {         
                float xDistance = v.x - sortedByDistance[i].x;
                float zDistance = v.z - sortedByDistance[i].z;

                int steps = 0;

                int xI = (int)v.x;
                int zI = (int)v.z;
                while ((Mathf.Abs(xDistance) > 0 || Mathf.Abs(zDistance) > 0) && steps < 1000)
                {
                    int chance = Random.Range(0, 2);
                    switch (chance)
                    {
                        case 0:
                            if (Mathf.Abs(xDistance) > 0)
                            {
                                xI -= (int)Mathf.Sign(xDistance);
                                xDistance -= (int)Mathf.Sign(xDistance);
                            }
                            break;
                        case 1:
                            if (Mathf.Abs(zDistance) > 0)
                            {
                                zI -= (int)Mathf.Sign(zDistance);
                                zDistance -= (int)Mathf.Sign(zDistance);
                            }
                            break;
                    }

                    grid[xI, zI].type = Tile.TileType.Road;

                    steps++;
                }
            }
        }
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
