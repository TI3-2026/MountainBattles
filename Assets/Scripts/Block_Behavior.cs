using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Block_Behavior : MonoBehaviour
{
    [Header("Dont Touch")]
    public int block_value;

    [Header("References")]
    public Canvas canvas;
    public Image img;
    public TextMeshProUGUI number;


    private bool clickable = true;

    private void Start()
    {
        canvas.worldCamera = Camera.main;
        number.enabled = false;
    }

    /*
    public void ChangeImage(Sprite sprite)
    {
        img.sprite = sprite;
    }*/

    public void SetNumber(int number)
    {
        this.number.text = number.ToString();
    }

    private void OnMouseUp() => ClickBlock();

    public void ClickBlock()
    {
        if (clickable)
        {
            img.color = Color.cyan;
            number.enabled = true;
            clickable = false;

            Manager_Game.Instance.manager_level.BlockClicked(block_value, this);
        }
    }

    public void Match()
    {
        // Debug.Log("Match!");
    }

    public void UnMatch()
    {
        // Debug.Log("UnMatch!");
        number.enabled = false;
        img.color = Color.white;
        clickable = true;
    }
}
