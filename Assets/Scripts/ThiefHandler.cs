using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ThiefHandler : MonoBehaviour
{
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float speed;
    public int levelDifficulty;

    Vector2 targetPosition;
    Vector3 touchPosition;
    Collider2D col;
    private bool thiefClicked;


    /**
     * set up target position and collider at the start
     */
    void Start()
    {
        SetRandomTargetPosition();
        col = GetComponent<Collider2D>();
    }

    /**
     * checks if thief has been clicked / pressed
     * if yes, change target position by choosing new one 
     * if thief hasn't reached target position yet, keep moving until he has
     * if he reaches target, choose random new target position
     */
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                Collider2D touchCollider = Physics2D.OverlapPoint(touchPosition);
                if (touchCollider && touchCollider.gameObject == this.gameObject)
                {
                    thiefClicked = true;
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if(thiefClicked)
                {
                    transform.position = new Vector2(touchPosition.x, touchPosition.y);
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                thiefClicked = false;
            }
        }

        // thief moves as long as target position isn't reached
        if ((Vector2)transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else
        {
            // if target is reached, set new target
            SetRandomTargetPosition();
        }
    }

    /**
     * set target position to random vector still within the screen
     */
    public void SetRandomTargetPosition()
    {
        Camera cam = Camera.main;
        var height = 2f * cam.orthographicSize;
        var width = height * cam.aspect;

        targetPosition = new Vector2(
                     Random.Range((-width / 2), (width / 2)),
                     Random.Range((-height / 2), (height / 2)));
    }

    /**
     * Thief collider
     * if collision with a window happens, thief speeds up by 10 percent
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Light")
        {
            double newSpeed = (double)speed * 1.05;
            speed = (float)newSpeed;
        }
    }
}
