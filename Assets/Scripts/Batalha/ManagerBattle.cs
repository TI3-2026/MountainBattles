using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ManagerBattle : MonoBehaviour
{
    // Singleton
    public static ManagerBattle Instance { get; private set; }
    private void Awake() 
    {         
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this;
    }

    [Header("References")]
    public HUDBatalha hud;
    public GolemComportamento inimigo;
    public GolemComportamento player;

    [Header("Level Control")]
    public float positionToWin = 6f;
    public float finalMovement = 0f;
    public float Tempo = 0f;

    [Header("Configurações Player")]
    public float forcaPlayerGanho = 0f;
    public float forcaPlayerPerda = 0f;
    public float forcaPlayerMax = 20f;
    public float forcaPlayerMin = 0f;
    public float forcaPlayer = 0f;

    [Header("Configurações Inimigo")]
    public float forcaInimigoMinima = 1f;
    public float forcaInimigoMaxima = 3f;
    public float forcaInimigo = 0f;

    private void Start() {
        forcaInimigo = (forcaInimigoMinima + forcaInimigoMaxima) / 2;
        forcaPlayer = (forcaPlayerMin + forcaPlayerMax) / 2;
    }

    private void Update() {
        CalcularForcaPlayer();
        CalcularForcaInimigo();
    }

    private void FixedUpdate() {
        ApplyStrengths();

        Tempo += Time.deltaTime;
        if(Tempo >= 0.05f)
        {
            hud.AtualizarRelacaoForcas(forcaPlayer, forcaInimigo);
            Tempo = 0f;
        }
    }

    private void CalcularForcaPlayer() {
        (bool skillCheck, bool acertou) = DetectarSkillCheck();

        if (skillCheck)
        {
            if (acertou)
            {
                // Acertou Skill Check
                forcaPlayer += forcaPlayerGanho;
                if (forcaPlayer > forcaPlayerMax) forcaPlayer = forcaPlayerMax;
            }
            else
            {
                // Penalidade (Errar SkillCheck)
                forcaPlayer -= forcaPlayerGanho / 2;
            }
        }

        forcaPlayer -= forcaPlayerPerda * Time.deltaTime;
        if (forcaPlayer < forcaPlayerMin) forcaPlayer = forcaPlayerMin;
    }

    private (bool, bool) DetectarSkillCheck()
    {
        bool tentouSkillCheck = Input.GetKeyDown(KeyCode.Space);

        // Detectar Touch
        if (!tentouSkillCheck && Input.touchCount > 0) {
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    tentouSkillCheck = true;
                    break;
                }
            }
        }

        if (tentouSkillCheck) return (tentouSkillCheck, hud.VerificarSkillCheck());
        return (tentouSkillCheck, false);
    }

    private void CalcularForcaInimigo() {
        forcaInimigo += Random.Range(forcaInimigoMinima, forcaInimigoMaxima)/10;

        if (forcaInimigo > forcaInimigoMaxima) forcaInimigo = forcaInimigoMaxima;
        if (forcaInimigo < forcaInimigoMinima) forcaInimigo = forcaInimigoMinima;
    }

    private void ApplyStrengths() {
        finalMovement = forcaPlayer - forcaInimigo;

        player.Velocity(finalMovement);
        inimigo.Velocity(finalMovement);
    }
}
