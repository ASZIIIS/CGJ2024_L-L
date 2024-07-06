using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestHex : MonoBehaviour
{
    private Grid grid;
    // Start is called before the first frame update
    void Start()
    {
        grid=GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition=grid.WorldToCell(clickPosition);
            Vector3 centerPosition=grid.CellToWorld(gridPosition);
            Debug.Log(clickPosition.ToString()+"->"+gridPosition.ToString()+"->"+centerPosition.ToString());
        }
    }
}
