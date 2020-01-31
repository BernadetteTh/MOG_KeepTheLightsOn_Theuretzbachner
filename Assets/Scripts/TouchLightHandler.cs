using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchLightHandler : MonoBehaviour
{
    //  public static float posX;
    // public static float posY;

    // Collider2D col;
    private bool isReady;
    private Color redColor;
    private Color greenColor;
    float lightTimer;


    // Start is called before the first frame update
    void Start()
    {
        redColor = new Color(222, 0, 10);
        greenColor = new Color(42, 240, 110);
        isReady = false;
        lightTimer = 8;
        GetComponent<SpriteRenderer>().color = redColor;
       
    }

    // Update is called once per frame
    void Update()
    {
        lightTimer--;
        if(lightTimer == 0)
        {
            GetComponent<SpriteRenderer>().color = greenColor;
            isReady = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D touchCollider = Physics2D.OverlapPoint(touchPosition);
            if (touchCollider && touchCollider.tag == "Timelight")
            {
                if(isReady)
                {
                    GetComponent<SpriteRenderer>().color = redColor;
                    lightTimer = 8;
                    isReady = false;
                }
            }
        }
    }
}
