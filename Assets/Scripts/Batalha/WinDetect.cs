using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinDetect : MonoBehaviour
{
    public bool isWin = false;
    public TextMeshProUGUI WinOrLose;

    public void Start()
    {
        WinOrLose.SetText("");
    }
    private void OnCollisionEnter(Collision collision) {
        StartCoroutine(LoseOrWin(isWin));
    }

    private IEnumerator LoseOrWin(bool IsWin)
    {
        yield return new WaitForSeconds(1f);
        if (IsWin)
        {
            WinOrLose.SetText("Vitória!");
            WinOrLose.color = Color.green;
        }
        else
        {
            WinOrLose.SetText("Derrota!");
            WinOrLose.color = Color.red;
        }
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("Menu");
    }
}
