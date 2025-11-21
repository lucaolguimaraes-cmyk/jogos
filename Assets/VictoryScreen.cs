using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public GameObject victoryPanel;
    public GameObject victoryText;
    public GameObject restartButton;
    public GameObject Agradecimentos;
    public GameObject hudParent;


    void Start()
    {
        // Começa invisível
        victoryPanel.SetActive(false);
        victoryText.SetActive(false);
        restartButton.SetActive(false);
        Agradecimentos.SetActive(false);
    }

    public void ShowVictory()
    {
        // Ativa todos os elementos
        victoryPanel.SetActive(true);
        victoryText.SetActive(true);
        restartButton.SetActive(true);
        Agradecimentos.SetActive(true);

        if(hudParent != null)
            hudParent.SetActive(false);

        // Pausa o jogo
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
