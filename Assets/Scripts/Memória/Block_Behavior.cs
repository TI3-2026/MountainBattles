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
    public TextMeshProUGUI infoMontanha;
    public Image ImgMontanha;


    private bool clickable = true;
    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
        canvas.worldCamera = Camera.main;
        //number.enabled = false;
        infoMontanha.enabled = true;
        ImgMontanha.enabled = true;
        clickable = false; 

        Manager_Game.Instance.manager_level.onCardsEnabled.AddListener(Clickabe);
    }

    /*
    public void ChangeImage(Sprite sprite)
    {
        img.sprite = sprite;
    }*/

    public void SetCard(Sprite sprite, string texto)
    {
        ImgMontanha.sprite = sprite;
        infoMontanha.text = texto;
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
            img.color = new Color(0f, 1f, 0f, 0.2f);
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
        LeanTween.cancel(gameObject);

        transform.localRotation = Quaternion.identity;

        LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 90), 0.15f).setOnComplete(() =>
        {
            infoMontanha.enabled = showFront;
            ImgMontanha.enabled = showFront;

            LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 180), 0.15f).setOnComplete(() =>
            {
                transform.localRotation = Quaternion.identity;
            });
        });
    }

    IEnumerator ErroMatch()
    {
        clickable = false;
        img.color = new Color(1f, 0f, 0f, 0.4f);
        yield return new WaitForSeconds(1f);
        img.color = new Color(0f, 0f, 0f, 0f);
        Flip(false);
        clickable = true;
    }
}
