using UnityEngine;
using UnityEngine.SceneManagement;

public class BotaoScript : MonoBehaviour
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
    
    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}