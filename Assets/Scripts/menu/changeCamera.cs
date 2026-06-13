using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class changeCamera : MonoBehaviour
{
    public CinemachineCamera currentCamera;
    public CinemachineCamera nextCamera;
    public GameObject canvastoOpen;
    public GameObject canvastoClose;

    public void OnMouseDown()
    {
        currentCamera.Priority = 0;
        nextCamera.Priority = 10;

        StartCoroutine(OpenCanvasAfterDelay());
    }

    IEnumerator OpenCanvasAfterDelay()
    {
        canvastoClose.SetActive(false);
        yield return new WaitForSeconds(2f);
        canvastoOpen.SetActive(true);
    }


    public void BackButton()
    {
        currentCamera.Priority= 10;
        nextCamera.Priority= 0;
        StartCoroutine(CloseCanvasAfterDelay());
    }

    IEnumerator CloseCanvasAfterDelay()
    {
        canvastoOpen.SetActive(false);
        yield return new WaitForSeconds(2f);
        canvastoClose.SetActive(true);
    }
}
