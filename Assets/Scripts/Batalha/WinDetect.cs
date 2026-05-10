using UnityEngine;
using UnityEngine.SceneManagement;

public class WinDetect : MonoBehaviour
{
    public bool isWin = false;

    private void OnCollisionEnter(Collision collision) {
        if (isWin) {
            Debug.Log("Win");
            SceneManager.LoadScene(0);
        }else {
            Debug.Log("Lose");
            SceneManager.LoadScene(0);
        }
    }
}
