using UnityEngine;

public class ImmuneSystemAttack : MonoBehaviour
{
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    private float cooldownTimer = 0f;
    public GameObject bulletPrefab;
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= attackRange && cooldownTimer <= 0)
        {
            float direction = 1f;
            if (player.transform.position.x < transform.position.x)
            {
                direction = -1f;
            }
            Vector2 shootPosition = (Vector2)transform.position + new Vector2(direction * 1f, 0);
            cooldownTimer = attackCooldown;
            GameObject bullet = Instantiate(bulletPrefab, shootPosition, Quaternion.identity);
            bullet.GetComponent<Projectile>().firedByEnemy = true;
            if (direction < 0)
            {
                bullet.transform.localScale = new Vector3(-bullet.transform.localScale.x, bullet.transform.localScale.y, bullet.transform.localScale.z);
            }
        }
    }
}