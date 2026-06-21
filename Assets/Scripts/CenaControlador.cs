using UnityEngine;
using UnityEngine.SceneManagement;

public class CenaControlador : MonoBehaviour
{
    private const float DoubleTapThreshold = 0.3f;
    private float lastTapTime = -1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeScene("Menu");
        }
    }

    public void ChangeScene(string scene)
    {
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
