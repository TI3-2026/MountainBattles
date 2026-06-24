using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class ManagerMemoria : MonoBehaviour
{
    //Singleton
    public static ManagerMemoria Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    
    [Header("Eventos")]
    public UnityEvent<bool> onCardsEnabled;

    [Header("Geração")]
    public float espacamentoHorizontal = 2.25f;
    public float espacamentoVertical = 4.4f;
    public int linhas = 2;
    public int colunas = 7;

    [Header("Configurações de Jogo")]
    public float tempoMostraInicial = 5f;
    public float tempoMostraAcerto = 2f;
    public float tempoMostraErro = 4f;
    public int DeuMatch = 0;
    
    public float cartasAcertadas = 0;
    public float cartasErradas = 0;

    [Header("Referencias")]
    public GameObject prefab_carta;
    public Alpinista alpinista;
    public GameObject canvas;
    public TextMeshProUGUI finaljogo;

    [Header("Info das montanhas")]
    private string[] InfoMontanhas = 
    {
    "VINSON\nÉ a montanha mais fria entre os Sete Cumes.",
    "DENALI\nO Denali é geologicamente descrito como um enorme bloco de granito.",
    "ACONCÁGUA\nO Aconcágua faz parte da Cordilheira dos Andes.",
    "EVEREST\nEverest cresce cerca de 4 milímetros por ano.",
    "ELBRUS\nO Elbrus é um vulcão adormecido.",
    "KILIMANJARO\nPor conta de sua neve, “Kilimanjaro” significa “Montanha Branca”.",
    "PIRÂMIDE DE CARSTENSZ\nSua escalada exige técnicas de rapel e escalada em rocha."
    };
    public Sprite[] Montanhas = new Sprite[7];

    [Header("Outras variáveis/referências. Não modificar.")]
    public List<CartoesScript> cartoesList;
    public CartoesAncoraScript cartoesAncora;
    public CartoesPivotScript cartoesPivot;
    private CartoesScript primeiraCarta;
    private CartoesScript segundaCarta;
    private int duplas;
    private int[] duplasValores;
    public int primeiraCartaValor = -1;
    public int segundaCartaValor = -1;
    public int Erro = 0;
    
    // ==================== Functions ====================

    private void Start()
    {
        finaljogo.text = "";
        GerarNivel();
    }

    private void GerarNivel()
    {
        duplas = colunas * linhas / 2;
        duplasValores = new int[colunas * linhas];
        
        // Generate block values
        for (int i = 0; i < duplasValores.Length/2; i++){
            duplasValores[i*2] = i;
            duplasValores[i*2 + 1] = i;
        }
        // Shuffle block values
        for (int i = 0; i < duplasValores.Length; i++){
            int random_index = Random.Range(0, duplasValores.Length);
            (duplasValores[random_index], duplasValores[i]) = (duplasValores[i], duplasValores[random_index]);
        }

        // Create blocks
        int duplaIndex = 0;
        for (int i = 0; i < colunas; i++)
        {
            for (int j = 0; j < linhas; j++)
            {
                Vector3 posicaoCarta = new Vector3(i * espacamentoHorizontal, j * espacamentoVertical, 0);

                GameObject carta = Instantiate(prefab_carta, cartoesAncora.transform);
                carta.name = $"Carta_{duplaIndex}";
                carta.transform.localPosition = posicaoCarta;

                CartoesScript cartaScript = carta.GetComponent<CartoesScript>();
                int value = duplasValores[duplaIndex];
                cartaScript.duplaValor = value;
                cartaScript.DefinirCarta(Montanhas[value], InfoMontanhas[value]);
                duplaIndex++;
                cartoesList.Add(cartaScript);
            }
        }
        StartCoroutine(I_MostraInicial());
    }
    private IEnumerator I_MostraInicial()
    {
        onCardsEnabled.Invoke(false);
        cartoesPivot.AparecerCartas();

        yield return new WaitForSeconds(tempoMostraInicial);
        foreach (var cartao in cartoesList)
        {
            cartao.Flip(false);
        }
        yield return new WaitForSeconds(0.3f); 
        onCardsEnabled.Invoke(true);
    }

    public void CartaClicada(int value=-1, CartoesScript cartao=null)
    {
        // Tratamento de erro
        if (cartao == null) return;

        // Cartao clicado
        if (primeiraCartaValor == -1)
        {
            primeiraCartaValor = value;
            primeiraCarta = cartao;
        }
        else if (segundaCartaValor == -1)
        {
            segundaCartaValor = value;
            segundaCarta = cartao;

            onCardsEnabled.Invoke(false);
            CheckMatch();
        }
    }
   
    private void CheckMatch()
    {
        //Match
        if (primeiraCartaValor == segundaCartaValor)
        {
            cartasAcertadas++;
            DeuMatch++;
            Erro = 0;
            StartCoroutine(I_CartoesAnimacao(match: true, acabarJogo: DeuMatch == 7));
        }
        else //UnMatch
        {
            cartasErradas++;
            if(alpinista.alpinistavtr.y > alpinista.inicio.y) Erro++;

            alpinista.ErrouCarta(ref Erro);
            StartCoroutine(I_CartoesAnimacao(match: false));
        }
    }
    private IEnumerator I_CartoesAnimacao(bool match, bool acabarJogo = false)
    {
        yield return new WaitForSeconds(0.5f);

        if (match) {
            primeiraCarta.DefinirMaterialMatch();
            segundaCarta.DefinirMaterialMatch();
            yield return new WaitForSeconds(tempoMostraAcerto);

            cartoesPivot.DesaparecerCartas();
            yield return new WaitForSeconds(cartoesPivot.tempoTransicao);

            alpinista.AcertouCarta();
            yield return new WaitForSeconds(alpinista.movimento_duracao);

            if (acabarJogo)
            {
                AcabarJogo();
                yield return new WaitForSeconds(4f);
                SceneManager.LoadScene("Menu");
            }


            cartoesPivot.AparecerCartas();
            primeiraCarta.DesabilitarCarta();
            segundaCarta.DesabilitarCarta();
        }else
        {
            primeiraCarta.ErrouMatch(tempoMostraErro);
            segundaCarta.ErrouMatch(tempoMostraErro);
        }

        yield return new WaitForSeconds(1.2f);
        onCardsEnabled.Invoke(true);

        primeiraCartaValor = -1;
        segundaCartaValor = -1;
    }

    private void AcabarJogo()
    {
        EnviarDados();
        canvas.SetActive(true);
        if (DeuMatch == 7)
        {
            finaljogo.SetText("Vitória!");
            finaljogo.color = Color.green;
        }
        else
        {
            finaljogo.SetText("Derrota!");
            finaljogo.color = Color.red;
        }
    }

    public void EnviarDados()
    {
        GoogleFormsAnalytics.Instance.SendForm(
            resJogouMemoria: true,
            resCartasAcertadas: cartasAcertadas,
            resCartasErradas: cartasErradas
        );
    }
}
