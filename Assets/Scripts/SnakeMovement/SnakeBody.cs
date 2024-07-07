using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    public SnakeBody nextBody=null;
    private SnakeHead head;
    private Grid grid;
    private Vector3Int currentGrid, targetGrid; 
    private int direction;
    private int depth;
    private float waveAmplitude, wavePhase, normalScale;
    private int growState=0;
    private bool growUp=false;
    public void Init(
            SnakeHead head, 
            Grid grid, 
            Vector3Int birthGrid, 
            int depth, 
            float waveAmplitude, 
            float wavePhase,
            float normalScale
        ){
        this.head=head;
        this.grid=grid;
        this.depth=depth;
        this.targetGrid=birthGrid;
        this.waveAmplitude=waveAmplitude;
        this.wavePhase=wavePhase;
        this.normalScale=normalScale;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void changeTarget(Vector3Int newTarget, int newDirection, bool isGrowing){
        if(nextBody is not null){
            nextBody.changeTarget(currentGrid, direction, (growState==2));
        }else{
            if(growState==2){
                nextBody=Instantiate(head.bodyPrefeb, grid.CellToWorld(currentGrid), Quaternion.identity).GetComponent<SnakeBody>();
                nextBody.Init(head, grid, currentGrid, depth+1, waveAmplitude, wavePhase, normalScale);
            }
        }
        if(isGrowing){
            growState=2;
        }else{
            if(growState>0){
                growState-=1;
            }
        }
        growUp=true;
        direction=newDirection;
        transform.rotation=Quaternion.Euler(0f,0f,60f*(4-this.direction));
        currentGrid=targetGrid;
        targetGrid=newTarget;
        return;
    }
    public void move(bool moveStage, bool pause){
        Vector3 currentPosition;
        if(!growUp){
            if(!pause){
                if(!moveStage){
                    transform.localScale=Vector3.one*normalScale
                        *(head.forwardTimer/head.forwardPeriod)/2;
                }else{
                    transform.localScale=Vector3.one*normalScale
                        *(1f+head.forwardTimer/head.forwardPeriod)/2;
                }
                
            }
        }
        if(!moveStage){
            //first half (center to edge)
            currentPosition=grid.CellToWorld(currentGrid)
                +GridManager.halfUnitVector[direction]
                *head.forwardTimer/head.forwardPeriod;
            if(!pause&&growState==1){
                transform.localScale=Vector3.one*normalScale
                    *(1f+Mathf.Sin(Mathf.PI*0.5f*(1+head.forwardTimer/head.forwardPeriod)));
            }
        }else{
            //second half (edge to center)
            currentPosition=grid.CellToWorld(targetGrid)
                -GridManager.halfUnitVector[direction]
                *(head.forwardPeriod-head.forwardTimer)/head.forwardPeriod;
            if(!pause&&growState==2){
                transform.localScale=Vector3.one*normalScale
                    *(1f+Mathf.Sin(Mathf.PI*0.5f*(head.forwardTimer/head.forwardPeriod)));
            }
        }
        currentPosition+=GridManager.waveUnitVector[direction]
            *Mathf.Sin((float)(wavePhase*depth+head.waveTimer)*Mathf.PI/head.wavePeriod)*waveAmplitude;
        transform.position=currentPosition;
        if(nextBody is not null){
            nextBody.move(moveStage, pause);
        }
    }
}
