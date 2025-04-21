using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Метод для загрузки сцены
    public void LoadScene()
    {
        SceneManager.LoadScene("GameScene");
    }
    // Метод для перехода к экрану настроек
    public void GoToSettings()
    {
        Debug.Log("Переход к Настройкам...");
        // Здесь можно загрузить сцену с настройками или показать панель настроек
        SceneManager.LoadScene("Settings"); // Замените "SettingsScene" на имя вашей сцены с настройками
    }

    // Метод для перехода к экрану авторов
    public void GoToCredits()
    {
        Debug.Log("Переход к Авторам...");
        // Здесь можно загрузить сцену с авторами или показать панель авторов
        SceneManager.LoadScene("Author"); // Замените "CreditsScene" на имя вашей сцены с авторами
    }
    // Метод для выхода из игры
    public void ExitGame()
    {
        Application.Quit();
    }
}