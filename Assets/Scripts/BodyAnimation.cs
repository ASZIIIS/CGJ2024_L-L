using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimation : MonoBehaviour
{
    public List<Sprite> sprites;
    private SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeSprite(int index){
        renderer=GetComponent<SpriteRenderer>();
        renderer.sprite=sprites[index];
    }
}
