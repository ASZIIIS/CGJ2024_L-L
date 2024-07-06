using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour
{
    public Tilemap tilemap; // 需要手动将Tilemap对象拖拽到此字段
    public GameObject[] tilePrefabs; // 用于生成Tile的GameObject预制件数组
    public float noiseScale1 = 0.1f; // 噪声1的缩放
    public float noiseScale2 = 0.05f; // 噪声2的缩放
    public float noiseWeight1 = 0.5f; // 噪声1的权重
    public float noiseWeight2 = 0.5f; // 噪声2的权重

    private Vector2 randomOffset1;
    private Vector2 randomOffset2;

    private List<GameObject> generatedObjects = new List<GameObject>(); // 存储生成的GameObject
    private GameObject currentParentObject; //
    public GameObject[] smallObjectsPrefabs; // 用于生成小物体的预制件数组
    public float smallObjectProbability = 0.2f; // 小物体生成的概率

    void Start()
    {
        GenerateOffsetsAndNoise();
        GenerateGameObjectsFromTiles();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearGeneratedObjects();
            GenerateOffsetsAndNoise();
            GenerateGameObjectsFromTiles();
        }
    }

    void GenerateOffsetsAndNoise()
    {
        // 初始化随机偏移
        randomOffset1 = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));
        randomOffset2 = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

        // 动态调整噪声参数以增加变化
        noiseScale1 = Random.Range(0.01f, 0.2f);
        noiseScale2 = Random.Range(0.01f, 0.2f);
        noiseWeight1 = Random.Range(0.3f, 0.7f);
        noiseWeight2 = 1 - noiseWeight1;
    }

    void GenerateGameObjectsFromTiles()
    {
        // 创建一个新的父级Object
        currentParentObject = new GameObject("GeneratedObjects_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        currentParentObject.transform.parent = this.transform;

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

                    // 使用两个Perlin噪声生成带有更多变化的地形，并引入随机偏移
                    float noiseValue1 = Mathf.PerlinNoise((x + randomOffset1.x) * noiseScale1, (y + randomOffset1.y) * noiseScale1);
                    float noiseValue2 = Mathf.PerlinNoise((x + randomOffset2.x) * noiseScale2, (y + randomOffset2.y) * noiseScale2);
                    float combinedNoiseValue = noiseValue1 * noiseWeight1 + noiseValue2 * noiseWeight2;

                    int prefabIndex = Mathf.FloorToInt(combinedNoiseValue * tilePrefabs.Length);
                    if (prefabIndex >= tilePrefabs.Length)
                    {
                        prefabIndex = tilePrefabs.Length - 1;
                    }

                    GameObject randomPrefab = tilePrefabs[prefabIndex];
                    GameObject tileGO = Instantiate(randomPrefab, place, Quaternion.identity, currentParentObject.transform);
                    tileGO.name = "Tile_" + localPlace.x + "_" + localPlace.y;

                    generatedObjects.Add(tileGO); // 将生成的GameObject添加到列表中
                    if (Random.value < smallObjectProbability)
                    {
                        int smallObjectIndex = Random.Range(0, smallObjectsPrefabs.Length);
                        GameObject smallObjectPrefab = smallObjectsPrefabs[smallObjectIndex];
                        Vector3 smallObjectPosition = place + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
                        GameObject smallObject = Instantiate(smallObjectPrefab, smallObjectPosition, Quaternion.identity, tileGO.transform);
                        smallObject.name = "SmallObject_" + localPlace.x + "_" + localPlace.y;
                    }
                }
            }
        }
    }

    void ClearGeneratedObjects()
    {
        foreach (GameObject obj in generatedObjects)
        {
            Destroy(obj); // 销毁生成的GameObject
        }
        generatedObjects.Clear(); // 清空列表

        if (currentParentObject != null)
        {
            Destroy(currentParentObject); // 销毁父级Object
        }
    }
}
