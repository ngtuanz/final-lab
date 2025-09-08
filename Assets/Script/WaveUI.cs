using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    public TextMeshProUGUI waveText;
    private int waveNumber = 0;

    // Gọi khi bắt đầu wave mới
    public void UpdateWave(int wave)
    {
        waveNumber = wave;
        waveText.text = "Wave " + waveNumber;
    }

    public int GetCurrentWave()
    {
        return waveNumber;
    }
}
