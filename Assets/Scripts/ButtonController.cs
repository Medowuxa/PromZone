using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public Sprite normalSprite; // Спрайт кнопки в стандартном состоянии
    public Sprite pressedSprite; // Спрайт кнопки в нажатом состоянии
    public GameObject linkedDoor; // Связанная дверь

    private SpriteRenderer spriteRenderer; // Компонент для управления спрайтом
    private bool isPressed = false; // Флаг состояния кнопки

    void Start()
    {
        // Получаем компонент SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Устанавливаем начальный спрайт
        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
        else
        {
            Debug.LogError("ButtonController: SpriteRenderer or Normal Sprite is missing!");
        }

        // Проверяем, находится ли персонаж уже на клетке с кнопкой
        CheckIfPlayerIsAlreadyOnButton();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что персонаж активировал кнопку
        if (other.CompareTag("Player") && !isPressed)
        {
            ActivateButton();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Если персонаж покинул клетку с кнопкой, можно вернуть её в исходное состояние
        if (other.CompareTag("Player") && !isPressed)
        {
            ResetButton();
        }
    }

    void CheckIfPlayerIsAlreadyOnButton()
    {
        // Проверяем, есть ли персонаж в текущей клетке
        Collider2D playerCollider = Physics2D.OverlapBox(transform.position, Vector2.one * 0.9f, 0);
        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            ActivateButton();
        }
    }

    void ActivateButton()
    {
        // Меняем спрайт на нажатый
        if (spriteRenderer != null && pressedSprite != null)
        {
            spriteRenderer.sprite = pressedSprite;
        }

        // Активируем связанную дверь
        if (linkedDoor != null)
        {
            DoorController doorController = linkedDoor.GetComponent<DoorController>();
            if (doorController != null)
            {
                doorController.OpenDoor(); // Открываем дверь
            }
            else
            {
                Debug.LogWarning("ButtonController: Linked door does not have a DoorController component!");
            }
        }

        isPressed = true; // Блокируем повторное нажатие
    }

    void ResetButton()
    {
        // Меняем спрайт обратно на нормальный
        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }

        isPressed = false; // Разблокируем повторное нажатие
    }
}