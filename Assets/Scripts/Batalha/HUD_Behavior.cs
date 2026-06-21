using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = System.Random;

public class HUDBatalha : MonoBehaviour
{
    public Slider relacaoForcas;

    [Header("Skill Check")]
    public Slider skillCheck;
    public Image skillCheckErro;
    public Image skillCheckSucesso;
    public TextMeshProUGUI feedbackText;
    public float skillCheckVelocidade = 1f;
    private float skillCheckMax = 0.65f;
    private float skillCheckMin = 0.35f;

    private bool skillCheckDirecao = true; // true -> right, false -> left

    private void Start() {
        ManagerBattle.Instance.hud = this;
        
        feedbackText.gameObject.SetActive(false);
        
        skillCheckDirecao = true;
        skillCheck.value = 0f;
        AtualizarAreaAcerto();
    }

    private void Update()
    {
        ComportamentoSkillCheck();
    }

    public void AtualizarRelacaoForcas(float forcaPlayer, float forcaInimigo) {
        relacaoForcas.maxValue = forcaPlayer + forcaInimigo;
        relacaoForcas.value = forcaPlayer;
    }

    private void AtualizarAreaAcerto()
    {
        float tamErro = skillCheckErro.rectTransform.rect.width;
        float tamSucesso = skillCheckSucesso.rectTransform.rect.width;

        float proporcao = (tamSucesso /  tamErro);
        skillCheckMin = 0.5f - (proporcao / 2);
        skillCheckMax = 0.5f + (proporcao / 2);
    }

    public bool VerificarSkillCheck()
    {
        if (skillCheck.value <= skillCheckMax && skillCheck.value >= skillCheckMin) return true;
        return false;
    }

    private void ComportamentoSkillCheck()
    {
        if (skillCheckDirecao)
        {
            skillCheck.value += skillCheckVelocidade * Time.deltaTime;
            if (skillCheck.value >= 1f)
            {
                skillCheckDirecao = false;
                skillCheck.value = 1f;
            }
        }
        else
        {
            skillCheck.value -= skillCheckVelocidade * Time.deltaTime;
            if (skillCheck.value <= 0f)
            {
                skillCheckDirecao = true;
                skillCheck.value = 0f;
            }
        }
    }

    public void AtualizarFeedback(bool acertou)
    {
        float animationTime = 1f;
        
        LeanTween.cancel(feedbackText.gameObject);
        feedbackText.rectTransform.localScale = new Vector3(0, 0, 0);
        feedbackText.gameObject.SetActive(true);
        
        if (acertou)
        {
            feedbackText.text = "Acertou!!!";
            feedbackText.color = Color.green;
        }
        else
        {
            feedbackText.text = "Foi quase...";
            feedbackText.color = Color.red;
        }

        float rotationAngle = UnityEngine.Random.Range(-25f, 25f);
        LeanTween.rotate(feedbackText.gameObject, new Vector3(0, 0, rotationAngle), animationTime)
            .setEase(LeanTweenType.easeOutQuad);
        LeanTween.scale(feedbackText.gameObject, new Vector3(1f, 1f, 1f), animationTime)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                feedbackText.gameObject.SetActive(false);
            });
        
    }
    
}
