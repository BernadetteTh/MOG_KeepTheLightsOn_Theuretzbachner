using UnityEngine;
using UnityEngine.SceneManagement;

public class StartHandler : MonoBehaviour
{

    public static int levelDifficulty;

    /** set level difficulty */
    private void SetLevelDifficulty(int _level)
    {
        levelDifficulty = _level;
    }

    /** load the right scene*/
    public void StartGameScene()
    {
        switch (levelDifficulty)
        {
            case 1:
                SceneManager.LoadScene("EasyGameScene", LoadSceneMode.Single);
                break;
            case 2:
                SceneManager.LoadScene("RegularGameScene", LoadSceneMode.Single);
                break;
            case 3:
                SceneManager.LoadScene("HardGameScene", LoadSceneMode.Single);
                break;
            case 4:
                SceneManager.LoadScene("MarathonGameScene", LoadSceneMode.Single);
                break;
            default:
                SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
                break;
        }
    }

    /** quit the game */
    public void QuitGame()
    {
        Application.Quit();
    }
}
