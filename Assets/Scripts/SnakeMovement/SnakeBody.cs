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
    public bool awake=false;
    public void Init(
            SnakeHead head, 
            Vector3Int birthGrid,
            int birthDirection, 
            int depth, 
            float waveAmplitude, 
            float wavePhase,
            float normalScale
        ){
        this.head=head;
        this.depth=depth;
        this.targetGrid=birthGrid;
        this.direction=birthDirection;
        transform.rotation=Quaternion.Euler(0f,0f,60f*(4-this.direction));
        this.waveAmplitude=waveAmplitude;
        this.wavePhase=wavePhase;
        this.normalScale=normalScale;
        this.transform.SetParent(head.parent);
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
        if(!awake){
            return;
        }
        if(nextBody != null){
            nextBody.changeTarget(targetGrid, direction);
            if(growState>0){
            if(growState==2){
                nextBody.activeGrow();
            }
            growState--;
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
        if(!awake){
            return;
        }
        Vector3 currentPosition;
        if(!growUp){
            if(!pause){
                if(moveStage){
                    transform.localScale=Vector3.one*normalScale
                        *((float)head.forwardTimer/head.forwardPeriod)/2;
                }else{
                    transform.localScale=Vector3.one*normalScale
                        *((float)head.forwardTimer/head.forwardPeriod+1f)/2;
                }
            }
        }else{
            if(moveStage){
                //first half (center to edge)
                currentPosition=head.CellToWorld(currentGrid)
                    +GridManager.halfUnitVector[direction]
                    *head.forwardTimer/head.forwardPeriod;
                if(!pause&&growState==1){
                    transform.localScale=Vector3.one*normalScale
                        *(1f+0.3f*Mathf.Sin(Mathf.PI*(1+(float)head.forwardTimer/head.forwardPeriod)));
                }
            }else{
                //second half (edge to center)
                currentPosition=head.CellToWorld(targetGrid)
                    -GridManager.halfUnitVector[direction]
                    *(head.forwardPeriod-head.forwardTimer)/head.forwardPeriod;
                if(!pause&&growState==2){
                    transform.localScale=Vector3.one*normalScale
                        *(1f+0.3f*Mathf.Sin(Mathf.PI*((float)head.forwardTimer/head.forwardPeriod)));
                }
            }
            currentPosition+=GridManager.waveUnitVector[direction]
                *Mathf.Sin((float)((wavePhase*depth+head.waveTimer)%head.wavePeriod)/head.wavePeriod*Mathf.PI*2)*waveAmplitude;
            transform.position=currentPosition;
            if(nextBody != null){
                nextBody.move(moveStage, pause);
            }
        }
    }
    public void addTail(int type, Vector3Int birthGrid, int headDirection){
        if(nextBody != null){
            nextBody.addTail(type, birthGrid, headDirection);
        }else{
            nextBody=Instantiate(head.bodyPrefeb, head.CellToWorld(birthGrid), Quaternion.identity).GetComponent<SnakeBody>();
            nextBody.Init(head, birthGrid, headDirection, depth+1, waveAmplitude, wavePhase, normalScale);
            nextBody.GetComponent<BodyAnimation>().ChangeSprite(type);
        }
    }
    public void activeGrow(){
        if(!awake){
            awake=true;
            head.soundEffect("4");
        }else{
            growState=2;
        }
    }
    public void updateCatOn(){
        if(nextBody!=null){
            if(nextBody.awake&&growUp){
                nextBody.updateCatOn();
                return;
            }
        }
        head.setGridCatOn(currentGrid, false);
    }
}
