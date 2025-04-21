using UnityEngine;

public class KeyController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // Уничтожаем ключ после сбора
        }
    }
}