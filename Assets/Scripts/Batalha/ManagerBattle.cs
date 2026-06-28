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
    public Animator Golem;
    public Animator MountainMan;

    [Header("Controle de Jogo")]
    public float positionToWin = 6f;
    public float finalMovement = 0f;
    private float Tempo = 0f;
    private bool batalhaTerminou = false;
    public float skillChecksAcertadas = 0;
    public float skillChecksErradas = 0;
    private float ultimoSkillCheck = 0f;

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
        Golem.SetInteger("Instance", 1);
        MountainMan.SetInteger("Instance", 1);
    }
    
    // ====================== Operações de forças ================
    private void CalcularForcaPlayer(bool acertou) {
        hud.AtualizarFeedback(acertou);
        
        if (acertou)
        {
            // Acertou Skill Check
            skillChecksAcertadas++;
            forcaPlayer += forcaPlayerGanho;
            if (forcaPlayer > forcaPlayerMax) forcaPlayer = forcaPlayerMax;
            Golem.SetInteger("Instance", 2);
            MountainMan.SetInteger("Instance", 3);
        }
        else
        {
            // Penalidade (Errar SkillCheck)
            skillChecksErradas++;
            forcaPlayer -= forcaPlayerGanho / 2;
            Golem.SetInteger("Instance", 3);
            MountainMan.SetInteger("Instance", 2);
        }
    }
    
    private void PerdaDeForcaPlayer()
    {
        forcaPlayer -= forcaPlayerPerda * Time.deltaTime;
        if (forcaPlayer < forcaPlayerMin) forcaPlayer = forcaPlayerMin;
    }
    
    public void TentarSkillCheck()
    {
        bool acertou = hud.VerificarSkillCheck();


        if (Time.time - ultimoSkillCheck > 1f) 
        {
            CalcularForcaPlayer(acertou);
            ultimoSkillCheck = Time.time;
        }
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
        EnviarDados();
        batalhaTerminou = true;
    }

    public void EnviarDados()
    {
        GoogleFormsAnalytics.Instance.SendForm(
            resJogouBatalha: true,
            resSkillChecksAcertadas: skillChecksAcertadas,
            resSkillChecksErradas: skillChecksErradas
        );
    }
    
}
