using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public int maxJumps = 2;
    private int jumpCount = 0;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    public float dashSpeed = 12f;
    public float dashDuration = 0.4f;
    private bool isDashing = false;
    public bool isInvincible = false;
    private float dashTimer = 0f;
    public float diveSpeed = 20f;
    public int diveDamage = 15;
    private bool isDiving = false;
    public int attackDamage = 10;
    public float attackRange = 1f;
    public static bool canAttack = false;
    public GameObject attackEffectPrefab;
    public GameObject bulletPrefab;
    private PlayerEnergy playerEnergy;
    private PlayerHealth playerHealth;
    private bool hasSkill = false;
    public int maxArrows = 5;
    private int currentArrows;
    public float arrowCooldown = 0.5f;
    private float arrowCooldownTimer = 0f;
    public float arrowRegenTime = 3f;
    private float arrowRegenTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerEnergy = GetComponent<PlayerEnergy>();
        playerHealth = GetComponent<PlayerHealth>();
        currentArrows = maxArrows;
    }

    void Update()
    {
        arrowCooldownTimer -= Time.deltaTime;
        arrowRegenTimer += Time.deltaTime;
        if (arrowRegenTimer >= arrowRegenTime && currentArrows < maxArrows)
        {
            currentArrows++;
            arrowRegenTimer = 0f;
        }

        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space) && !isGrounded && canAttack)
        {
            isDiving = true;
            jumpCount = maxJumps;
            rb.linearVelocity = new Vector2(0, -diveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            isDashing = true;
            isInvincible = true;
            dashTimer = dashDuration;
        }

        if (isDashing)
        {
            rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, rb.linearVelocity.y);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;
                isInvincible = false;
            }
        }
        else if (!isDiving)
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

    public void GiveArrow()
    {
        if (currentArrows < maxArrows)
        {
            currentArrows++;
        }
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
        if (currentArrows <= 0 || arrowCooldownTimer > 0)
        {
            return;
        }
        Vector2 shootPosition = (Vector2)transform.position + new Vector2(transform.localScale.x * 0.5f, 0);
        GameObject bullet = Instantiate(bulletPrefab, shootPosition, Quaternion.identity);
        if (transform.localScale.x < 0)
        {
            bullet.transform.localScale = new Vector3(-bullet.transform.localScale.x, bullet.transform.localScale.y, bullet.transform.localScale.z);
        }
        currentArrows--;
        arrowCooldownTimer = arrowCooldown;
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
            if (isDiving)
            {
                isDiving = false;
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 1f);
                foreach (Collider2D enemyCollider in hitEnemies)
                {
                    EnemyHealth enemy = enemyCollider.GetComponent<EnemyHealth>();
                    if (enemy != null)
                    {
                        Debug.Log("Dive hasari veriliyor: " + enemyCollider.name + " | Miktar: " + diveDamage);
                        enemy.TakeDamage(diveDamage);
                    }
                }
            }
            isGrounded = true;
            jumpCount = 0;
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