using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TileMapController : MonoBehaviour
{
    [Header("��Ƭ")]
    public Tilemap tilemap; // ��Ҫ�ֶ���Tilemap������ק�����ֶ�
    public Sprite[] TileSprites; // ��������Tile��GameObjectԤ�Ƽ�����
    public GameObject TilePrefab;


    private List<GameObject> generatedObjects = new List<GameObject>(); // �洢���ɵ�GameObject

    public Transform gridParentTransf; //
    [Header("С����")]
    public GameObject[] smallObjectsPrefabs; // ��������С�����Ԥ�Ƽ�����

    public float smallObjectProbability = 0.2f; // С�������ɵĸ���

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
    
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(FilpAllTile());
        }
    }

    #region �����л�

    public float flipTimeSplit = 0.5f;
    //4-- -6
    IEnumerator FilpAllTile()
    {
        Dictionary<int, List<Animator>> animDic = new Dictionary<int, List<Animator>>();
        foreach (Transform _gridTransf in gridParentTransf)
        {
            int _index = int.Parse(_gridTransf.name.Split("_")[1]);
            if (!animDic.ContainsKey(_index))
            {
                animDic[_index] = new List<Animator>();
            }
            animDic[_index].Add(_gridTransf.GetComponent<Animator>());
        }
        
        for (int i = 4;i>=-6;i--)
        {
            List<Animator> _animList = animDic[i];
            foreach (var _anim in _animList)
            {
                _anim.Play("Grid_Filp1");
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

    void ClearGeneratedObjects()
    {
        if (generatedObjects != null)
        {
            foreach (GameObject obj in generatedObjects)
            {
                DestroyImmediate(obj); // �������ɵ�GameObject
            }
            generatedObjects.Clear(); // ����б�
        }
        
    }
    #endregion

}
