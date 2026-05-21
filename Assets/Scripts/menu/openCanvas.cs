using UnityEngine;

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
            canvasL.SetActive(!canvasL.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            canvasM.SetActive(!canvasM.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            canvasB.SetActive(!canvasB.activeSelf);
        }
    }
}