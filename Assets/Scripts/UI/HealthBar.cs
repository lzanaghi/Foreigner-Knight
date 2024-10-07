using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float health;
    public PlayerCombat playerCombat;
    public float MAX_DELAY_TIME = 1f;
    public float delayTime = 1f;
    private float lerpSpeed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {   
    }

    // Update is called once per frame
    void Update()
    {
        health = playerCombat.currentHealth;
        if(healthSlider.value != health)
        {
            healthSlider.value = health;
        } 
        delayTime -= Time.deltaTime;
        if(delayTime < 0){
            if(healthSlider.value < easeHealthSlider.value){
                easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed); 
            }
        }
    }

    IEnumerator DecreaseEaseHealthBar(){
        if(healthSlider.value != easeHealthSlider.value){
            yield return new WaitForSeconds(1f);
            while(healthSlider.value != easeHealthSlider.value)
                easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
        }
        yield return null;
    }
}
