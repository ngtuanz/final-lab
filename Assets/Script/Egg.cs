using UnityEngine;

public class Egg : MonoBehaviour
{
    public float lifeTime = 5f;              // sau thời gian này nếu chưa chạm đất thì biến mất
    public GameObject breakEffectPrefab;     // hiệu ứng vỡ trứng
    public float breakEffectDuration = 1.5f; // thời gian tồn tại của hiệu ứng vỡ
    public float breakOffset = 1f;           // khoảng cách cao hơn đáy map

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (MapBounds.Instance != null && transform.position.y <= MapBounds.Instance.MinY + breakOffset)
        {
            BreakEgg();
        }
    }

    void BreakEgg()
    {
        if (breakEffectPrefab != null)
        {
            GameObject effect = Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, breakEffectDuration);
        }

        Destroy(gameObject);
    }
}
