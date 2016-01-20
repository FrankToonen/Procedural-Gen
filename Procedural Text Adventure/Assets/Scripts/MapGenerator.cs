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
    bool[,] villageConnections;

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
        villageConnections = new bool[villagePoints.Count, villagePoints.Count];

        for (int i = 0; i < villagePoints.Count; i++)
        {
            Vector3 v = villagePoints[i];

            // Insertion sort
            List<Vector3> sortedByDistance = new List<Vector3>();
            for (int j = 0; j < villagePoints.Count; j++)
            {
                Vector3 nV = villagePoints[j];
                bool added = false;
                for (int k = 0; k < sortedByDistance.Count; k++)
                {
                    Vector3 oV = sortedByDistance[k];
                    if (Vector3.Distance(nV, v) < Vector3.Distance(oV, v))
                    {
                        sortedByDistance.Insert(k, nV);
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    sortedByDistance.Add(nV);
                }
            }

            // Create connections
            for (int j = 1; j < 3; j++)
            {        
                int destinationIndex = villagePoints.IndexOf(sortedByDistance[j]);
                villageConnections[i, destinationIndex] = true;
                villageConnections[destinationIndex, i] = true;
            }
        }

        CreatePaths();
    }

    void CreatePaths()
    {
        for (int i = 0; i < villageConnections.GetLength(0); i++)
        {
            for (int j = 0; j < villageConnections.GetLength(1); j++)
            {
                if (j < i || !villageConnections[i, j])
                {
                    continue;
                }

                Vector3 s = villagePoints[i];
                Vector3 d = villagePoints[j];

                float xDistance = s.x - d.x;
                float zDistance = s.z - d.z;

                int xI = (int)s.x;
                int zI = (int)s.z;
                while (Mathf.Abs(xDistance) > 0 || Mathf.Abs(zDistance) > 0)
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
                    grid[xI, zI].color = new Color(.5f, .4f, .1f);
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
