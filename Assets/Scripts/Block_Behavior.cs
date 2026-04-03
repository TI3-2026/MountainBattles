using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Block_Behavior : MonoBehaviour
{
    private Image img;

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        canvas.worldCamera = Camera.main;
        img = GetComponentInParent<Image>();
    }

    public void ChangeImage(Sprite sprite)
    {
        img.sprite = sprite;
    }
}
