using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [Header("Boss Stats")]
    public int health = 100;

    [Header("UI Settings")]
    public Slider healthBar;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    private float fireTimer;

    [Header("Attack Modes")]
    public bool useFanAttack = false;
    public bool useDirectAttack = true;
    public int fanBulletCount = 8;
    public float bulletSpeed = 5f;

    [Header("Movement")]
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;

    private Transform targetPoint;
    private EnemySpawner spawner;
    private Transform player;

    void Start()
    {
        // tìm player để direct attack
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // gán target đầu tiên
        targetPoint = pointB;

        // setup UI máu
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.maxValue = health;
            healthBar.value = health;
        }
    }

    void Update()
    {
        Move();

        // xử lý bắn
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            if (useFanAttack) FanAttack();
            if (useDirectAttack) DirectAttack();
            fireTimer = 0f;
        }
    }

    // ================== Movement ==================
    void Move()
    {
        if (pointA == null || pointB == null) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }

    // ================== Attack ==================
    void FanAttack()
    {
        if (bulletPrefab == null || firePoint == null) return;

        for (int i = 0; i < fanBulletCount; i++)
        {
            float angle = (360f / fanBulletCount) * i;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = bullet.transform.up * bulletSpeed;
        }
    }

    void DirectAttack()
    {
        if (bulletPrefab == null || firePoint == null || player == null) return;

        Vector2 dir = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = dir * bulletSpeed;
    }

    // ================== Health ==================
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (healthBar != null) healthBar.value = health;
        if (health <= 0) Die();
    }

    public void SetSpawner(EnemySpawner s)
    {
        spawner = s;
    }

    void Die()
    {
        Debug.Log("Boss defeated!");
        if (spawner != null)
        {
            spawner.BossDied();   // chỉ gọi BossDied, không gọi EnemyDied
        }

        if (healthBar != null)
            Destroy(healthBar.gameObject);

        Destroy(gameObject);
    }
    // ✅ Hàm khởi tạo boss, được gọi từ EnemySpawner
    public void Init(EnemySpawner s, Transform a, Transform b)
    {
        spawner = s;
        pointA = a;
        pointB = b;
        targetPoint = (pointB != null) ? pointB : pointA;

        Debug.Log($"[Boss] Init -> A={(pointA ? pointA.name : "null")} B={(pointB ? pointB.name : "null")} target={(targetPoint ? targetPoint.name : "null")}");
    }

}
