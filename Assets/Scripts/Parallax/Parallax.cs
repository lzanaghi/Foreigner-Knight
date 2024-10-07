using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject cameraPlayer;
    private float lengthX, startPosX, startPosY;
    public float parallaxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Parallax for X axis
        float tempX = cameraPlayer.transform.position.x * (1 - parallaxSpeed);
        float distX = cameraPlayer.transform.position.x * parallaxSpeed;

        // Parallax for Y axis
        float distY = cameraPlayer.transform.position.y * parallaxSpeed;

        // Apply the parallax effect to both X and Y axes
        transform.position = new Vector3(startPosX + distX, startPosY + distY, transform.position.z);

        // Reset the background position on the X axis
        if (tempX > startPosX + lengthX)
        {
            startPosX += lengthX;
        }
        else if (tempX < startPosX - lengthX)
        {
            startPosX -= lengthX;
        }

    }
}