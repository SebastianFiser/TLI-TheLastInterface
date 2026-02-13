using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Handles procedural generation of the world map using cellular automata.
/// </summary>
public class WorldGenerator : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase dirtTile;

    [Header("Generation Settings")]
    [Tooltip("Total width of the map (centered around 0).")]
    [SerializeField] private int xRange = 20;
    [SerializeField] private int totalHeight = 500;
    [SerializeField] private int undergroundDepth = 300;
    [SerializeField] private int crustSize = 5;

    [Header("Cellular Automata Settings")]
    [Range(0, 100)] [SerializeField] private int fillPercent = 45;
    [SerializeField] private int smoothIterations = 3;
    [SerializeField] private int neighborThreshold = 4;

    private int[,] mapGrid;
    private int width;
    private int height;

    private void Start()
    {
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        width = xRange * 2;
        height = totalHeight;
        mapGrid = new int[width, height];

        InitializeMapRandomly();

        for (int i = 0; i < smoothIterations; i++)
        {
            SmoothMap();
        }

        CreateWorldCrust();
        GenerateHills();
        DrawMap();
    }

    private void InitializeMapRandomly()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Only fill tiles below the underground depth
                if (y < undergroundDepth)
                {
                    mapGrid[x, y] = (Random.Range(0, 100) < fillPercent) ? 1 : 0;
                }
                else
                {
                    mapGrid[x, y] = 0;
                }
            }
        }
    }

    private void SmoothMap()
    {
        int[,] smoothedMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborWallCount = GetSurroundingWallCount(x, y);

                if (neighborWallCount > neighborThreshold)
                    smoothedMap[x, y] = 1;
                else if (neighborWallCount < neighborThreshold)
                    smoothedMap[x, y] = 0;
                else
                    smoothedMap[x, y] = mapGrid[x, y];
            }
        }

        mapGrid = smoothedMap;
    }

    private int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;

        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                // Check if the neighbor is within map boundaries
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if (neighborX != gridX || neighborY != gridY)
                    {
                        wallCount += mapGrid[neighborX, neighborY];
                    }
                }
                else
                {
                    // Treat out-of-bounds as walls to create solid borders
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    private void CreateWorldCrust()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = height - crustSize; y < height; y++)
            {
                mapGrid[x, y] = 1;
            }
        }
    }

    private void GenerateHills()
    {
        //function for Hill generations using perlín noise
    }

    private void DrawMap()
    {
        targetTilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Reconstructing the offset position for Tilemap
                Vector3Int tilePosition = new Vector3Int(x - xRange, y + undergroundDepth, 0);

                if (mapGrid[x, y] == 1)
                {
                    targetTilemap.SetTile(tilePosition, dirtTile);
                }
                else
                {
                    targetTilemap.SetTile(tilePosition, null);
                }
            }
        }
    }
}
