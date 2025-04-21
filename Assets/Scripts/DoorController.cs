using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool isOpen = false; // Флаг состояния двери

    void Update()
    {
        if (isOpen)
        {
            // Если это НЕ YellowDoor, то скрываем объект
            if (!CompareTag("YellowDoor"))
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void OpenDoor()
    {
        isOpen = true; // Открываем дверь
    }
}
