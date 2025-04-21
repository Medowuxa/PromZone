using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость перемещения между клетками
    private Vector2 screenBounds; // Границы экрана в мировых координатах
    private float cellSize = 1.12f; // Размер клетки в мировых координатах (112 пикселей при PPU = 100)
    private bool isMoving = false; // Флаг движения
    private Vector3 targetPosition; // Целевая позиция
    private SpriteRenderer spriteRenderer; // Компонент для управления спрайтом

    // Новая переменная для отслеживания движения коробки
    private GameObject boxToPush; // Коробка, которую нужно толкать
    private bool isPushingBox = false; // Флаг для движения коробки

    // Новый флаг для ключа
    private bool hasKey = false; // Флаг наличия ключа
    private bool gameFinished = false;


    void Start()
    {
        // Получаем границы экрана в мировых координатах
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        // Получаем компонент SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Начальная позиция должна быть кратна размеру клетки
        SnapToGrid();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!isMoving && !isPushingBox)
        {
            // Управление перемещением
            if (Input.GetKeyDown(KeyCode.W))
                TryMove(Vector3.up);
            else if (Input.GetKeyDown(KeyCode.S))
                TryMove(Vector3.down);
            else if (Input.GetKeyDown(KeyCode.A))
            {
                TryMove(Vector3.left);
                spriteRenderer.flipX = true; // Отражаем спрайт влево
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                TryMove(Vector3.right);
                spriteRenderer.flipX = false; // Возвращаем спрайт вправо
            }
        }

        // Плавное движение к целевой позиции
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Если достигли целевой позиции
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition; // Зафиксируем позицию
                isMoving = false;
            }
        }

        // Плавное движение коробки
        if (isPushingBox && boxToPush != null)
        {
            Vector3 currentBoxPosition = boxToPush.transform.position;
            Vector3 targetBoxPosition = boxToPush.GetComponent<BoxMover>().targetPosition;

            // Используем скорость из BoxMover
            float boxMoveSpeed = boxToPush.GetComponent<BoxMover>().moveSpeed;

            boxToPush.transform.position = Vector3.MoveTowards(currentBoxPosition, targetBoxPosition, boxMoveSpeed * Time.deltaTime);

            // Если коробка достигла целевой позиции
            if (Vector3.Distance(boxToPush.transform.position, targetBoxPosition) < 0.01f)
            {
                boxToPush.transform.position = targetBoxPosition; // Зафиксируем позицию коробки
                isPushingBox = false;
                boxToPush.GetComponent<BoxMover>().isMoving = false; // Остановить движение коробки
            }
        }
    }

    void TryMove(Vector3 direction)
    {
        // Проверяем, есть ли препятствие в соседней клетке
        Vector3 nextCell = transform.position + new Vector3(direction.x * cellSize, direction.y * cellSize, 0);
        Collider2D wallCollider = Physics2D.OverlapBox(nextCell, Vector2.one * 0.9f, 0);

        // Если впереди стена, блокируем движение
        if (wallCollider != null && wallCollider.CompareTag("Wall"))
        {
            Debug.Log("Нельзя пройти через стену!");
            return;
        }

        // Проверяем, есть ли коробка в соседней клетке
        Collider2D boxCollider = Physics2D.OverlapBox(nextCell, Vector2.one * 0.9f, 0);

        if (boxCollider != null && boxCollider.CompareTag("Box"))
        {
            // Если есть коробка, пытаемся её толкнуть
            PushBox(boxCollider.gameObject, direction);
        }
        else
        {
            // Если коробки нет, двигаемся сами
            SetTargetPosition(direction);
        }
    }

   void PushBox(GameObject box, Vector3 direction)
{
    // Рассчитываем новую позицию для коробки
    Vector3 newBoxPosition = box.transform.position + new Vector3(direction.x * cellSize, direction.y * cellSize, 0);

    // Проверяем, находится ли новая позиция в пределах границ
    if (IsWithinBounds(newBoxPosition))
    {
        // Проверяем, нет ли стены, закрытой двери или другой коробки за коробкой
        Collider2D wallCollider = Physics2D.OverlapBox(newBoxPosition, Vector2.one * 0.9f, 0);
        if (wallCollider != null && (wallCollider.CompareTag("Wall") || wallCollider.CompareTag("Door")))
        {
            Debug.Log(wallCollider.CompareTag("Door") ? "Коробка уперлась в закрытую дверь!" : "Коробка уперлась в стену!");
            return;
        }

        // Проверяем, нет ли другой коробки в новой позиции
        Collider2D otherBoxCollider = Physics2D.OverlapBox(newBoxPosition, Vector2.one * 0.9f, 0);
        if (otherBoxCollider != null && otherBoxCollider.CompareTag("Box"))
        {
            Debug.Log("Коробка уперлась в другую коробку!");
            return;
        }

        // Устанавливаем целевую позицию для коробки
        box.GetComponent<BoxMover>().SetTargetPosition(newBoxPosition);

        // Перемещаемся сами
        SetTargetPosition(direction);

        // Устанавливаем флаг для движения коробки
        boxToPush = box;
        isPushingBox = true;
    }
}

    void SetTargetPosition(Vector3 direction)
    {
        // Устанавливаем новую целевую позицию
        Vector3 newTarget = transform.position + new Vector3(direction.x * cellSize, direction.y * cellSize, 0);

        // Проверяем, находится ли новая позиция в пределах границ
        if (IsWithinBounds(newTarget))
        {
            targetPosition = newTarget;
            isMoving = true;
        }
    }

    bool IsWithinBounds(Vector3 position)
    {
        // Рассчитываем границы с учетом стен
        float topBoundary = screenBounds.y - 1.12f; // Верхняя граница с учетом верхней стены
        float bottomBoundary = -screenBounds.y + 1.12f; // Нижняя граница с учетом нижней стены
        float leftBoundary = -screenBounds.x; // Левая граница
        float rightBoundary = screenBounds.x; // Правая граница

        return position.x >= leftBoundary && position.x <= rightBoundary &&
               position.y >= bottomBoundary && position.y <= topBoundary;
    }

    void SnapToGrid()
    {
        // Привязываем текущую позицию к сетке
        Vector3 snappedPosition = new Vector3(
            Mathf.Round(transform.position.x / cellSize) * cellSize,
            Mathf.Round(transform.position.y / cellSize) * cellSize,
            transform.position.z
        );
        transform.position = snappedPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
{
    if (gameFinished) return;

    if (other.CompareTag("Key") && !hasKey)
    {
        hasKey = true;
        Debug.Log("Ключ собран!");
        OpenYellowDoor();
    }

    if (other.CompareTag("YellowDoor") && hasKey)
    {
        gameFinished = true; // чтобы не вызвать повторно
        ShowCompletionMessage();
    }
}


    void OpenYellowDoor()
    {
        // Находим все желтые двери на сцене
        GameObject[] yellowDoors = GameObject.FindGameObjectsWithTag("YellowDoor");

        foreach (GameObject door in yellowDoors)
        {
            DoorController doorController = door.GetComponent<DoorController>();
            if (doorController != null)
            {
                doorController.OpenDoor(); // Открываем дверь
            }
            else
            {
                Debug.LogWarning("YellowDoor does not have a DoorController component!");
            }
        }
    }

    void ShowCompletionMessage()
{
    Debug.Log("Поздравляем, вы прошли игру!");
    SceneManager.LoadScene("VictoryScene"); // Загрузка сцены победы
}


}