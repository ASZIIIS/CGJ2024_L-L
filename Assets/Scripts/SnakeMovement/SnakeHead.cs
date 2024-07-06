using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SnakeHead : MonoBehaviour
{
    
    private GridManager manager;
    private Grid grid;
    private int direction;
    private Vector3Int currentGrid, targetGrid;
    private bool moveStage=false;
    private SnakeBody nextBody, tail;
    private bool systemPause=false, pause=true;
    public int forwardTimer, forwardPeriod, waveTimer, wavePeriod;
    // Start is called before the first frame update
    void Start()
    {
        manager=GameObject.Find("Grid").GetComponent<GameController>().manager;
        grid=GameObject.Find("Grid").GetComponent<Grid>();
        direction=(int)Directions.Up;
        targetGrid=new Vector3Int(0,0,-10);
    }
    // Update is called once per frame
    void FixedUpdate(){
        if(systemPause){
            return;
        }
        Vector3 currentPosition;
        if(forwardTimer==0){
            if(moveStage){
                //reach the center of a grid
                //TODO: check grow
                if(nextBody is not null){
                    nextBody.changeTarget(currentGrid, direction);
                }
                currentGrid=targetGrid;
                //TODO: change direction
                targetGrid=manager.move(currentGrid, direction);
            }
        }
        if(!pause){
            if(!moveStage){
                //first half (center to edge)
                currentPosition=grid.CellToWorld(currentGrid)+GridManager.halfUnitVector[direction]*forwardTimer/forwardPeriod;
                transform.position=currentPosition;
            }else{
                //second half (edge to center)
                currentPosition=grid.CellToWorld(targetGrid)-GridManager.halfUnitVector[direction]*(forwardPeriod-forwardTimer)/forwardPeriod;
                transform.position=currentPosition;
            }
        }
        if(nextBody is not null){
            nextBody.move(moveStage, pause);
        }
    }
    public void forwardGoTime(){
        forwardTimer=(forwardTimer+1)%forwardPeriod;
    }
    public void waveGoTime(){
        waveTimer=(waveTimer+1)%wavePeriod;
    }
    public void initTime(int forwardPeriod, int wavePeriod){
        forwardPeriod=forwardPeriod;
        wavePeriod=wavePeriod;
        forwardTimer=0;
        waveTimer=0;
    }
}
