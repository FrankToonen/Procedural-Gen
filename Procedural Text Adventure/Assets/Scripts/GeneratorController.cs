using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorController : MonoBehaviour
{
    public float meshWidth = 10, meshHeight = 10;
    public int width, height;

    List<MeshGenerator> generators;

    void Start()
    {
        generators = new List<MeshGenerator>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            MeshGenerator generator = child.GetComponent<MeshGenerator>();
            Vector3 newPos = child.position;
            newPos.x *= meshWidth - (meshWidth / generator.width);
            newPos.z *= meshHeight - (meshHeight / generator.height);
            child.position = newPos;

            generators.Add(child.GetComponent<MeshGenerator>());
        }

        Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generate();
        }
    }

    void Generate()
    {
        foreach (MeshGenerator g in generators)
        {
            g.GenerateGrid();
        }

        MergeEdges();

        foreach (MeshGenerator g in generators)
        {
            for (int i = 0; i < g.interpolateTimes; i++)
            {
                g.InterpolateY();
            }
            g.CreateMesh(meshWidth, meshHeight);
            g.VisualizeGrid();
        }
    }

    void MergeEdges()
    {
        for (int i = 0; i < generators.Count - 1; i++)
        {
            MeshGenerator g1 = generators[i];
            MeshGenerator g2 = (i + 1) % width > 0 ? generators[i + 1] : null;
            MeshGenerator g3 = (i + width) < generators.Count ? generators[i + width] : null;


            if (g2 != null)
            {
                if (g1.height == g2.height)
                {
                    for (int j = 0; j < g1.height; j++)
                    {
                        Tile t1 = g1.grid[j, g1.width - 1];
                        Tile t2 = g2.grid[j, 0];

                        float midpoint = (t1.y + t2.y) / 2;

                        t1.y = midpoint;
                        t2.y = midpoint;
                    }
                }
            }

            if (g3 != null)
            {
                if (g1.width == g3.width)
                {
                    for (int j = 0; j < g1.width; j++)
                    {
                        Tile t1 = g1.grid[g1.width - 1, j];
                        Tile t3 = g3.grid[0, j];

                        float midpoint = (t1.y + t3.y) / 2;

                        t1.y = midpoint;
                        t3.y = midpoint;
                    }
                }
            }
        }
    }
}
