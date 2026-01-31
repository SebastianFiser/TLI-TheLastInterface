using UnityEngine;
using UnityEngine.Tilemaps;

public class worldGenSkrpt : MonoBehaviour
{
    [Header("Block Specs")]
    public Tilemap mojemapa;
    public TileBase hlinaTrava;
    public TileBase hlinaHlina;
    [Header("CordSpecs")]
    public int xRange = 20;
    public int yRange = -15;
    public int grassLevel = -5;
    [Header("tileSpecs")]
    private int[,] mapa;
    private int sirka;
    private int vyska;
    [Range(0, 100)] public int fillPercent = 45;
    public int neededNeighbors = 4;

    void Start()
    {
        GenerujMapu();

        vyhladMapu();
        vyhladMapu();
        vyhladMapu();

        vykresliMapu();
    }

    private void GenerujMapu()
    {
        sirka = xRange * 2;
        vyska = Mathf.Abs(yRange - grassLevel);
        mapa = new int[sirka, vyska];
        NaplnMapuNahodne();
    }

    private void NaplnMapuNahodne()
    {
        for (int x = 0; x < sirka; x++)
            for (int y = 0; y < vyska; y++)
            {
                int selectedNum = Random.Range(0, 100);
                if (selectedNum < fillPercent)
                {
                    mapa[x, y] = 1; // Tady doplň 1
                }
                else
                {
                    mapa[x, y] = 0; // Tady doplň 0
                }
            }
    }

    private void vyhladMapu()
    {
        int[,] novaMapa = new int[sirka, vyska];
        for (int x = 0; x < sirka; x++)
            for (int y = 0; y < vyska; y++)
            {
                int s = GetSurroundingWallCount(x, y);
                if (s > neededNeighbors)
                    novaMapa[x, y] = 1;
                else if (s < neededNeighbors)
                    novaMapa[x, y] = 0;
                else
                    novaMapa[x, y] = mapa[x, y];
            }
        mapa = novaMapa;
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallcount = 0;
        for (int x = gridX - 1; x <= gridX + 1; x++)
            for (int y = gridY - 1; y <= gridY + 1; y++)
            {
                if (x >= 0 && x < sirka && y >= 0 && y < vyska)
                {
                    if (gridX != x || gridY != y)
                        wallcount += mapa[x, y];
                }
                else
                    wallcount++;
            }
        return wallcount;
    }

    private void vykresliMapu()
    {
        for (int x = 0; x < sirka; x++)
        {
            for (int y = 0; y < vyska; y++)
            {
                Vector3Int pozice = new Vector3Int(x - xRange, y + yRange, 0);

                if (mapa[x, y] == 1)
                    mojemapa.SetTile(pozice, hlinaHlina);
                else
                    mojemapa.SetTile(pozice, null);
            }
        }
    }

}

