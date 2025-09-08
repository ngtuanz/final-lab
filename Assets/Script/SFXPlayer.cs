using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource shootSound;   // âm thanh bắn đạn
    public AudioSource explosionSound; // âm thanh va chạm gà nổ

    // Gọi hàm này khi player bắn đạn
    public void PlayShoot()
    {
        if (shootSound != null)
            shootSound.Play();
    }

    // Gọi hàm này khi đạn trúng gà
    public void PlayExplosion()
    {
        if (explosionSound != null)
            explosionSound.Play();
    }
}
