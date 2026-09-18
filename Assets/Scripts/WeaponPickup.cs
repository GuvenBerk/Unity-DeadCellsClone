using UnityEngine;
using TMPro;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponData;
    public TextMeshPro promptText;
    private bool playerInRange = false;
    private PlayerMovement player;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!PlayerMovement.canAttack)
        {
            spriteRenderer.enabled = false;
        }

        promptText.text = "E";
        promptText.fontSize = 10;
        promptText.transform.localPosition = new Vector3(0, 1f, 0);
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.color = Color.black;
        promptText.gameObject.SetActive(false);

    }

    void Update()
    {
        if (!PlayerMovement.canAttack)
        {
            return;
        }
        if (!spriteRenderer.enabled)
        {
            spriteRenderer.enabled = true;
        }
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Picking up weapon: " + weaponData.weaponName);
            player.PickUpWeapon(weaponData);
            Destroy(gameObject);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!PlayerMovement.canAttack)
        {
            return;
        }
        if (other.GetComponent<PlayerMovement>() != null)
        {
            player = other.GetComponent<PlayerMovement>();
            playerInRange = true;
            promptText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            playerInRange = false;
            promptText.gameObject.SetActive(false);
        }
    }
}