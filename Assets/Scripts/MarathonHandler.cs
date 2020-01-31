using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class MarathonHandler : MonoBehaviour
{
    public static bool isMarathon;
    public float levelTimer;
    public float lightTimer;

    public GameObject[] allWindows;
    public GameObject[] allThiefs;
    public GameObject window;

    public static bool hasWon;
    public static int levelDifficulty;
    public static int marathonRound;

    public GameObject windowPrefab;
    public GameObject longPrefab;
    public GameObject timelight;
    public Text levelTimeText;
    public AudioClip timelightSound;

    public float thiefSpeed;
    public int spawnedWindows;

    private bool isReady;
    private bool validPosition;
    private Color redColor;
    private Color greenColor;

    // Start is called before the first frame update
    void Start()
    {
        if (isMarathon == false)
        {
            marathonRound = 0;
            isMarathon = true;
        }
        hasWon = false;
        levelDifficulty = 4;
        SetThisRound();
        SetTimelight();
        allThiefs = GameObject.FindGameObjectsWithTag("Thief");

        foreach (GameObject thief in allThiefs)
        {
            thief.GetComponent<ThiefHandler>().speed = thiefSpeed;
        }

        for (int i = 0; i < 10; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Light").Length < spawnedWindows)
            {
                SpawnWindow("windowPrefab");
                SpawnWindow("longPrefab");
            }
        }
        allWindows = GameObject.FindGameObjectsWithTag("Light");

    }

    public void SetThisRound()
    {
        levelTimer = 10;
        thiefSpeed = 1;
        spawnedWindows = 10;
        if (marathonRound > 0)
        {
            for (int i = 0; i < marathonRound; i++)
            {
                levelTimer += 5;
                thiefSpeed += 0.2f;
                spawnedWindows -= 1;
                if (spawnedWindows == 0)
                {
                    spawnedWindows = 2;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int levelSeconds = (int)Time.timeSinceLevelLoad;
        levelTimeText.text = levelSeconds + " / " + levelTimer.ToString();

        if (Time.timeSinceLevelLoad > lightTimer && isReady == false)
        {
            timelight.GetComponentInChildren<Light2D>().intensity = 0.5f;
            timelight.GetComponent<SpriteRenderer>().color = greenColor;
            isReady = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnTimelightPress();
        }

        if (Time.timeSinceLevelLoad > levelTimer)
        {
            hasWon = true;
            marathonRound++;
            DestroyObjectsOnSceneEnd();
            SceneManager.LoadScene("MarathonGameScene");
        }

        bool allOut = true;
        foreach (GameObject w in allWindows)
        {
            if (w.GetComponentInChildren<Light2D>().intensity != 0)
            {
                allOut = false;
            }
        }

        if (allOut)
        {
            hasWon = false;
            DestroyObjectsOnSceneEnd();
            SceneManager.LoadScene("EndScene");
        }
    }

    /**
     * if game object "timelight" is clicked / pressed and is ready,
     * it slows down all the thieves to half their speed and resets
     */
    public void OnTimelightPress()
    {
        AudioSource.PlayClipAtPoint(timelightSound, transform.position);
        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D touchCollider = Physics2D.OverlapPoint(touchPosition);
        if (touchCollider && touchCollider.gameObject.tag == "Timelight")
        {
            if (isReady)
            {
                foreach (GameObject thief in allThiefs)
                {
                    double newSpeed = thief.GetComponent<ThiefHandler>().speed * 0.5;
                    thief.GetComponent<ThiefHandler>().speed = (float)newSpeed;
                }
                timelight.GetComponentInChildren<Light2D>().intensity = 0.5f;
                timelight.GetComponent<SpriteRenderer>().color = redColor;
                lightTimer = (lightTimer - 1) * 2;
                isReady = false;
            }
        }
    }

    /**
     * set up details of game object "timelight" 
     * set light to red, set timer and make it inactive
     */
    public void SetTimelight()
    {
        isReady = false;
        lightTimer = 8;
        redColor = new Color(222, 0, 10);
        greenColor = new Color(0, 255, 60);
        timelight = GameObject.FindGameObjectWithTag("Timelight");
        timelight.GetComponentInChildren<Light2D>().intensity = 0.5f;
        timelight.GetComponent<SpriteRenderer>().color = redColor;
    }

    /** Destroys all windows, thiefs and timer when player wins or loses*/
    public void DestroyObjectsOnSceneEnd()
    {
        foreach (GameObject thief in allThiefs)
        {
            Destroy(thief);
        }
        foreach (GameObject window in allWindows)
        {
            Destroy(window);
        }
        Destroy(timelight);
    }

    /**
    * set up and scales new window prefab at random position
    * if position is already occuppied by another game object, 
    * anther random position is chosen
    */
    public void SpawnWindow(string _prefabName)
    {
        float scaleFactor = 0.2f;
        validPosition = false;

        Camera cam = Camera.main;
        var height = 2f * cam.orthographicSize;
        var width = height * cam.aspect;

        var windowRadius = windowPrefab.GetComponent<CircleCollider2D>().radius * scaleFactor;
        var windowSize = longPrefab.GetComponent<BoxCollider2D>().size * (scaleFactor * 1.2f);
        var timelightSize = timelight.GetComponent<BoxCollider2D>().size * (scaleFactor * 2);
        var textSize = levelTimeText.GetComponent<BoxCollider2D>().size * (scaleFactor / 80);

        Bounds timelightBounds = new Bounds(timelight.transform.localPosition, timelightSize);
        Bounds textBounds = new Bounds(levelTimeText.transform.localPosition, textSize);

        Vector3 randomWindowPos = new Vector3(0, 0, 0);

        for (int i = 0; i < 10; i++)
        {
            if (validPosition == false)
            {
                validPosition = true;
                // create new random position for window prefab
                randomWindowPos = new Vector3(
                     Random.Range((-width / 2) + (windowRadius * 3), (width / 2) - windowRadius * 3),
                     Random.Range((-height / 2) + (windowRadius), (height / 2) - (windowRadius * 2)),
                     cam.farClipPlane / 2);

                Bounds windowBounds = new Bounds(randomWindowPos, windowSize);

                GameObject[] windowArr = GameObject.FindGameObjectsWithTag("Light");
                if (windowArr.Length > 0)
                {
                    foreach (GameObject w in windowArr)
                    {
                        Vector3 existingPos = w.transform.localPosition;
                        Bounds existingBounds = new Bounds(existingPos, windowSize);
                        if (windowBounds.Intersects(existingBounds)
                            || windowBounds.Intersects(timelightBounds) || windowBounds.Intersects(textBounds))
                        {
                            validPosition = false;
                        }
                    }
                }

                if (validPosition)
                {
                    switch (_prefabName)
                    {
                        case "windowPrefab":
                            GameObject window = Instantiate(windowPrefab, randomWindowPos, Quaternion.identity);
                            window.transform.localScale = new Vector3(
                            transform.localScale.x * scaleFactor,
                            transform.localScale.y * scaleFactor,
                            transform.localScale.z * scaleFactor);
                            window.GetComponentInChildren<Light2D>().pointLightInnerRadius *= scaleFactor;
                            window.GetComponentInChildren<Light2D>().pointLightOuterRadius *= scaleFactor;
                            break;
                        case "longPrefab":
                            GameObject windowLong = Instantiate(longPrefab, randomWindowPos, Quaternion.identity);
                            windowLong.transform.localScale = new Vector3(
                            transform.localScale.x * scaleFactor,
                            transform.localScale.y * scaleFactor,
                            transform.localScale.z * scaleFactor);
                            windowLong.GetComponentInChildren<Light2D>().pointLightInnerRadius *= scaleFactor;
                            windowLong.GetComponentInChildren<Light2D>().pointLightOuterRadius *= scaleFactor;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
