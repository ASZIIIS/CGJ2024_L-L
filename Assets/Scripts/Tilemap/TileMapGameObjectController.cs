using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TileMapGameObjectController : MonoBehaviour
{
    [Header("��Ƭ")]
    public Tilemap tilemap; // ��Ҫ�ֶ���Tilemap������ק�����ֶ�
    public Sprite[] TileSprites; // ��������Tile��GameObjectԤ�Ƽ�����
    public GameObject TilePrefab;
    public Dictionary<Vector2, GameObject> TileMapData = new Dictionary<Vector2, GameObject>();

    private List<GameObject> generatedObjects = new List<GameObject>(); // �洢���ɵ�GameObject

    public Transform gridParentTransf; //
    [Header("С����")]
    public GameObject[] smallObjectsPrefabs; // ��������С�����Ԥ�Ƽ�����

    public float smallObjectProbability = 0.2f; // С�������ɵĸ���


    public Vector2 FilpStartPos;
    [FoldoutGroup("�������")]
    public float noiseScale1 = 0.1f; // ����1������
    [FoldoutGroup("�������")]
    public float noiseScale2 = 0.05f; // ����2������
    [FoldoutGroup("�������")]
    public float noiseWeight1 = 0.5f; // ����1��Ȩ��
    [FoldoutGroup("�������")]
    public float noiseWeight2 = 0.5f; // ����2��Ȩ��

    private Vector2 randomOffset1;
    private Vector2 randomOffset2;

    public GameObject directionMarkerPrefab; // ·��Ԥ�Ƽ�

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

    #region �����л�
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

    #region �ؿ�����
    [Button("�������ɵ�������")]
    public void ReGenerateGridGO()
    {
        ClearGeneratedObjects();
        GenerateOffsetsAndNoise();
        GenerateGameObjectsFromTiles();
    }

    void GenerateOffsetsAndNoise()
    {
        // ��ʼ�����ƫ��
        randomOffset1 = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));
        randomOffset2 = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

        // ��̬�����������������ӱ仯
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

                    // ʹ������Perlin�������ɴ��и���仯�ĵ��Σ����������ƫ��
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

                    generatedObjects.Add(tileGO); // �����ɵ�GameObject��ӵ��б���

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
        // ��ȡ�������λ��
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        // ��ȡTile���ڵ�GameObject
        foreach (GameObject tileGO in generatedObjects)
        {
            if (tilemap.WorldToCell(tileGO.transform.position) == cellPos)
            {
                // ��Tileλ�÷��÷����
                GameObject marker = Instantiate(directionMarkerPrefab, tileGO.transform.position, Quaternion.identity, tileGO.transform);

                // ���÷����������ͨ��ĳ�ַ�ʽ���÷�������ͨ��UI��Ԥ�����߼���
                GridSingle gridSingle = tileGO.GetComponent<GridSingle>();
                if (gridSingle != null)
                {
                    gridSingle.SetDirection(Directions.RightDown); // ʾ������
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

        generatedObjects.Clear(); // ����б�
    }
    #endregion

}
