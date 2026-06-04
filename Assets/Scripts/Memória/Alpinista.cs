using System.Collections;
using UnityEngine;

public class Alpinista : MonoBehaviour
{
    [Header("References")]

    public Transform pos_inicio;
    public Transform pos_final;

    [Header("Variables")]
    public int total_movimentos = 10;
    public int movimentos = 0;
    public float movimento_duracao = 1f;

    public Vector3 alpinistavtr;
    public Vector3 movimento = new Vector3(0, 12, 0);
    public Vector3 inicio = new Vector3(-4, -24, 100);
    
    private void Start()
    {
        alpinistavtr = inicio;
    }

    public void AcertouCarta()
    {
        movimentos++;

        Vector3 movement = (pos_final.position - pos_inicio.position)/total_movimentos;
        movement *= movimentos;
        
        Vector3 destino = pos_inicio.position + movement;
        LeanTween.move(gameObject, destino, movimento_duracao).setEase(LeanTweenType.easeInOutSine);
    }

    // ! Refazer
    public void ErrouCarta(ref int erro)
    {
        if(transform.position.y > inicio.y && erro == 1)
        {
            Vector3 destino = transform.position + Vector3.down/2;
            LeanTween.move(gameObject, destino, 0.5f);
        }
        else if(transform.position.y > inicio.y && erro == 2)
        {
            Vector3 destino = transform.position + Vector3.down;
            LeanTween.move(gameObject, destino, 0.5f);
        }
        else if (transform.position.y > inicio.y && erro >= 3)
        {
            LeanTween.move(gameObject, alpinistavtr - movimento, 0.5f).setOnComplete(() =>
            {
                alpinistavtr = transform.position;
            });
            erro = 0;
        }
    }
}
