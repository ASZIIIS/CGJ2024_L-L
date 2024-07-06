using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour
{
    public Tilemap tilemap; // ��Ҫ�ֶ���Tilemap������ק�����ֶ�
    public GameObject[] tilePrefabs; // ��������Tile��GameObjectԤ�Ƽ�
   
    void Start()
    {
        GenerateGameObjectsFromTiles();
    }

    void GenerateGameObjectsFromTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Vector3Int localPlace = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);
                    Vector3 place = tilemap.CellToWorld(localPlace);
                    GameObject randomPrefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
                    GameObject tileGO = Instantiate(randomPrefab, place, Quaternion.identity);
                    tileGO.name = "Tile_" + localPlace.x + "_" + localPlace.y;
                }
            }
        }
    }
}
