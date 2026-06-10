using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CartoesScript : MonoBehaviour
{
    [Header("Configurações")]
    public float tempoFlip = 0.15f;

    [Header("Referencias")]
    public Canvas canvas;
    public TextMeshProUGUI infoMontanha;
    public Image ImgMontanha;


    [Header("Auto Preenchivel")]
    public int block_value;

    

    private bool clicavel = true;
    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
        canvas.worldCamera = Camera.main;
        //number.enabled = false;
        infoMontanha.enabled = true;
        ImgMontanha.enabled = true;
        clicavel = false; 

        ManagerLevel.Instance.onCardsEnabled.AddListener(Clicavel);
    }

    public void SetCard(Sprite sprite, string texto)
    {
        ImgMontanha.sprite = sprite;
        infoMontanha.text = texto;
    }

    private void Clicavel(bool clicavel)
    {
        this.clicavel = clicavel;
    }

    public void ClicarCarta()
    {
        if (clicavel)
        {
            Flip(true);
            clicavel = false;

            ManagerLevel.Instance.CartaClicada(block_value, this);
        }
    }

    public void ErrouMatch() => StartCoroutine(I_ErroMatch());
    private IEnumerator I_ErroMatch()
    {
        clicavel = false;
        yield return new WaitForSeconds(1f);
        Flip(false);
        clicavel = true;
    }

    public void Flip(bool showFront)
    {
        LeanTween.cancel(gameObject);

        if (showFront) LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 0), tempoFlip).setEaseOutQuad();
        else LeanTween.rotateLocal(gameObject, new Vector3(0, 0, -180), tempoFlip).setEaseOutQuad();
    }

    
}
