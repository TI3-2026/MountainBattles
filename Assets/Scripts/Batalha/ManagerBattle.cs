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

    public UnityEvent onBatalhaTerminou = new UnityEvent();

    [Header("References")]
    public HUDBatalha hud;
    public GolemComportamento inimigo;
    public GolemComportamento player;

    [Header("Level Control")]
    public float positionToWin = 6f;
    public float finalMovement = 0f;
    private float Tempo = 0f;
    private bool batalhaTerminou = false;

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

        onBatalhaTerminou.AddListener(BatalhaTerminou);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) TentarSkillCheck();
        
        PerdaDeForcaPlayer();
        CalcularForcaInimigo();
    }

    private void FixedUpdate() {
        AplicarForcas();

        Tempo += Time.deltaTime;
        if(Tempo >= 0.05f)
        {
            hud.AtualizarRelacaoForcas(forcaPlayer, forcaInimigo);
            Tempo = 0f;
        }
    }
    
    // ====================== Operações de forças ================
    private void CalcularForcaPlayer(bool acertou) {
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
    
    private void PerdaDeForcaPlayer()
    {
        forcaPlayer -= forcaPlayerPerda * Time.deltaTime;
        if (forcaPlayer < forcaPlayerMin) forcaPlayer = forcaPlayerMin;
    }
    
    public void TentarSkillCheck()
    {
        CalcularForcaPlayer(hud.VerificarSkillCheck());
    }

    private void CalcularForcaInimigo() {
        forcaInimigo += Random.Range(forcaInimigoMinima, forcaInimigoMaxima)/10;

        if (forcaInimigo > forcaInimigoMaxima) forcaInimigo = forcaInimigoMaxima;
        if (forcaInimigo < forcaInimigoMinima) forcaInimigo = forcaInimigoMinima;
    }

    private void AplicarForcas() {
        finalMovement = forcaPlayer - forcaInimigo;

        if (!batalhaTerminou)
        {
            player.Velocity(finalMovement);
            inimigo.Velocity(finalMovement);
        }
        else
        {
            player.Velocity(0f);
            inimigo.Velocity(0f);
        }
    }

    // ====================== Controle de batalha ================
    
    private void BatalhaTerminou()
    {
        batalhaTerminou = true;
    }
}
