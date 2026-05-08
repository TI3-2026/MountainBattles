using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            if (panel.activeSelf)
                panel.SetActive(false);
            else
                panel.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Fechando o jogo...");
    }

    // NOVA FUNÇÃO (só isso aqui)
    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}