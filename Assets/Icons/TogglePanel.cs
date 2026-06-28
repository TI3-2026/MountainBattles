using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    [Header("Painel que será ativado/desativado")]
    public GameObject panel;

    public void Toggle()
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}