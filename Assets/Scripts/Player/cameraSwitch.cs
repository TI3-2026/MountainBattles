using UnityEngine;

public class cameraSwitch : MonoBehaviour
{
    public Camera menuCamera;
    public Camera playerCamera;
    public GameObject menuCanvas;
    public GameObject menuGeralCanvas;

    void Start()
    {
        menuCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        menuCanvas.SetActive(true);
        menuGeralCanvas.SetActive(false);
    }

    public void StartGame()
    {
        menuCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        menuCanvas.SetActive(false);
        menuGeralCanvas.SetActive(true);
    }
}