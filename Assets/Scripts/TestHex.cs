using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestHex : MonoBehaviour
{
    private Grid grid;
    private GridManager manager;
    GameObject head, body;
    int direction;
    // Start is called before the first frame update
    void Start()
    {
        grid=GetComponent<Grid>();
        // manager=new GridManager(6);
        // head=GameObject.Find("SnakeHead");
        // body=GameObject.Find("SnakeBody");
        // direction=0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition=grid.WorldToCell(clickPosition);
            // Vector3Int targetPosition=manager.move(gridPosition, direction);
            // head.transform.position=grid.CellToWorld(targetPosition);
            // body.transform.position=grid.CellToWorld(gridPosition+new Vector3Int(0,0,1));
            Debug.Log(gridPosition.ToString());
        }
        if(Input.GetKeyDown(KeyCode.Alpha0)){
            direction=0;
            Debug.Log("Direction: "+direction);
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            direction=1;
            Debug.Log("Direction: "+direction);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            direction=2;
            Debug.Log("Direction: "+direction);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            direction=3;
            Debug.Log("Direction: "+direction);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            direction=4;
            Debug.Log("Direction: "+direction);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5)){
            direction=5;
            Debug.Log("Direction: "+direction);
        }
    }
}
