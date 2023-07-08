using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainTile : Tile
{
    public float groundHeight;
    private TilemapCollider2D collider;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        collider = go.GetComponent<TilemapCollider2D>();
        groundHeight = (go.transform.position.y + go.transform.localScale.y / 2);
        return base.StartUp(position, tilemap, go);
    }
}
