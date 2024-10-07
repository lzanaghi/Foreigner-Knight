using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
