using UnityEngine;
using UnityEngine.SceneManagement;

public class openCanvas : MonoBehaviour
{
    public GameObject canvasL;
    public GameObject canvasM;
    public GameObject canvasB;

    void Start()
    {
        canvasL.SetActive(false);
        canvasM.SetActive(false);
        canvasB.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("Labirinto");
            //canvasL.SetActive(!canvasL.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene("Memoria");
            //canvasM.SetActive(!canvasM.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("Batalha");
            //canvasB.SetActive(!canvasB.activeSelf);
        }
    }
}