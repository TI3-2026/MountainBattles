using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Block_Behavior : MonoBehaviour
{
    [Header("Dont Touch")]
    public int block_value;

    [Header("References")]
    public Image img;


    private bool clickable = true;

    private void Start()
    {
        //Canvas canvas = GetComponentInParent<Canvas>();
        //canvas.worldCamera = Camera.main;
    }

    /*
    public void ChangeImage(Sprite sprite)
    {
        img.sprite = sprite;
    }*/

    private void OnMouseUp() => ClickBlock();

    public void ClickBlock()
    {
        if (clickable)
        {
            Debug.Log("Clicked block with value: " + block_value);
            img.color = Color.cyan;
            clickable = false;

            Manager_Game.Instance.manager_level.BlockClicked(block_value, this);
        }
    }

    public void Match()
    {
        Debug.Log("Match!");
    }

    public void UnMatch()
    {
        Debug.Log("UnMatch!");
        img.color = Color.white;
        clickable = true;
    }
}
