using UnityEngine;


public class WindowHandler : MonoBehaviour
{
    public static float posX;
    public static float posY;

    bool isLit;
    Collider2D col;
    Color windowColor;

    /**
     * set up window at the start
     */
    void Start()
    {
        SetWindow();
    }

    /**
     * checks if playerd clicked / pressed on window
     * if window is not lit anymore, it lights up again
     */
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D touchCollider = Physics2D.OverlapPoint(touchPosition);
            if (touchCollider && touchCollider.gameObject == this.gameObject)
            {
                if (isLit == false)
                {
                    // light turns back on
                    touchCollider.GetComponentInChildren<UnityEngine.Experimental.Rendering.LWRP.Light2D>().intensity = 0.5f;
                    isLit = true;
                    touchCollider.GetComponent<SpriteRenderer>().color = windowColor;
                }
            }
        }
    }

    /**
     * Sets up window details like color, bool isLit, collider and position
     */
    public void SetWindow()
    {
        windowColor = new Color(255, 255, 255);
        isLit = true;
        col = GetComponent<Collider2D>();
        posX = transform.position.x;
        posY = transform.position.y;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    /**
     * Window collider
     * if collision with a thief happens, window is not lit anymore 
     * and changes light, bool isLit and color
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Thief")
        {
            isLit = false;
            GetComponentInChildren<UnityEngine.Experimental.Rendering.LWRP.Light2D>().intensity = 0;
            GetComponent<SpriteRenderer>().color = new Color(182, 182, 182);
        }
    }
}

