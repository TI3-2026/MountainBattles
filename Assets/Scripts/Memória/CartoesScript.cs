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
    public GameObject cartaObj;
    public Material materialMatch;
    // UI
    public Canvas canvas;
    public TextMeshProUGUI infoMontanha;
    public Image ImgMontanha;


    [Header("Variáveis de controle. Não modificar.")]
    public int duplaValor;
    private bool desabilitado = false;
    private bool clicavel = true;
    

    private void Start()
    {
        transform.Rotate(0, 180, 0);
        canvas.worldCamera = Camera.main;

        infoMontanha.enabled = true;
        ImgMontanha.enabled = true;
        clicavel = false; 

        ManagerLevel.Instance.onCardsEnabled.AddListener(Clicavel);
    }


    public void DefinirCarta(Sprite sprite, string texto)
    {
        ImgMontanha.sprite = sprite;
        infoMontanha.text = texto;
    }

    public void Clicavel(bool clicavel)
    {
        this.clicavel = clicavel;
    }

    // Match
    public void DefinirMaterialMatch() => cartaObj.GetComponent<Renderer>().material = materialMatch;
    public void DesabilitarCarta() => desabilitado = true;

    public void ClicarCarta()
    {
        if (desabilitado) return;

        if (clicavel)
        {
            Flip(true);
            clicavel = false;

            ManagerLevel.Instance.CartaClicada(duplaValor, this);
        }
    }

    public void ErrouMatch(float tempo) => StartCoroutine(I_ErroMatch(tempo));
    private IEnumerator I_ErroMatch(float tempo)
    {
        clicavel = false;
        yield return new WaitForSeconds(tempo);
        Flip(false);
        clicavel = true;
    }

    public void Flip(bool showFront)
    {
        LeanTween.cancel(gameObject);

        if (showFront) LeanTween.rotateLocal(gameObject, new Vector3(0, 180, 0), tempoFlip).setEaseOutQuad();
        else LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 0), tempoFlip).setEaseOutQuad();
    }

    
}
