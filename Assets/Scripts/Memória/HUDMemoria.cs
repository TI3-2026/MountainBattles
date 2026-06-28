using UnityEngine;
using TMPro;

public class HUDMemoria : MonoBehaviour
{
    public TextMeshProUGUI textoFinal;
    public TextMeshProUGUI tempoRestante;

    private void Start()
    {
        ManagerMemoria.Instance.hud = this;
    }

    public void ExibirTextoFinal(string texto, Color cor)
    {
        textoFinal.text = texto;
        textoFinal.color = cor;
        textoFinal.gameObject.SetActive(true);

        tempoRestante.gameObject.SetActive(false);
    }


    public void AtualizarTempoRestante(float tempo)
    {
        tempoRestante.text = $"{tempo.ToString("F0")}";
    }
}
