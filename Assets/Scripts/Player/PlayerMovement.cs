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
    public WeaponData leftWeapon;
    private int leftAmmo;
    private float leftAmmoRegenTimer;
    public WeaponData rightWeapon;
    private int rightAmmo;
    private float rightAmmoRegenTimer;
    public float attackRange = 1f;
    public static bool canAttack = false;
    public GameObject attackEffectPrefab;
    private PlayerEnergy playerEnergy;
    private PlayerHealth playerHealth;
    private bool hasSkill = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerEnergy = GetComponent<PlayerEnergy>();
        playerHealth = GetComponent<PlayerHealth>();
        if (leftWeapon != null && leftWeapon.hasAmmo)
        {
            leftAmmo = leftWeapon.maxAmmo;
        }
        if (rightWeapon != null && rightWeapon.hasAmmo)
        {
            rightAmmo = rightWeapon.maxAmmo;
        }
    }

    void Update()
    {
        if (leftWeapon != null && leftWeapon.hasAmmo)
        {
            leftAmmoRegenTimer += Time.deltaTime;
            if (leftAmmoRegenTimer >= leftWeapon.ammoRegenTime && leftAmmo < leftWeapon.maxAmmo)
            {
                leftAmmo++;
                leftAmmoRegenTimer = 0f;
            }
        }
        if (rightWeapon != null && rightWeapon.hasAmmo)
        {
            rightAmmoRegenTimer += Time.deltaTime;
            if (rightAmmoRegenTimer >= rightWeapon.ammoRegenTime && rightAmmo < rightWeapon.maxAmmo)
            {
                rightAmmo++;
                rightAmmoRegenTimer = 0f;
            }
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
    }

    public void EquipLeftWeapon(WeaponData newWeapon)
    {
        leftWeapon = newWeapon;
    }

    public void PickUpWeapon(WeaponData newWeapon)
    {
        if (leftWeapon == null)
        {
            leftWeapon = newWeapon;
        }
        else if (rightWeapon == null)
        {
            rightWeapon = newWeapon;
        }
    }

    void Attack()
    {
        if (!canAttack)
        {
            return;
        }
        if (leftWeapon == null)
        {
            return;
        }
        Debug.Log("Attack!");
        if (leftWeapon.weaponType == WeaponType.Melee)
        {
            Vector2 attackPosition = (Vector2)transform.position + new Vector2(transform.localScale.x * (leftWeapon.attackRange + 0.5f), 0);
            GameObject effect = Instantiate(attackEffectPrefab, attackPosition, Quaternion.identity);
            Destroy(effect, 0.2f);
            Collider2D hitEnemy = Physics2D.OverlapCircle(attackPosition, 0.5f);

            Debug.Log("Collider found: " + hitEnemy);

            if (hitEnemy != null)
            {
                EnemyHealth enemy = hitEnemy.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(leftWeapon.damage);
                    playerEnergy.GainEnergy(10);
                }
            }
        }
        else
        {
            if (leftWeapon.hasAmmo && leftAmmo <= 0)
            {
                return;
            }
            Vector2 shootPosition = (Vector2)transform.position + new Vector2(transform.localScale.x * 0.5f, 0);
            GameObject bullet = Instantiate(leftWeapon.bulletPrefab, shootPosition, Quaternion.identity);
            if (transform.localScale.x < 0)
            {
                bullet.transform.localScale = new Vector3(-bullet.transform.localScale.x, bullet.transform.localScale.y, bullet.transform.localScale.z);
            }
            if (leftWeapon.hasAmmo)
            {
                leftAmmo--;
            }
        }
    }

    void Shoot()
    {
        if (!canAttack)
        {
            return;
        }
        if (rightWeapon == null)
        {
            return;
        }
        Debug.Log("Shoot!");
        if (rightWeapon.weaponType == WeaponType.Melee)
        {
            Vector2 attackPosition = (Vector2)transform.position + new Vector2(transform.localScale.x * (rightWeapon.attackRange + 0.5f), 0);
            GameObject effect = Instantiate(attackEffectPrefab, attackPosition, Quaternion.identity);
            Destroy(effect, 0.2f);
            Collider2D hitEnemy = Physics2D.OverlapCircle(attackPosition, 0.5f);

            Debug.Log("Collider found: " + hitEnemy);

            if (hitEnemy != null)
            {
                EnemyHealth enemy = hitEnemy.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(rightWeapon.damage);
                    playerEnergy.GainEnergy(10);
                }
            }
        }
        else
        {
            if (rightWeapon.hasAmmo && rightAmmo <= 0)
            {
                return;
            }
            Vector2 shootPosition = (Vector2)transform.position + new Vector2(transform.localScale.x * 0.5f, 0);
            GameObject bullet = Instantiate(rightWeapon.bulletPrefab, shootPosition, Quaternion.identity);
            if (transform.localScale.x < 0)
            {
                bullet.transform.localScale = new Vector3(-bullet.transform.localScale.x, bullet.transform.localScale.y, bullet.transform.localScale.z);
            }
            if (rightWeapon.hasAmmo)
            {
                rightAmmo--;
            }
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