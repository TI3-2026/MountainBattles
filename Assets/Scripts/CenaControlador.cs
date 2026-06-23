using UnityEngine;
using UnityEngine.SceneManagement;

public class CenaControlador : MonoBehaviour
{
    private bool trocandoCena = false;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!trocandoCena) ChangeScene("Menu");
        }
    }

    public void ChangeScene(string scene)
    {
        trocandoCena = true;
        
        if (ManagerLevel.Instance != null)
        {
            ManagerLevel.Instance.EnviarDados();
        }else if (ManagerBattle.Instance != null)
        {
            ManagerBattle.Instance.EnviarDados();
        }
        
        SceneManager.LoadScene(scene);
    }
}
