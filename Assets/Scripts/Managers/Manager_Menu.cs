using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Manager_Menu : MonoBehaviour
{

    private UIDocument uiDocument;
    private Button button_lab;
    private Button button_mem;
    private Button button_battle;

    private void Start() {
        uiDocument = GetComponent<UIDocument>();

        button_lab = uiDocument.rootVisualElement.Q<Button>("button_lab");
        button_mem = uiDocument.rootVisualElement.Q<Button>("button_mem");
        button_battle = uiDocument.rootVisualElement.Q<Button>("button_battle");

        button_lab.clicked += Click_Lab;
        button_mem.clicked += Click_Mem;
        button_battle.clicked += Click_Battle;
    }

    private void Click_Lab() {
        SceneManager.LoadScene("Labirinto");
    }

    private void Click_Mem() {
        SceneManager.LoadScene("Memoria");
    }

    private void Click_Battle() {
        SceneManager.LoadScene("Batalha");
    }
}
