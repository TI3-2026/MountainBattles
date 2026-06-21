using UnityEngine;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        if (sceneName == "Batalha") GoogleFormsAnalytics.Instance.SendForm(resJogouBatalha:true);
        else if (sceneName == "Memoria") GoogleFormsAnalytics.Instance.SendForm(resJogouMemoria:true);
        SceneManager.LoadScene(sceneName);
    }
}
