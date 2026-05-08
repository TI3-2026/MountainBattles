using System.Collections;
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
        //number.enabled = false;
        number.enabled = true;
        clickable = false; 

        Manager_Game.Instance.manager_level.onCardsEnabled.AddListener(Clickabe);
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

    private void Clickabe(bool clickable)
    {
        this.clickable = clickable;
    }

    private void OnMouseUp() => ClickBlock();
    public void ClickBlock()
    {
        if (clickable)
        {
            /*img.color = Color.cyan;
            number.enabled = true;
            clickable = false;*/
            Flip(true);
            clickable = false;

            Manager_Game.Instance.manager_level.BlockClicked(block_value, this);
        }
    }

    public void Match(bool match)
    {
        if (match)
        {
            img.color = Color.green;
        }
        else
        {
            /*number.enabled = false;
            img.color = Color.white;
            clickable = true;*/
            StartCoroutine(ErroMatch());
        }
    }

    public void Flip(bool showFront)
    {
        LeanTween.rotateZ(gameObject, 90f, 0.15f).setOnComplete(() =>
        {
            number.enabled = showFront;
            LeanTween.rotateZ(gameObject, 180f, 0.15f).setOnComplete(() =>
            {
                transform.rotation = Quaternion.identity;
            });
        });
    }

    IEnumerator ErroMatch()
    {
        clickable = false;
        img.color = Color.red;
        yield return new WaitForSeconds(1f);
        img.color = Color.white;
        Flip(false);
        clickable = true;
    }
}
