using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Referencias")]
    public Transform pos_montanha;
    public Transform pos_player;


    [Header("Variaveis")]
    public float animacao_duracao = 1f;

    private bool playerFocado = false;

    private void Start() {
        Manager_Level.Instance.camScript = this;
        
        gameObject.transform.position = pos_montanha.position;
    }

    // Mostrar toda a cena para mostrar toda a montanha
    public void MostrarMontanha() {
        LeanTween.move(gameObject, pos_montanha.position, animacao_duracao)
        .setEase(LeanTweenType.easeInOutSine);
        playerFocado = false;
    }

    // Mostrar o player novamente (posicao padrao)
    public void MostrarPlayer() {
        LeanTween.move(gameObject, pos_player.position, animacao_duracao)
        .setEase(LeanTweenType.easeInOutSine);
        playerFocado = true;
        StartCoroutine(I_AcompanharPlayer());
    }

    private IEnumerator I_AcompanharPlayer() {
        yield return new WaitForSeconds(animacao_duracao);
        while (playerFocado) {
            transform.position = pos_player.position;
            yield return null;
        }
    }
}
