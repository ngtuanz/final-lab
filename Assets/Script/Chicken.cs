using UnityEngine;

public class Chicken : MonoBehaviour
{
    [Header("Egg Drop")]
    public GameObject eggPrefab;
    public Transform dropPoint;
    public float minDropTime = 1f, maxDropTime = 3f;
    private float nextDropTime;

    [Header("Egg Settings")]
    public float eggFallSpeed = 5f;   // ✅ tốc độ rơi trứng

    [Header("Item Drop")]
    public GameObject drumstickPrefab;
    public GameObject powerupPrefab;
    [Range(0f, 1f)] public float powerupDropChance = 0.2f;

    public GameObject multiUpPrefab;
    [Range(0f, 1f)] public float multiUpDropChance = 0.15f;

    [Header("Explosion Effect")]
    public GameObject explosionPrefab;

    [Header("Movement")]
    public float speed = 1f;
    public Transform potionA, potionB;

    private Transform target;
    private float timer;
    private EnemySpawner spawner;

    void Start()
    {
        target = potionB;
        SetNextDropTime();
    }

    void Update()
    {
        // Di chuyển qua lại
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
            target = (target == potionA) ? potionB : potionA;

        // ✅ Chỉ thả trứng nếu Player đang alive
        if (GameManager.Instance != null && GameManager.Instance.isPlayerAlive)
        {
            timer += Time.deltaTime;
            if (timer >= nextDropTime)
            {
                GameObject egg = Instantiate(eggPrefab, dropPoint.position, Quaternion.identity);

                // ✅ chỉnh tốc độ rơi trứng
                Rigidbody2D rb = egg.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.down * eggFallSpeed;
                }

                SetNextDropTime();
                timer = 0;
            }
        }
    }

    void SetNextDropTime()
    {
        nextDropTime = Random.Range(minDropTime, maxDropTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bullet"))
        {
            Destroy(collision.gameObject);
            Die();
        }
    }

    public void Die()
    {
        // Spawn explosion + drumstick
        if (explosionPrefab) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        if (drumstickPrefab) Instantiate(drumstickPrefab, transform.position, Quaternion.identity);

        // Chỉ rơi 1 trong 2 loại: PowerUp hoặc MultiUp
        float roll = Random.value;
        if (powerupPrefab != null && roll <= powerupDropChance)
        {
            Instantiate(powerupPrefab, transform.position, Quaternion.identity);
        }
        else if (multiUpPrefab != null && roll <= (powerupDropChance + multiUpDropChance))
        {
            Instantiate(multiUpPrefab, transform.position, Quaternion.identity);
        }

        Debug.Log("Enemy died: " + gameObject.name);
        spawner?.EnemyDied(gameObject);
        Destroy(gameObject);
    }

    public void SetSpawner(EnemySpawner s) => spawner = s;
}
