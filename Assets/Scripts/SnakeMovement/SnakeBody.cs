using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    public SnakeBody nextBody;
    private SnakeHead head;
    private Grid grid;
    private Vector3Int currentGrid, targetGrid; 
    private int direction;
    private int depth;
    private float waveAmplitude, wavePhase;
    public SnakeBody(
            SnakeHead head, 
            Grid grid, 
            Vector3Int birthGrid, 
            int depth, 
            float waveAmplitude, 
            float wavePhase
        ){
        this.head=head;
        this.grid=grid;
        this.depth=depth;
        this.targetGrid=birthGrid;
        this.waveAmplitude=waveAmplitude;
        this.wavePhase=wavePhase;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void changeTarget(Vector3Int newTarget, int newDirection){
        if(nextBody is not null){
            nextBody.changeTarget(currentGrid, direction);
        }
        direction=newDirection;
        currentGrid=targetGrid;
        targetGrid=newTarget;
        return;
    }
    public void move(bool moveStage, bool pause){
        Vector3 currentPosition;
        if(!moveStage){
            //first half (center to edge)
            currentPosition=grid.CellToWorld(currentGrid)
                +GridManager.halfUnitVector[direction]
                *head.forwardTimer/head.forwardPeriod;
        }else{
            //second half (edge to center)
            currentPosition=grid.CellToWorld(targetGrid)
                -GridManager.halfUnitVector[direction]
                *(head.forwardPeriod-head.forwardTimer)/head.forwardPeriod;
        }
        currentPosition+=GridManager.waveUnitVector[direction]
            *Mathf.Sin((float)(wavePhase*depth+head.waveTimer)*Mathf.PI)*waveAmplitude;
        if(nextBody is null){
            if(!pause){
                head.forwardGoTime();
            }
            head.waveGoTime();
        }else{
            nextBody.move(moveStage, pause);
        }
    }
    public void birth(){
        
    }
    public void digest(){

    }
}
