using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float attackCooldown = 1f;
    private float cooldownTimer = 0f;
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
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                if (PlayerMovement.canAttack)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
                else
                {
                    playerHealth.TakeDamage(50);
                }

                cooldownTimer = attackCooldown;
            }
        }
    }
}