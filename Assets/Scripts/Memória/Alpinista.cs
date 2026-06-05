using System.Collections;
using UnityEngine;

public class Alpinista : MonoBehaviour
{
    [Header("Movimentacao")]
    public GameObject[] pos_movimentos;
    public int movimentos = 0;
    public float movimento_duracao = 1f;
    private int total_movimentos;

    public Vector3 alpinistavtr;
    public Vector3 movimento = new Vector3(0, 12, 0);
    public Vector3 inicio = new Vector3(-4, -24, 100);
    
    private void Start()
    {
        alpinistavtr = inicio;

        transform.position = pos_movimentos[0].transform.position;
        total_movimentos = pos_movimentos.Length-1;
    }

    public void AcertouCarta()
    {
        movimentos++;
        LeanTween.move(gameObject, pos_movimentos[movimentos].transform.position, movimento_duracao).setEase(LeanTweenType.easeInOutSine);
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
