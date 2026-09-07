using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    private bool isGrounded = true;
    public float dashSpeed = 5f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    public int attackDamage = 10;
    public float attackRange = 1f;
    public static bool canAttack = false;
    public GameObject attackEffectPrefab;
    public GameObject bulletPrefab;
    private PlayerEnergy playerEnergy;
    private bool hasSkill = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerEnergy = GetComponent<PlayerEnergy>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            isDashing = true;
            dashTimer = dashDuration;
        }

        if (isDashing)
        {
            rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, rb.linearVelocity.y);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
        }
    }

    public void GiveSkill()
    {
        hasSkill = true;
    }

    void Attack()
    {
        if (!canAttack)
        {
            return;
        }
        Debug.Log("Attack!");
        Vector2 attackPosition = (Vector2)transform.position + new Vector2(transform.localScale.x * (attackRange + 0.5f), 0);
        GameObject effect = Instantiate(attackEffectPrefab, attackPosition, Quaternion.identity);
        Destroy(effect, 0.2f);
        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPosition, 0.5f);

        Debug.Log("Collider found: " + hitEnemy);

        if (hitEnemy != null)
        {
            EnemyHealth enemy = hitEnemy.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                playerEnergy.GainEnergy(10);
            }
        }
    }

    void Shoot()
    {
        if (!canAttack)
        {
            return;
        }
        Vector2 shootPosition = (Vector2)transform.position + new Vector2(transform.localScale.x * 0.5f, 0);
        GameObject bullet = Instantiate(bulletPrefab, shootPosition, Quaternion.identity);
        if (transform.localScale.x < 0)
        {
            bullet.transform.localScale = new Vector3(-bullet.transform.localScale.x, bullet.transform.localScale.y, bullet.transform.localScale.z);
        }
    }

    void UseSkill()
    {
        if (!canAttack)
        {
            return;
        }
        if (!hasSkill)
        {
            return;
        }
        bool success = playerEnergy.UseEnergy(30f);
        if (success)
        {
            Debug.Log("Skill used!");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 3f);
            foreach (Collider2D enemyCollider in hitEnemies)
            {
                EnemyHealth enemy = enemyCollider.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(20);
                }
            }
        }
        else
        {
            Debug.Log("Not enough energy!");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 attackPosition = (Vector2)transform.position + new Vector2(transform.localScale.x * (attackRange + 0.5f), 0);
        Gizmos.DrawWireSphere(attackPosition, 0.5f);
    }
}