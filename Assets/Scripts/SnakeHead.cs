using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    public static int timer, period;
    public static double getTimeRatio(){
        return ((double)timer)/period;
    }
    public static void goTime(){
        timer=(timer+1)%period;
    }
    public static void initTime(int period){
        SnakeHead.period=period;
        timer=0;
    }
    private GridManager manager;
    private Directions direction;
    private Vector3Int currentGrid, targetGrid;
    // Start is called before the first frame update
    void Start()
    {
        manager=GameObject.Find("Grid").GetComponent<GameController>().manager;
        direction=Directions.Up;
    }

    // Update is called once per frame
    void FixedUpdate(){
        //transform.position=
    }
}
