using UnityEngine;

public class SkillPickup : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.GiveSkill();
            Destroy(gameObject);
        }
    }
}