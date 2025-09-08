using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public float spawnHeight = 0f;

    [Header("Wave Settings")]
    public float waveDelay = 2f;
    private int aliveCount = 0;
    //private bool waveActive = false;
    private float waveTimer = 0f;
    private int waveNumber = 0;

    // ✅ Cờ chống gọi NextWave 2 lần
    private bool awaitingNextWave = false;

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public int wavePerBoss = 3;
    private bool bossActive = false;

    // ✅ Truyền pointA/pointB cho Boss sau khi Instantiate
    public Transform bossPointA;
    public Transform bossPointB;

    [Header("UI")]
    public WaveUI waveUI;

    void Start()
    {
        waveNumber = 1;
        SpawnWave();
        //waveActive = true;

        if (waveUI != null)
            waveUI.UpdateWave(waveNumber);

        Debug.Log($"[Spawner] Start -> wave {waveNumber}");
    }

    void Update()
    {
        // ✅ CHỈ duy nhất cơ chế này sinh wave mới sau delay
        if (awaitingNextWave)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0f)
            {
                awaitingNextWave = false;
                NextWave();
            }
        }
    }

    void SpawnWave()
    {
        // Nếu là wave boss
        if (waveNumber % wavePerBoss == 0 && bossPrefab != null)
        {
            SpawnBoss();
            Debug.Log($"[Spawner] Wave {waveNumber} -> Boss appeared!");
            return;
        }

        int before = aliveCount;
        int pattern = Random.Range(0, 3);

        if (pattern == 0) SpawnLine();
        else if (pattern == 1) SpawnRectangle();
        else SpawnZigZag();

        int spawned = aliveCount - before;
        //waveActive = true;

        Debug.Log($"[Spawner] Spawn wave {waveNumber} with {spawned} enemies. Total alive: {aliveCount}");
    }

    void SpawnLine()
    {
        for (int i = -4; i <= 4; i++)
            SpawnEnemy(new Vector3(i * 1.5f, transform.position.y + spawnHeight, 0));
    }

    void SpawnRectangle()
    {
        for (int y = 0; y < 3; y++)
            for (int x = -3; x <= 3; x++)
                SpawnEnemy(new Vector3(x * 1.5f, transform.position.y + spawnHeight - y * 1.5f, 0));
    }

    void SpawnZigZag()
    {
        for (int i = -4; i <= 4; i++)
            SpawnEnemy(new Vector3(
                i * 1.5f,
                transform.position.y + spawnHeight + ((i % 2 == 0) ? 0f : -1.5f),
                0));
    }

    void SpawnEnemy(Vector3 pos)
    {
        GameObject e = Instantiate(enemyPrefab, pos, Quaternion.identity);
        Chicken c = e.GetComponentInChildren<Chicken>();
        if (c != null)
        {
            c.SetSpawner(this);
            aliveCount++;
        }
        else
        {
            Debug.LogWarning("[Spawner] Spawned prefab has NO Chicken script: " + e.name);
        }
    }

    void SpawnBoss()
    {
        if (bossActive) return;

        GameObject b = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        // Boss script có thể ở root hoặc ở con → lấy cả trong children
        Boss boss = b.GetComponent<Boss>();
        if (boss == null) boss = b.GetComponentInChildren<Boss>();

        if (boss != null)
        {
            // ✅ Truyền Spawner + 2 điểm A/B cho boss tại runtime
            boss.Init(this, bossPointA, bossPointB);

            Debug.Log($"[Spawner] Boss spawned at {bossSpawnPoint.position}. " +
                      $"A={(bossPointA ? bossPointA.name : "null")} B={(bossPointB ? bossPointB.name : "null")}");
        }
        else
        {
            Debug.LogError("[Spawner] Boss prefab does not contain Boss script!");
        }

        bossActive = true;
        //waveActive = true;   // đang trong boss fight
        aliveCount = 1;      // boss cũng tính là 1
    }

    public void EnemyDied(GameObject enemy)
    {
        // Nếu lỡ có ai gọi EnemyDied cho boss → bỏ qua, vì dùng BossDied() riêng.
        if (enemy.CompareTag("Boss"))
        {
            Debug.LogWarning("[Spawner] EnemyDied called with Boss tag. Ignored. Use BossDied().");
            return;
        }

        aliveCount = Mathf.Max(0, aliveCount - 1);

        if (aliveCount == 0 && !bossActive)
        {
            //waveActive = false;

            // ✅ Lên lịch wave mới thông qua cơ chế duy nhất (awaitingNextWave)
            awaitingNextWave = true;
            waveTimer = waveDelay;

            Debug.Log($"[Spawner] Wave {waveNumber} finished. Next in {waveDelay}s");
        }
    }

    // ✅ Boss gọi hàm này khi chết
    public void BossDied()
    {
        bossActive = false;
        aliveCount = 0;
        //waveActive = false;

        // ✅ Chỉ lên lịch bằng cờ chờ wave mới (tránh NextWave 2 lần)
        awaitingNextWave = true;
        waveTimer = waveDelay;

        Debug.Log($"[Spawner] Wave {waveNumber} finished (Boss defeated). Next in {waveDelay}s");
    }

    private void NextWave()
    {
        waveNumber++;
        if (waveUI != null) waveUI.UpdateWave(waveNumber);

        Debug.Log($"[Spawner] >>> NEXT WAVE: {waveNumber}");

        SpawnWave();
        // waveActive được set trong SpawnWave()
    }
}
