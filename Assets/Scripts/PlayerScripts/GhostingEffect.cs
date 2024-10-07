using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GhostingEffect : MonoBehaviour
{

    public float timeBtwSpawns;
    private float timeBtwSpawnsSeconds;
    public GameObject ghost;
    public bool makeGhosting = false; 

    // Start is called before the first frame update
    void Start()
    {
        timeBtwSpawnsSeconds = timeBtwSpawns;   
    }

    // Update is called once per frame
    void Update()
    {
        if (makeGhosting){
            if(timeBtwSpawnsSeconds>0){
                timeBtwSpawnsSeconds -= Time.deltaTime;

            }else{
                //generate a spawn
                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.localScale = this.transform.localScale;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                timeBtwSpawnsSeconds = timeBtwSpawns;
                Destroy(currentGhost, 1f);
            }  
        }
    }
}
