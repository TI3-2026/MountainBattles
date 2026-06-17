using UnityEngine;
using UnityEngine.SceneManagement;

public class CenaControlador : MonoBehaviour
{
    private const float DoubleTapThreshold = 0.3f;
    private float lastTapTime = -1f;

    void Update()
    {
        DetectKeyboard();
    }

    void DetectKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeScene("Menu");
        }
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
