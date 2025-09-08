using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float appearSpeed = 3f, targetY = -3f;
    public GameObject bulletPrefab;
    public GameObject doubleBulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f, fireRate = 0.3f;

    Rigidbody2D rb;
    private bool hasDoubleShot = false;
    private bool hasMultiShot = false;
    bool canControl = false;

    [Header("Power-up")]
    public float doubleShotDuration = 5f;
    public float multiShotDuration = 5f;
    private SFXPlayer sfx;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Spawn());
        sfx = FindFirstObjectByType<SFXPlayer>();

    }

    void FixedUpdate()
    {
        if (!canControl) return;

        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector2 pos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;

        if (MapBounds.Instance)
            pos = MapBounds.Instance.ClampPosition(pos);

        rb.MovePosition(pos);
    }

    IEnumerator Spawn()
    {
        while (Mathf.Abs(transform.position.y - targetY) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(transform.position.x, targetY),
                appearSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = new Vector2(transform.position.x, targetY);

        canControl = true;
        InvokeRepeating(nameof(Shoot), 0, fireRate);
    }

    void Shoot()
    {
        if (hasMultiShot)
        {
            Vector2[] directions = new Vector2[]
            {
                Vector2.up,
                (Vector2.up + Vector2.left).normalized,
                (Vector2.up + Vector2.right).normalized,
                (Vector2.up*0.3f + Vector2.left).normalized,
                (Vector2.up*0.3f + Vector2.right).normalized
            };

            foreach (Vector2 dir in directions)
            {
                GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                b.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
            }
        }
        else if (hasDoubleShot)
        {
            GameObject bullet1 = Instantiate(doubleBulletPrefab, firePoint.position + new Vector3(0.3f, 0, 0), Quaternion.identity);
            GameObject bullet2 = Instantiate(doubleBulletPrefab, firePoint.position - new Vector3(0.3f, 0, 0), Quaternion.identity);
            bullet1.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * bulletSpeed;
            bullet2.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * bulletSpeed;
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * bulletSpeed;
        }

        if (sfx != null) sfx.PlayShoot();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("egg"))
        {
            // Player chết → báo GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
                Time.timeScale = 0;
            }

            Destroy(gameObject); // huỷ Player
        }

        if (collision.CompareTag("item"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Player ăn Item thường!");
        }

        if (collision.CompareTag("PowerUp"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(ActivateDoubleShot());
            Debug.Log("Player ăn PowerUp -> Bắn đôi!");
        }

        if (collision.CompareTag("MultiUp"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(ActivateMultiShot());
            Debug.Log("Player ăn MultiUp -> Bắn 5 hướng!");
        }
    }

    public IEnumerator ActivateDoubleShot()
    {
        hasDoubleShot = true;
        yield return new WaitForSeconds(doubleShotDuration);
        hasDoubleShot = false;
    }

    public IEnumerator ActivateMultiShot()
    {
        hasMultiShot = true;
        yield return new WaitForSeconds(multiShotDuration);
        hasMultiShot = false;
    }
}
