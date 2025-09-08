using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isPlayerAlive = true;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverWaveText;   // text hiển thị wave khi game over

    private void Awake()
    {
        // giữ instance singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayerDied()
    {
        isPlayerAlive = false;
        Debug.Log("Player đã chết!");

#if UNITY_2023_1_OR_NEWER
        WaveUI waveUI = Object.FindFirstObjectByType<WaveUI>();
#else
        WaveUI waveUI = Object.FindObjectOfType<WaveUI>();
#endif

        if (waveUI != null && gameOverWaveText != null)
        {
            gameOverWaveText.text = "WARE " + waveUI.GetCurrentWave();
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void PlayerRespawn()
    {
        isPlayerAlive = true;
        Debug.Log("Player hồi sinh!");
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        isPlayerAlive = true;
        SceneManager.LoadScene("MAIN");  // load lại scene
    }

    public void Quit()
    {
        SceneManager.LoadScene("MENU");
    }
}
