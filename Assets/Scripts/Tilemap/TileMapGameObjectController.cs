using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
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
    public GameObject[] smallObjectsPrefabsLevel1;
    public GameObject[] smallObjectLevel2;
    public GameObject[] smallObjectLevel3; // 用于生成小物体的预制件数组

    public float smallObjectProbability = 0.2f; // 小物体生成的概率

    public Vector2Int FilpStartPos;
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


    public static int CurLevel = 1;
    public static bool isLoadLevel = false;
    void Start()
    {
        InstorageTileMapData();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            FilpAllTile(FilpStartPos);
        }
    }

    #region Tile数据
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
    /// <summary>
    /// 根据二维坐标获取对应物体
    /// </summary>
    /// <param name="_pos"></param>
    /// <returns></returns>
    public GameObject GetTileObject(Vector2Int _pos)
    {
        if (TileMapData.ContainsKey(_pos))
        {
            return TileMapData[_pos];
        }

        return null;
    }
    #endregion

    #region 动画切换
    public void FilpAllTile(Vector2Int _startPos)
    {
        StartCoroutine(FilpAllTileIEnum(_startPos));
    }
    public float flipTimeSplit = 0.5f;
    
    // 从上到下反转地图  4-- -6
    //IEnumerator FilpFromUpToBottom()
    //{
    //    Dictionary<int, List<Animator>> animDic = new Dictionary<int, List<Animator>>();
    //    foreach (Transform _gridTransf in gridParentTransf)
    //    {
    //        int _index = int.Parse(_gridTransf.name.Split("_")[1]);
    //        if (!animDic.ContainsKey(_index))
    //        {
    //            animDic[_index] = new List<Animator>();
    //        }
    //        animDic[_index].Add(_gridTransf.GetComponent<Animator>());
    //    }
    //    for (int i = 4; i >= -6; i--)
    //    {
    //        List<Animator> _animList = animDic[i];
    //        foreach (var _anim in _animList)
    //        {
    //            _anim.Play("Grid_Filp1");
    //        }
    //        yield return new WaitForSeconds(flipTimeSplit);
    //    }
    //}


    IEnumerator FilpAllTileIEnum(Vector2Int _startPos)
    {
        isLoadLevel = true;

        List<Vector2Int> flipedSprite = new List<Vector2Int>();
        //一轮
        List<Vector2Int> _stack = new List<Vector2Int> { _startPos };
        while (_stack.Count > 0)
        {
            List<Vector2Int> _tempStack = new List<Vector2Int>();
            foreach (var _vector2 in _stack)
            {
                if (TileMapData.ContainsKey(_vector2))
                {
                    flipedSprite.Add(_vector2);
                    //有该数据并且还未反转过
                    TileMapData[_vector2].GetComponent<Animator>().Play("Grid_Filp1");

                    var _aroundGrids = GridManager.getAroundGrids(_vector2);
                    foreach (var _aroundGrid in _aroundGrids)
                    {
                        if (TileMapData.ContainsKey(_aroundGrid) && !flipedSprite.Contains(_aroundGrid))
                        {
                            _tempStack.Add(_aroundGrid);
                        }
                    }
                }
            }
            _stack = _tempStack;
          
            
            yield return new WaitForSeconds(flipTimeSplit);
        }

        CurLevel++;
        isLoadLevel = false;

        //todo: 关卡加载结束，启用玩家交互
    }
    #endregion

    #region 地块生成
    [Button("初始化Grid")]
    public void InitGenerateGridGO()
    {
        ClearGeneratedObjects();
        GenerateOffsetsAndNoise();
        GenerateGameObjectsFromTiles();
    }

    private string spriteName = "Sprite";
    [Button("读取地图数据")]
    public void LoadGridData()
    {
        Transform level1Transf = GameObject.Find("Level1").transform;
        Transform level2Transf = GameObject.Find("Level2").transform;
        Transform level3Transf = GameObject.Find("Level3").transform;

        foreach (Transform _transf in gridParentTransf)
        {
            string _name = _transf.name;
            _transf.Find("Sprite").GetComponent<SpriteRenderer>().sprite = level1Transf.Find(_name).GetComponent<SpriteRenderer>().sprite;
            _transf.GetComponentInChildren<GridAnimEvent>().level1TargetSprite = level2Transf.Find(_name).GetComponent<SpriteRenderer>().sprite;
            _transf.GetComponentInChildren<GridAnimEvent>().level2TargetSprite = level3Transf.Find(_name).GetComponent<SpriteRenderer>().sprite;

            Transform _transfChild = _transf.Find(spriteName);
            for(int i = _transfChild.childCount-1;i>=0;i--)
            {
                DestroyImmediate(_transfChild.GetChild(i).gameObject);
            }
            // var _smallGo = _transf.Find(smallObjectName);
            // if (_smallGo != null)
            // {
            //     DestroyImmediate(_smallGo.gameObject);
            // }

            GameObject[] _smallGoPrefabs = null;
            if(CurLevel == 1)
            {
                _smallGoPrefabs = smallObjectsPrefabsLevel1;
            }
            else if(CurLevel == 2)
            {
                _smallGoPrefabs = smallObjectLevel2;
            }
            else if(CurLevel == 3)
            {
                _smallGoPrefabs = smallObjectLevel3;
            }
            
            //处理小物体
            if (Random.value < smallObjectProbability)
            {
                int smallObjectIndex = Random.Range(0, _smallGoPrefabs.Length);
                GameObject smallObjectPrefab = _smallGoPrefabs[smallObjectIndex];

                Vector3 ObjectLocalPosition = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
                GameObject smallObject = Instantiate(smallObjectPrefab, Vector3.zero,Quaternion.identity, _transf);
                smallObject.transform.localPosition = ObjectLocalPosition;
                smallObject.transform.SetParent(_transf.Find("Sprite"));
                //smallObject.name = "SmallObject_" + localPlace.x + "_" + localPlace.y;
                smallObject.name = smallObjectName;
            }
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
                }
            }
        }
    }
    
    string smallObjectName = "SmallObject";
    public void ReGenerateSmallGameObject(Transform _transf)
    {
        //清除旧的小物体和铁轨
        Transform _transfChild = _transf.Find(spriteName);
        for(int i = _transfChild.childCount-1;i>=0;i--)
        {
            Destroy(_transfChild.GetChild(i).gameObject);
        }
        
        
        //生成新的小物体
        if (Random.value < smallObjectProbability)
        {
            int smallObjectIndex = Random.Range(0, smallObjectsPrefabsLevel1.Length);
            GameObject smallObjectPrefab = smallObjectsPrefabsLevel1[smallObjectIndex];
            Vector3 smallObjectLocalPosition = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
            GameObject smallObject = Instantiate(smallObjectPrefab,Vector3.zero , Quaternion.identity, _transf);
            smallObject.transform.localPosition = smallObjectLocalPosition;
            smallObject.transform.SetParent(_transf.Find("Sprite"));
            smallObject.transform.localRotation = Quaternion.identity;
            //smallObject.name = "SmallObject_" + localPlace.x + "_" + localPlace.y;
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
