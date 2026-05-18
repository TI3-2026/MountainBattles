using UnityEngine;

public class cameraSwitch : MonoBehaviour
{
    public Camera menuCamera;
    public Camera playerCamera;
    public GameObject menuCanvas;

    void Start()
    {
        menuCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        menuCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        menuCanvas.SetActive(false);
    }
}