using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Fade))]
public class SampleSceneManager : MonoBehaviour
{
    private Fade fadeIn;

    // Start is called before the first frame update
    void Start()
    {
        fadeIn = GetComponent<Fade>();
        fadeIn.fadeImage.color = new Color(0f,0f,0f,1f);
        StartCoroutine(fadeIn.FadeInFromBlack());   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
