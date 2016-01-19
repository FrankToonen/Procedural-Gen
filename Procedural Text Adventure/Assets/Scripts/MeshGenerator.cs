using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : TextureGenerator
{
    public float meshWidth = 10, meshHeight = 10;
    public float treeChance = 0.1f;
    public float houseChance = 0.4f;
    public float boatChance = 0.01f;

    List<Vector3> vertexList;
    List<GameObject> instantiatedObjects;

    protected override void VisualizeGrid()
    {
        base.VisualizeGrid();

        vertexList = new List<Vector3>();

        if (instantiatedObjects != null)
        {
            foreach (GameObject obj in instantiatedObjects)
            {
                Destroy(obj);
            }
        }
        instantiatedObjects = new List<GameObject>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {

                Tile tile = grid[x, z];

                float yValue = tile.y < possibleYValues[2] ? possibleYValues[2] * noiseIntensity : tile.y * noiseIntensity;
                vertexList.Add(new Vector3(x * (meshWidth / width), yValue, z * (meshHeight / height)));

                if (tile.type == Tile.TileType.Grass && Random.value < treeChance)
                {
                    GameObject newTree = Instantiate(Resources.Load<GameObject>("Prefabs/TreePrefab"), vertexList[vertexList.Count - 1], Quaternion.identity) as GameObject;
                    newTree.transform.parent = display.transform;
                    instantiatedObjects.Add(newTree);
                }
                else if (tile.type == Tile.TileType.Village && Random.value < houseChance)
                {
                    GameObject newHouse = Instantiate(Resources.Load<GameObject>("Prefabs/HousePrefab"), vertexList[vertexList.Count - 1], Quaternion.identity) as GameObject;
                    newHouse.transform.parent = display.transform;
                    newHouse.transform.localScale += Vector3.up * (.2f * Random.value);
                    instantiatedObjects.Add(newHouse);
                }
                else if (tile.type == Tile.TileType.VillageBorder)
                {
                    GameObject newBorder = Instantiate(Resources.Load<GameObject>("Prefabs/BorderPrefab"), vertexList[vertexList.Count - 1], Quaternion.identity) as GameObject;
                    newBorder.transform.parent = display.transform;
                    instantiatedObjects.Add(newBorder);
                }
                else if (tile.type == Tile.TileType.Water && Random.value < boatChance)
                {
                    GameObject newBoat = Instantiate(Resources.Load<GameObject>("Prefabs/BoatPrefab"), vertexList[vertexList.Count - 1], Quaternion.identity) as GameObject;
                    newBoat.transform.parent = display.transform;
                    instantiatedObjects.Add(newBoat);
                }
            }
        }

        CreateMesh();
    }

    void CreateMesh()
    {
        Vector3[] vertices = vertexList.ToArray();

        JitterVertices(ref vertices);

        int[] triangles = FindTriangles(vertices.Length, vertices);

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / meshWidth, vertices[i].z / meshHeight);
        }

        Mesh newMesh = new Mesh();
        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.uv = uvs;
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        display.GetComponent<MeshFilter>().mesh = newMesh;
        display.GetComponent<MeshRenderer>().material.mainTexture = gridTexture;
    }

    int[] FindTriangles(int vertices, Vector3[] v)
    {
        int[] triangles = new int[(width - 1) * (height - 1) * 2 * 3];

        int index = 0;
        for (int i = 0; i < vertices - height; i++)
        {

            // Triangle 1
            if (i % height < height - 1)
            {
                triangles[index++] = i;
                triangles[index++] = i + 1;
                triangles[index++] = i + height;
            }

            // Triangle 2
            if (i % height > 0)
            {
                triangles[index++] = i;
                triangles[index++] = i + height;
                triangles[index++] = i + height - 1;
            }
        }

        return triangles;
    }

    void JitterVertices(ref Vector3[] vertices)
    {
        float w = (meshWidth / width) / 4;
        float h = (meshHeight / height) / 4;

        for (int i = 0; i < vertices.Length; i++)
        {
			
            if (!IsBoundary(i))
            {
                Vector3 v = vertices[i];
                v.x += (Random.value * 2 - 1) * w;
                v.z += (Random.value * 2 - 1) * h;
                vertices[i] = v;
            }
        }
    }
}
