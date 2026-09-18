using UnityEngine;

public class MemoryFragmentPickup : MonoBehaviour
{
    public int fragmentIndex = 0;
    void Start()
    {
        if (PlayerPrefs.GetInt("MemoryFragment_" + fragmentIndex, 0) == 1)
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            PlayerPrefs.SetInt("MemoryFragment_" + fragmentIndex, 1);
            string message = "";
            switch (fragmentIndex)
            {
                case 0:
                    message = "...your brother...";
                    break;
                case 1:
                    message = "...go back to...";
                    break;
                case 2:
                    message = "...he's waiting...";
                    break;
            }
            FindFirstObjectByType<MemoryUI>().ShowMemory(message);
            Destroy(gameObject);
        }
    }
}