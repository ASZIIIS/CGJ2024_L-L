using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour
{
    public Tilemap tilemap; // ��Ҫ�ֶ���Tilemap������ק�����ֶ�
    public GameObject[] tilePrefabs; // ��������Tile��GameObjectԤ�Ƽ�����
    public float noiseScale1 = 0.1f; // ����1������
    public float noiseScale2 = 0.05f; // ����2������
    public float noiseWeight1 = 0.5f; // ����1��Ȩ��
    public float noiseWeight2 = 0.5f; // ����2��Ȩ��

    private Vector2 randomOffset1;
    private Vector2 randomOffset2;

    private List<GameObject> generatedObjects = new List<GameObject>(); // �洢���ɵ�GameObject
    private GameObject currentParentObject; //
    public GameObject[] smallObjectsPrefabs; // ��������С�����Ԥ�Ƽ�����
    public float smallObjectProbability = 0.2f; // С�������ɵĸ���

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
        // ����һ���µĸ���Object
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

                    // ʹ������Perlin�������ɴ��и���仯�ĵ��Σ����������ƫ��
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
        foreach (GameObject obj in generatedObjects)
        {
            Destroy(obj); // �������ɵ�GameObject
        }
        generatedObjects.Clear(); // ����б�

        if (currentParentObject != null)
        {
            Destroy(currentParentObject); // ���ٸ���Object
        }
    }
}
