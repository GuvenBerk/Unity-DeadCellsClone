using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 10f;
    private Vector2 startPosition;
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        float direction = Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(speed * direction, 0);
    }

    void Update()
    {
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(10);
            Destroy(gameObject);
        }
    }
}