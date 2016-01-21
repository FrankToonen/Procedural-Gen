using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureGenerator : MapGenerator
{
    protected Texture2D gridTexture;

    /* protected override void Start()
    {
        base.Start();

        VisualizeGrid();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            VisualizeGrid();
        }
    }*/

    public virtual void VisualizeGrid()
    {
        gridTexture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //gridTexture.SetPixel(x, z, grid[x, z].color);

                Tile tile = grid[x, z];
                Tuple t = GetColor(tile.x, tile.y, tile.z);
                Color c = tile.color;

                if (tile.type != Tile.TileType.Village && tile.type != Tile.TileType.VillageBorder && tile.type != Tile.TileType.Road)
                {
                    tile.type = t.type;
                    c = t.color;
                }
                
                gridTexture.SetPixel(x, z, c);
            }
        }
        gridTexture.Apply();

        GetComponent<MeshRenderer>().material.mainTexture = gridTexture;
    }
}
