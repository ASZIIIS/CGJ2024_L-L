using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TileMapGameObjectController : MonoBehaviour
{
    [Header("瓦片")]
    public Tilemap tilemap; // 需要手动将Tilemap对象拖拽到此字段
    public Sprite[] TileSprites; // 用于生成Tile的GameObject预制件数组
    public GameObject TilePrefab;
    public Dictionary<Vector2, GameObject> TileMapData = new Dictionary<Vector2, GameObject>();

    private List<GameObject> generatedObjects = new List<GameObject>(); // 存储生成的GameObject

    public Transform gridParentTransf; //
    [Header("小物体")]
    public GameObject[] smallObjectsPrefabs; // 用于生成小物体的预制件数组

    public float smallObjectProbability = 0.2f; // 小物体生成的概率


    public Vector2 FilpStartPos;
    [FoldoutGroup("随机噪声")]
    public float noiseScale1 = 0.1f; // 噪声1的缩放
    [FoldoutGroup("随机噪声")]
    public float noiseScale2 = 0.05f; // 噪声2的缩放
    [FoldoutGroup("随机噪声")]
    public float noiseWeight1 = 0.5f; // 噪声1的权重
    [FoldoutGroup("随机噪声")]
    public float noiseWeight2 = 0.5f; // 噪声2的权重

    private Vector2 randomOffset1;
    private Vector2 randomOffset2;

    public GameObject directionMarkerPrefab; // 路标预制件

    void Start()
    {
        InstorageTileMapData();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FilpAllTile(FilpStartPos);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            PlaceDirectionMarker();
        }
    }

    void InstorageTileMapData()
    {
        foreach (Transform _gridTransf in gridParentTransf)
        {
            string[] _indexs = _gridTransf.name.Split("_");
            int _index1 = int.Parse(_indexs[1]);
            int _index2 = int.Parse(_indexs[2]);
            TileMapData[new Vector2(_index1, _index2)] = _gridTransf.gameObject;
        }
    }

    #region 动画切换
    public void FilpAllTile(Vector2 _startPos)
    {
        StartCoroutine(FilpAllTileIEnum(_startPos));
    }
    public float flipTimeSplit = 0.5f;
    //4-- -6
    IEnumerator FilpAllTileIEnum(Vector2 _startPos)
    {
        List<Vector2> _stack = new List<Vector2>();
        _stack.Add(_startPos);
        while (_stack != null)
        {
            List<Vector2> _tempStack = new List<Vector2>();
            foreach (var _vector2 in _stack)
            {
                if (TileMapData.ContainsKey(_vector2))
                {
                    TileMapData[_vector2].GetComponent<Animator>().Play("Grid_Filp1");
                }
            }

            yield return new WaitForSeconds(flipTimeSplit);
        }
    }
    #endregion

    #region 地块生成
    [Button("重新生成地面物体")]
    public void ReGenerateGridGO()
    {
        ClearGeneratedObjects();
        GenerateOffsetsAndNoise();
        GenerateGameObjectsFromTiles();
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

                    int prefabIndex = Mathf.FloorToInt(combinedNoiseValue * TileSprites.Length);
                    if (prefabIndex >= TileSprites.Length)
                    {
                        prefabIndex = TileSprites.Length - 1;
                    }

                    GameObject tileGO = Instantiate(TilePrefab, place, Quaternion.identity, gridParentTransf);
                    tileGO.GetComponent<GridSingle>().Init(TileSprites[prefabIndex]);
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

    void PlaceDirectionMarker()
    {
        // 获取鼠标点击的位置
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        // 获取Tile所在的GameObject
        foreach (GameObject tileGO in generatedObjects)
        {
            if (tilemap.WorldToCell(tileGO.transform.position) == cellPos)
            {
                // 在Tile位置放置方向标
                GameObject marker = Instantiate(directionMarkerPrefab, tileGO.transform.position, Quaternion.identity, tileGO.transform);

                // 设置方向（这里可以通过某种方式设置方向，例如通过UI或预定义逻辑）
                GridSingle gridSingle = tileGO.GetComponent<GridSingle>();
                if (gridSingle != null)
                {
                    gridSingle.SetDirection(Directions.RightDown); // 示例方向
                }

                break;
            }
        }
    }

    void ClearGeneratedObjects()
    {
        for (int i = gridParentTransf.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gridParentTransf.GetChild(i).gameObject);
        }

        generatedObjects.Clear(); // 清空列表
    }
    #endregion

}
