using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class BleedGenerator
{
    /*static BleedPointInfo GetBleedPoint (int width, int height)
	{
		Vector2 newPoint = new Vector2 (Random.Range (0, width - 1), Random.Range (0, height - 1));
		foreach (BleedPointInfo bpi in bleedPoints) {
			if (bpi.position == newPoint) {
				return GetBleedPoint ();
			}
		}

		return new BleedPointInfo (newPoint, maxBleed);
	}*/

    public static Tile[,] BleedPoints(Tile[,] grid, List<Vector3> points, int maxBleed, Color tileColor, float bleedChance = 0.5f)
    {
        BleedPointInfo[,] bleedGrid = new BleedPointInfo[grid.GetLength(0), grid.GetLength(0)];
        foreach (Vector3 p in points)
        {
            BleedPointInfo bpi = new BleedPointInfo(p, maxBleed);
            bleedGrid[(int)p.x, (int)p.z] = bpi;
        }
        BleedPointInfo[,] newBleedGrid = bleedGrid.Clone() as BleedPointInfo[,];

        // Bleed bleedpoints until everything stopped bleeding
        bool isStillBleeding = true;
        while (isStillBleeding)
        {
            bool isBleeding = false;

            for (int x = 0; x < bleedGrid.GetLength(0); x++)
            {
                for (int y = 0; y < bleedGrid.GetLength(1); y++)
                {
                    BleedPointInfo bpi = bleedGrid[x, y];
                    if (bpi != null && bpi.bloodLeft > 0)
                    {
                        if (x - 1 > 0 && Random.value < bleedChance)
                        { // left
                            if (newBleedGrid[x - 1, y] == null)
                            {
                                newBleedGrid[x - 1, y] = new BleedPointInfo(new Vector3(x - 1, bpi.position.y, y), bpi.bloodLeft - 1);
                            }
                        }

                        if (x + 1 < bleedGrid.GetLength(0) && Random.value < bleedChance)
                        { // right
                            if (newBleedGrid[x + 1, y] == null)
                            {
                                newBleedGrid[x + 1, y] = new BleedPointInfo(new Vector3(x + 1, bpi.position.y, y), bpi.bloodLeft - 1);
                            }
                        }

                        if (y - 1 > 0 && Random.value < bleedChance)
                        { // up
                            if (newBleedGrid[x, y - 1] == null)
                            {
                                newBleedGrid[x, y - 1] = new BleedPointInfo(new Vector3(x, bpi.position.y, y - 1), bpi.bloodLeft - 1);
                            }
                        }

                        if (y + 1 < bleedGrid.GetLength(1) && Random.value < bleedChance)
                        { // down
                            if (newBleedGrid[x, y + 1] == null)
                            {
                                newBleedGrid[x, y + 1] = new BleedPointInfo(new Vector3(x, bpi.position.y, y + 1), bpi.bloodLeft - 1);
                            }
                        }

                        bpi.bloodLeft--;
                        isBleeding = true;
                    }
                }
            }

            bleedGrid = newBleedGrid.Clone() as BleedPointInfo[,];

            isStillBleeding = isBleeding;
        }

        // Convert BleedPoint grid to Tile grid
        for (int x = 0; x < bleedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < bleedGrid.GetLength(1); y++)
            {
                BleedPointInfo bpi = bleedGrid[x, y];
                if (bpi != null)
                {
                    grid[x, y] = new Tile(Tile.TileType.Village, (int)bpi.position.x, grid[x, y].y, (int)bpi.position.z, tileColor);
                }
            }
        }

        // Mark village borders
        for (int x = 0; x < bleedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < bleedGrid.GetLength(1); y++)
            {
                Tile tile = grid[x, y];
                if (tile.type == Tile.TileType.Village)
                {

                    bool border = false;

                    for (int u = -1; u <= 1; u++)
                    {
                        for (int v = -1; v <= 1; v++)
                        {

                            if ((u + v) % 2 == 0)
                            {
                                continue;
                            }

                            int xI = x + u;
                            int yI = y + v;

                            if (xI < 0 || xI >= grid.GetLength(0) || yI < 0 || yI >= grid.GetLength(1))
                            {
                                continue;
                            }

                            border = border || (grid[xI, yI].type != Tile.TileType.Village && grid[xI, yI].type != Tile.TileType.VillageBorder);    
                        }
                    }

                    if (border)
                    {
                        tile.type = Tile.TileType.VillageBorder;
                    }
                }
            }
        }

        return grid;
    }

    public class BleedPointInfo
    {
        public Vector3 position;
        public int bloodLeft;
        public int maxBlood;

        public BleedPointInfo(Vector3 _position, int max)
        {
            position = _position;
            bloodLeft = max;
            maxBlood = max;
        }
    }
}
