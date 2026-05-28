using System.Collections;
using UnityEngine;

public class Alpinista : MonoBehaviour
{
    public GameObject alpinista;
    public Vector3 alpinistavtr;
    public Vector3 movimento = new Vector3(0, 12, 0);
    public Vector3 inicio = new Vector3(-4, -24, 100);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alpinistavtr = inicio;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AcertouCarta()
    {
        if(transform.position.y < 20)
        {
            LeanTween.move(gameObject, alpinistavtr + movimento, 0.5f).setOnComplete(() =>
            {
                alpinistavtr = transform.position;
            });
        }
    }
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
