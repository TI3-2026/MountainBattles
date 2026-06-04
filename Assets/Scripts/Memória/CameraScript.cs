using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Referencias")]
    public Transform pos_montanha;
    public Transform pos_player;


    [Header("Variaveis")]
    public float animacao_duracao = 1f;

    private void Start() {
        Manager_Level.Instance.camScript = this;
        
        gameObject.transform.position = pos_montanha.position;
    }

    // Mostrar toda a cena para mostrar toda a montanha
    public void MostrarMontanha() {
        LeanTween.move(gameObject, pos_montanha.position, animacao_duracao)
        .setEase(LeanTweenType.easeInOutSine);
    }

    // Mostrar o player novamente (posicao padrao)
    public void MostrarPlayer() {
        LeanTween.move(gameObject, pos_player.position, animacao_duracao)
        .setEase(LeanTweenType.easeInOutSine);
    }
}
