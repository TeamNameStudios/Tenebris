using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTerrainGeneration : Singleton<TileTerrainGeneration>
{
    public int width = 60;
    public Tilemap groundTilemap;
    public Grid gridPrefab;
    public Tile groundTile;

    [Range(0, 100)] public float heightValue, smoothness;

    public float seed;

    public void GenerateTilemap(Transform parentTransform, Tilemap tilemap)
    {
        seed = Random.Range(-500, 500);

        for (int x = 0; x < width; x++)
        {
            int height = Mathf.RoundToInt(heightValue * Mathf.PerlinNoise(x / smoothness, seed));

            for (int y = 0; y < height; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                //_grid.transform.SetParent(parentTransform);
                //_grid.transform.GetChild(0).position = parentTransform.position;
            }
        }
    }

    public Tilemap CreateTilemap(Transform parentTransform)
    {
        Grid grid = Instantiate(gridPrefab, new Vector3(parentTransform.position.x - 30f, parentTransform.position.y), Quaternion.identity);
        grid.transform.SetParent(parentTransform);
        grid.transform.GetChild(0).position = grid.transform.position + new Vector3(0, -16f, 0);  // -16

        Tilemap tilemap = grid.GetComponentInChildren<Tilemap>();

        return tilemap;
    }
}