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
    [SerializeField] private int grassLevel = -5;
    [SerializeField] private int groundLevel = 0;

    [Header("Cellular Automata Settings")]
    [Range(0, 100)] [SerializeField] private int fillPercent = 45;
    [SerializeField] private int smoothIterations = 3;
    [SerializeField] private int neighborThreshold = 4;

    [Header("Hills Detail")]
    [SerializeField] private float seed = 0.0f;
    [SerializeField] private float PrimaryScale = 0.01f;
    [SerializeField] private float SecondarScale = 0.08f;

    private int[,] mapGrid;
    private int width;
    private int height;

    private void Start()
    {
        SetASeed();
        GenerateWorld();

    }

    private void SetASeed()
    {
        seed = UnityEngine.Random.Range(0f, 100000f);
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
        AddGrass();
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
            for (int y = undergroundDepth - crustSize; y < undergroundDepth; y++)
            {
                mapGrid[x, y] = 1;
            }
        }
    }

    private void GenerateHills()
    {
        int MountMaxHeight = 30;
        //function for Hill generations using perlín noise
        for (int x = 0; x < width; x++)
        {

            float PrimaryNoise = Mathf.PerlinNoise((x + seed) * PrimaryScale, 0f) * 1.5f;
            float SecondaryNoise = Mathf.PerlinNoise((x + seed) * SecondarScale, 0f) * 0.2f;
            float finalNoise = PrimaryNoise + SecondaryNoise;

            int currentHillTop = undergroundDepth + Mathf.RoundToInt(finalNoise * MountMaxHeight);

            currentHillTop = Mathf.Clamp(currentHillTop, 0, height - 1);

           for (int y = undergroundDepth; y < currentHillTop; y++)
           {
                mapGrid[x, y] = 1;
           }
        }
    }

    private void AddGrass()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                // 1. Najdeme první blok hlíny (shora dolů)
                if (mapGrid[x, y] == 1)
                {
                    // 2. Uděláme z něj trávu
                    mapGrid[x, y] = 2;

                    // 3. KLÍČOVÝ KROK: Zastavíme hledání v tomto sloupci!
                    // Tím zajistíme, že se tráva neudělá nikde hlouběji.
                    break;
                }
            }
        }
    }

    private void DrawMap()
    {
        targetTilemap.ClearAllTiles();
        int StartY = grassLevel - undergroundDepth;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Reconstructing the offset position for Tilemap
                Vector3Int tilePosition = new Vector3Int(x - xRange, y + StartY, 0);

                if (mapGrid[x, y] == 1)
                {
                    targetTilemap.SetTile(tilePosition, dirtTile);
                }
                else if (mapGrid[x, y] ==2)
                    targetTilemap.SetTile(tilePosition, grassTile);
                else
                {
                    targetTilemap.SetTile(tilePosition, null);
                }
            }
        }
    }
}
