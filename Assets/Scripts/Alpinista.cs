using UnityEngine;

public class Alpinista : MonoBehaviour
{
    public GameObject alpinista;
    public Vector3 alpinistavtr;
    public Vector3 movimento = new Vector3(0, 6, 0);
    public Vector3 inicio = new Vector3(-4, -24, 100);
    public int Erro = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alpinistavtr = inicio;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Erro++;
            ErrouCarta(ref Erro);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            AcertouCarta();
        }
    }

    public void AcertouCarta()
    {
        if(alpinistavtr.y < 20)
        {
            alpinistavtr = alpinistavtr + movimento;
            alpinista.transform.position = alpinistavtr;
        }
    }
    public void ErrouCarta(ref int erro)
    {
        if(alpinistavtr.y > inicio.y && erro == 1)
        {
            alpinista.transform.position = alpinistavtr + Vector3.down/2;
        }
        else if(alpinistavtr.y > inicio.y && erro == 2)
        {
            alpinista.transform.position = alpinistavtr + Vector3.down/2 + Vector3.down/2;
        }
        else if (alpinistavtr.y > inicio.y && erro >= 3)
        {
            alpinistavtr = alpinistavtr - movimento;
            alpinista.transform.position = alpinistavtr;
            erro = 0;
        }
    }

}
