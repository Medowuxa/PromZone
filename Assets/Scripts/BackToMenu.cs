using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    // Метод для возврата в главное меню
    public void GoToMainMenu()
    {
        Debug.Log("Возвращаемся в главное меню...");
        SceneManager.LoadScene("MainMenu"); // Замените "MainMenu" на имя вашей начальной сцены
    }
}
