using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Block_Behavior : MonoBehaviour
{
    [Header("Dont Touch")]
    public int block_value;

    [Header("References")]
    public Image img;

    private void Start()
    {
        //Canvas canvas = GetComponentInParent<Canvas>();
        //canvas.worldCamera = Camera.main;
        
        //img = GetComponentInParent<Image>();
    }

    /*
    public void ChangeImage(Sprite sprite)
    {
        img.sprite = sprite;
    }*/

    public void ClickBlock()
    {
        Debug.Log("oi");
    }
}
