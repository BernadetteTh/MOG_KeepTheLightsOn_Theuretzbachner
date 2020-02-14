using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class SceneHandler : MonoBehaviour
{
    public float levelTimer;
    public float lightTimer;

    public GameObject[] allWindows;
    public GameObject[] allThiefs;
    public GameObject window;

    public static bool hasWon;
    public static int levelDifficulty;

    public GameObject windowPrefab;
    public GameObject longPrefab;
    public GameObject timelight;
    public Text levelTimeText;
    public AudioClip timelightSound;

    private bool isReady;
    private bool validPosition;
    private bool withTimelight;
    private Color redColor;
    private Color greenColor;

    /**
     * sets up level time, checks if game leve easy or regular
     * if regular level, windows are randomly generated
     * collects all thief and window objects in an array
     */
    void Start()
    {
        SetLevelTime();
        hasWon = false;
        allThiefs = GameObject.FindGameObjectsWithTag("Thief");

        if (SceneManager.GetActiveScene().name == "RegularGameScene")
        {
            SetRegularGameScene();
        }
        if (SceneManager.GetActiveScene().name == "HardGameScene")
        {
            SetHardGameScene();
        }
        allWindows = GameObject.FindGameObjectsWithTag("Light");
    }

    public void SetRegularGameScene()
    {
        SetTimelight();
        foreach (GameObject thief in allThiefs)
        {
            thief.GetComponent<ThiefHandler>().speed = 1.2f;
        }

        for (int i = 0; i < 10; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Light").Length < 5)
            {
                SpawnWindow("windowPrefab");
                SpawnWindow("longPrefab");
            }
        }
    }

    public void SetHardGameScene()
    {
        SetTimelight();
        foreach (GameObject thief in allThiefs)
        {
            thief.GetComponent<ThiefHandler>().speed = 1.3f;
        }

        for (int i = 0; i < 10; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Light").Length < 3)
            {
                SpawnWindow("windowPrefab");
                SpawnWindow("longPrefab");
            }
        }
    }

    /**
     * counts down and displays level time, as well as light timer if it's a regular game level
     * after light timer is ready, it activates game object "timelight" and registers if clicked / pressed
     * loads end scene once level time is over, while checking if game has been won or lost
     * loads end scene once all window lights have gone out
     */
    void Update()
    {
        int levelSeconds = (int)Time.timeSinceLevelLoad;
        levelTimeText.text = levelSeconds + " / " + levelTimer.ToString();

        // if timelight exists
        if (withTimelight)
        {
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
        }

        // check if leve time has run out, game ends
        if (Time.timeSinceLevelLoad > levelTimer)
        {
            hasWon = true;
            DestroyObjectsOnSceneEnd();
            SceneManager.LoadScene("EndScene");
        }

        // check if all lights are out or not
        bool allOut = true;
        foreach (GameObject w in allWindows)
        {
            if (w.GetComponentInChildren<Light2D>().intensity != 0)
            {
                allOut = false;
            }
        }

        // all the ligths have gone out, game lost
        if (allOut)
        {
            hasWon = false;
            DestroyObjectsOnSceneEnd();
            SceneManager.LoadScene("EndScene");
        }
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
     * if game object "timelight" is clicked and is ready,
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
                    double newSpeed = thief.GetComponent<ThiefHandler>().speed * 0.3;
                    thief.GetComponent<ThiefHandler>().speed = (float)newSpeed;
                }
                timelight.GetComponentInChildren<Light2D>().intensity = 0.5f;
                timelight.GetComponent<SpriteRenderer>().color = redColor;
                lightTimer = (lightTimer - 1) * 3;
                isReady = false;
            }
        }
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

        var timelightCol = timelight.GetComponent<BoxCollider2D>();
        var windowCol = windowPrefab.GetComponent<CircleCollider2D>();
        var longCol = longPrefab.GetComponent<BoxCollider2D>();
        var textCol = levelTimeText.GetComponent<BoxCollider2D>();

        var windowRadius = windowPrefab.GetComponent<CircleCollider2D>().radius * scaleFactor;
        var windowSize = longPrefab.GetComponent<BoxCollider2D>().size * scaleFactor;
        var windowbounds = longPrefab.GetComponent<BoxCollider2D>().bounds; 

        var timelightSize = timelight.GetComponent<BoxCollider2D>().size * scaleFactor;
        Bounds timelightBounds = new Bounds(timelight.transform.localPosition, timelightSize);

        var textSize = levelTimeText.GetComponent<BoxCollider2D>().size * scaleFactor;
        Bounds textBounds = new Bounds(levelTimeText.transform.localPosition, textSize);

        Vector3 randomWindowPos = new Vector3(0, 0, 0);

        // cechk if new random window position is valid or something else is already there
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

                // create bounds for window at new position
                Bounds windowBounds = new Bounds(randomWindowPos, windowSize / 1.5f);
                if (Physics.CheckBox(randomWindowPos, windowSize * 2f))
                {
                    validPosition = false;
                }

                // check if new position interfers with any other existing windows
                GameObject[] windowArr = GameObject.FindGameObjectsWithTag("Light");
                if (windowArr.Length > 0)
                {
                    foreach (GameObject w in windowArr)
                    {
                        Vector3 existingPos = w.transform.localPosition;
                        Bounds existingBounds = new Bounds(existingPos, windowSize * 2f);
                        if (windowBounds.Intersects(existingBounds))
                        {
                            validPosition = false;
                        }
                    }
                }
               
                // if position is valid, create one of two possible windows
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

    /**
     * set up details of game object "timelight" 
     * set light to red, set timer and make it inactive
     */
    public void SetTimelight()
    {
        withTimelight = true;
        isReady = false;
        lightTimer = 8;

        redColor = new Color(222, 0, 10);
        greenColor = new Color(0, 255, 60);
        timelight = GameObject.FindGameObjectWithTag("Timelight");
        timelight.GetComponentInChildren<Light2D>().intensity = 0.5f;
        timelight.GetComponent<SpriteRenderer>().color = redColor;
    }

    /**
     * set amount of seconds of game time level gets, depending on chosen level difficulty 
     */
    private void SetLevelTime()
    {
        levelTimer = 10;
        levelDifficulty = StartHandler.levelDifficulty;

        if (levelDifficulty == 0)
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "EasyGameScene":
                    levelDifficulty = 1;
                    break;
                case "RegularGameScene":
                    levelDifficulty = 2;
                    break;
                case "HardGameScene":
                    levelDifficulty = 3;
                    break;
                default:
                    break;
            }
        }

        switch (levelDifficulty)
        {
            case 1:
                levelTimer = 15;
                break;
            case 2:
                levelTimer = 20;
                break;
            case 3:
                levelTimer = 30;
                break;
            default:
                break;
        }
    }
}
