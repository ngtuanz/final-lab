using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PGame : MonoBehaviour
{
    public GameObject pausegame;
    public GameObject PlayGame;
    private bool isPaused = false;
    void Start()
    {
        pausegame.SetActive(false);
        PlayGame.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        pausegame.SetActive(true);
        PlayGame.SetActive(false);
        Time.timeScale = 0;
        isPaused = true;
    }
    public void ContinueGame()
    {
        pausegame.SetActive(false);
        PlayGame.SetActive(true);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void remainsecert()
    {
        SceneManager.LoadScene("MENU");
    }
    public void relast()
    {
        SceneManager.LoadScene("MAIN");
        Time.timeScale = 1.0f;
    }
}
