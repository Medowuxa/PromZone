using UnityEngine;

public class BoxMover : MonoBehaviour
{
    public Vector3 targetPosition; // Целевая позиция коробки
    public bool isMoving = false; // Флаг движения

    public float moveSpeed = 5f; // Скорость перемещения коробки

    void Update()
    {
        if (isMoving)
        {
            Vector3 currentBoxPosition = transform.position;

            // Плавное движение к целевой позиции
            transform.position = Vector3.MoveTowards(currentBoxPosition, targetPosition, moveSpeed * Time.deltaTime);

            // Если коробка достигла целевой позиции
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition; // Зафиксируем позицию коробки
                isMoving = false;
            }
        }
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        isMoving = true;
    }
}