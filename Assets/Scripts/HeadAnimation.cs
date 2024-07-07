using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAnimation : MonoBehaviour
{
    public List<RuntimeAnimatorController> runtimeAnimatorControllers;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeAnimation(int index){
        animator=GetComponent<Animator>();
        animator.runtimeAnimatorController = runtimeAnimatorControllers[index];
    }
    public void setAnimation(bool state){
        animator.SetBool("Leader", state);
    }
}
