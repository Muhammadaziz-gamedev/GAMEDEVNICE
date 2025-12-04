using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;

    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    void Update()
    {
        if (_isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    void Quit()
    {
        Application.Quit();
    }
}
