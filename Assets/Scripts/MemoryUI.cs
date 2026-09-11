using UnityEngine;
using TMPro;
using System.Collections;

public class MemoryUI : MonoBehaviour
{
    public TextMeshProUGUI memoryText;
    void Start()
    {

    }
    public void ShowMemory(string message)
    {
        memoryText.text = message;
        memoryText.gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    void Update()
    {

    }
    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        memoryText.gameObject.SetActive(false);
    }
}