using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDBatalha : MonoBehaviour
{
    public Slider relacaoForcas;

    [Header("Skill Check")]
    public Slider skillCheck;
    public Image skillCheckErro;
    public Image skillCheckSucesso;
    public float skillCheckVelocidade = 1f;
    private float skillCheckMax = 0.65f;
    private float skillCheckMin = 0.35f;

    private bool skillCheckDirecao = true; // true -> right, false -> left

    private void Start() {
        ManagerBattle.Instance.hud = this;
        
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
    
}
