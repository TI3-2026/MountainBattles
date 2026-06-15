using UnityEngine;

public class CartoesPivotScript : MonoBehaviour
{
    [Header("Configurações")]
    public float tempoTransicao = 0.5f;

    private void Awake() {
        ManagerLevel.Instance.cartoesPivot = this;
    }

    private void Start() {
        transform.localRotation = Quaternion.Euler(-90, 0, 0); // Rotação inicial para sumir as cartas
    }

    public void DesaparecerCartas() 
    {
        LeanTween.rotateLocal(gameObject, new Vector3(-90, 0, 0), tempoTransicao).setEaseOutQuad();
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), tempoTransicao).setEaseOutQuad();
    }

    public void AparecerCartas()
    {
        LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 0), tempoTransicao).setEaseOutQuad();
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), tempoTransicao).setEaseOutQuad();
    }
}
