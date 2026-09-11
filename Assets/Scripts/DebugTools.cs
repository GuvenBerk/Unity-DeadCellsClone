using UnityEngine;

public class DebugTools : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Tum PlayerPrefs silindi!");
        }
    }
}