using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private PlayerMovement playerMovement;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {

    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead)
        {
            return;
        }
        if (playerMovement.isInvincible)
        {
            return;
        }

        currentHealth -= damageAmount;
        Debug.Log("Player health: " + currentHealth);
        StartCoroutine(FlashHurt());

        if (currentHealth <= 0)
        {
            isDead = true;
            Debug.Log("Player died!");
            PlayerMovement.canAttack = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator FlashHurt()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
}