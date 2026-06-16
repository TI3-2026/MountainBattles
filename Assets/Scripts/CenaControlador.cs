using UnityEngine;
using UnityEngine.SceneManagement;

public class CenaControlador : MonoBehaviour
{
    private const float DoubleTapThreshold = 0.3f;
    private float lastTapTime = -1f;

    void Update()
    {
        DetectTouch();
        DetectKeyboard();
    }

    void DetectTouch()
    {
        if (Input.touchCount != 1)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
        {
            return;
        }

        if (Time.time - lastTapTime <= DoubleTapThreshold)
        {
            ChangeScene("Menu");
        }

        lastTapTime = Time.time;
    }

    void DetectKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeScene("Menu");
        }
    }

    void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
