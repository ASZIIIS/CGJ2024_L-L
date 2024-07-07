using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    Grid grid;
    TileMapGameObjectController tileMapController;

    GameObject MouseGridGo;

    public Directions CurDirect = Directions.None;
    public GridSingle MouseGrid=> MouseGridGo?.GetComponent<GridSingle>();
    public GameObject mouseGo, TrailPrefab;

    public List<Sprite> trailSprites;


    private void Awake()
    {
        grid = GetComponent<Grid>();
        tileMapController = GetComponent<TileMapGameObjectController>();
    }

    private void Update()
    {
        Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = grid.WorldToCell(clickPosition);

        MouseGridGo = tileMapController.GetTileObject(new Vector2Int(gridPosition.x, gridPosition.y));
        mouseGo.SetActive(MouseGridGo!=null);

        //�����Ϊ�գ����Է���
        if(MouseGridGo !=null)
        {
            mouseGo.transform.position = MouseGridGo.transform.position;
            if(Input.GetMouseButtonDown(0) && MouseGrid.direction == Directions.None)
            {
                SetTrail(MouseGrid, CurDirect);
            }
        }

    }
    public void SetCurTrailDirect(Directions _direct)
    {
        CurDirect = _direct;
        mouseGo.GetComponentInChildren<SpriteRenderer>().sprite = trailSprites[(int)_direct -1];
    }

    public void SetTrail(Vector2Int _pos, Directions _direct)
    {
        var targetTrail = tileMapController.GetTileObject(new Vector2Int(_pos.x, _pos.y)).GetComponent<GridSingle>();
        SetTrail(targetTrail,_direct);
    }
    void SetTrail(GridSingle _gridSingle,Directions _direct)
    {
        if (_gridSingle.catOn)
        {
            return;
        }
        
        Transform _spriteParent = _gridSingle.transform.Find("Sprite");
        GameObject _trailGo = GameObject.Instantiate(TrailPrefab, _gridSingle.transform.position,Quaternion.identity, _spriteParent);
        _trailGo.GetComponent<SpriteRenderer>().sprite = DirectToSprite(_direct);

        _gridSingle.SetTrail(_direct);
    }

    public Sprite DirectToSprite(Directions _direct)
    {
        return trailSprites[(int)_direct - 1];
    }
}
