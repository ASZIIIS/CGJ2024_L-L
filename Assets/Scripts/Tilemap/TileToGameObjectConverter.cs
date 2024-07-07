using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileToGameObjectConverter : MonoBehaviour
{
    public Tilemap tilemap; // 需要手动将Tilemap对象拖拽到此字段
    public GameObject tilePrefab; // 用于生成瓦片的预制件
    public Transform parentTransform; // 指定的父级GameObject

    private List<GameObject> tileGameObjects = new List<GameObject>(); // 存储生成的GameObject
    [Button("储存level地块")]
    public void ReGenerateGridGO()
    {
        ConvertTilesToGameObjects();
    }
    void Start()
    {
        ConvertTilesToGameObjects();
    }

    void ConvertTilesToGameObjects()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int localPlace = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);
                TileBase tile = tilemap.GetTile(localPlace);
                if (tile != null)
                {
                    CreateTileGameObject(localPlace, tile);
                }
            }
        }
    }

    void CreateTileGameObject(Vector3Int position, TileBase tile)
    {
        Vector3 worldPosition = tilemap.CellToWorld(position);
        GameObject tileGO = Instantiate(tilePrefab, worldPosition, Quaternion.identity, parentTransform);
        tileGO.name = "Tile_" + position.x + "_" + position.y;
        tileGameObjects.Add(tileGO); // 将生成的GameObject添加到列表中
        SpriteRenderer spriteRenderer = tileGO.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Tile tileData = tile as Tile;
            if (tileData != null)
            {
                spriteRenderer.sprite = tileData.sprite;
            }
        }
    }

    // 这里可以添加其他方法来管理生成的GameObject，例如清理、更新等
    public void ClearGeneratedObjects()
    {
        foreach (GameObject obj in tileGameObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj); // 销毁生成的GameObject
            }
        }
        tileGameObjects.Clear(); // 清空列表
    }
}
