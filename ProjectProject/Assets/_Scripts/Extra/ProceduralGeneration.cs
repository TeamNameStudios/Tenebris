//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class ProceduralGeneration : MonoBehaviour
//{
//    [SerializeField] int width;
//    [SerializeField] int height;

//    [SerializeField] TileBase groundTile; // for the rule tile

//    [SerializeField] Tilemap groundTilemap;

//    private int[,] map;

//    private void Start()
//    {
//        map = GenerateArray(width, height, false);
        
//        RenderMap(map, groundTilemap, groundTile);
//    }

//    public int[,] GenerateArray(int width, int height, bool empty)
//    {
//        int[,] map = new int[width, height];

//        for (int x = 0; x < map.GetUpperBound(0); x++)
//        {
//            for (int y = 0; y < map.GetUpperBound(1); y++)
//            {
//                if (empty)
//                {
//                    map[x, y] = 0;
//                }
//                else if(!empty)
//                {
//                    map[x, y] = 1;
//                }
//            }
//        }
        
//        return map;
//    }

//    public void RenderMap(int[,] map, Tilemap groundTileMap, TileBase groundTileBase)
//    {
//        for (int x = 0; x < map.GetUpperBound(0); x++)
//        {
//            for (int y = 0; y < map.GetUpperBound(1); y++)
//            {
//                if (map[x,y] == 1)
//                {
//                    groundTileMap.SetTile(new Vector3Int(x, y, 0), groundTileBase);
//                }
//            }
//        }

//    }

//}
