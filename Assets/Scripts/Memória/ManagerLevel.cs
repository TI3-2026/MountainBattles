using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ManagerLevel : MonoBehaviour
{
    //Singleton
    public static ManagerLevel Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }


    [Header("Geração")]
    public float espacamentoHorizontal = 2.25f;
    public float espacamentoVertical = 4.4f;
    public int linhas = 2;
    public int colunas = 7;

    [Header("Configurações de Jogo")]
    public float tempoMostraInicial = 5f;

    [Header("Referencias")]
    public GameObject prefab_carta;
    public Transform carta_pivot;
    public Alpinista alpinista;
    public GameObject canvas;
    public TextMeshProUGUI finaljogo;

    [Header("Auto Preenchível")]
    public List<CartoesScript> cartoes_list;
    public CameraScript camScript;
    
    [Header("Prefab")]
    



    private int duos_blocks;
    private int[] block_values;
    public string[] InfoMontanhas = 
    {
    "VINSON – É a montanha mais fria entre os Sete Cumes.",
    "DENALI – O Denali é geologicamente descrito como um enorme bloco de granito.",
    "ACONCÁGUA – O Aconcágua faz parte da Cordilheira dos Andes.",
    "EVEREST – O Everest cresce cerca de 4 milímetros por ano.",
    "ELBRUS – O Elbrus é um vulcão adormecido.",
    "KILIMANJARO – Por conta de sua neve, “Kilimanjaro” significa “Montanha Branca”.",
    "PIRÂMIDE DE CARSTENSZ – Sua escalada exige técnicas de rapel e escalada em rocha."
    };
    public Sprite[] Montanhas = new Sprite[7];

    // Level Control Variables
    public UnityEvent<bool> onCardsEnabled;

    public int first_block_value = -1;
    private CartoesScript first_block;
    public int second_block_value = -1;
    private CartoesScript second_block;
    public int Erro = 0;
    public int DeuMatch = 0;

    // ==================== Functions ====================

    private void Start()
    {
        canvas.SetActive(false);
        GerarNivel();
    }

    private void GerarNivel()
    {
        duos_blocks = colunas * linhas / 2;
        block_values = new int[colunas * linhas];
        
        // Generate block values
        for (int i = 0; i < block_values.Length/2; i++){
            block_values[i*2] = i;
            block_values[i*2 + 1] = i;
        }
        // Shuffle block values
        for (int i = 0; i < block_values.Length; i++){
            int random_index = Random.Range(0, block_values.Length);
            int temp = block_values[i];
            block_values[i] = block_values[random_index];
            block_values[random_index] = temp;
        }
        
        // Create blocks
        int block_index = 0;
        for (int i = 0; i < colunas; i++)
        {
            for (int j = 0; j < linhas; j++)
            {
                Vector3 posicaoCarta = new Vector3(i * espacamentoHorizontal, 0, j * espacamentoVertical);

                GameObject carta = Instantiate(prefab_carta, carta_pivot);
                carta.name = $"Carta_{block_index}";
                CartoesScript cartaScript = carta.GetComponent<CartoesScript>();

                carta.transform.localPosition = posicaoCarta;
                int value = block_values[block_index];
                cartaScript.block_value = value;
                cartaScript.SetCard(Montanhas[value], InfoMontanhas[value]);
                block_index++;
                cartoes_list.Add(cartaScript);
            }
        }
        StartCoroutine(I_MostraInicial());
    }

    private IEnumerator I_MostraInicial()
    {
        onCardsEnabled.Invoke(false);
        yield return new WaitForSeconds(tempoMostraInicial);
        foreach (var cartao in cartoes_list)
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
        if (first_block_value == -1)
        {
            first_block_value = value;
            first_block = cartao;
        }
        else if (second_block_value == -1)
        {
            second_block_value = value;
            second_block = cartao;

            onCardsEnabled.Invoke(false);
            CheckMatch();
        }
    }

    private void CheckMatch()
    {
        //Match
        if (first_block_value == second_block_value)
        {
            DeuMatch++;
            Erro = 0;
            StartCoroutine(I_CartoesAnimacao(match: true));
            if(DeuMatch == 7)
            {
                StartCoroutine(AcabarJogo());
            }
        }
        else //UnMatch
        {
            if(alpinista.alpinistavtr.y > alpinista.inicio.y) Erro++;

            alpinista.ErrouCarta(ref Erro);
            StartCoroutine(I_CartoesAnimacao(match: false));
        }
    }

    private IEnumerator I_CartoesAnimacao(bool match)
    {
        yield return new WaitForSeconds(1f);

        if (match) {
            Vector3 pivot_position = carta_pivot.localPosition;

            yield return new WaitForSeconds(0.5f);

            carta_pivot.localPosition = new Vector3(100, 100, 100); // Sumir com o objeto da tela
            yield return new WaitForSeconds(0.25f);
            camScript.MostrarPlayer();
            yield return new WaitForSeconds(camScript.animacao_duracao+0.25f);

            alpinista.AcertouCarta();
            yield return new WaitForSeconds(alpinista.movimento_duracao+0.25f);

            camScript.MostrarMontanha();
            yield return new WaitForSeconds(camScript.animacao_duracao+0.25f);
            carta_pivot.localPosition = pivot_position;
        }else
        {
            first_block.ErrouMatch();
            second_block.ErrouMatch();
        }
        
        yield return new WaitForSeconds(1.2f);
        onCardsEnabled.Invoke(true);

        first_block_value = -1;
        second_block_value = -1;
    }

    private IEnumerator AcabarJogo()
    {
        yield return new WaitForSeconds(1f);
        canvas.SetActive(true);
        if (DeuMatch == 7)
        {
            finaljogo.SetText("Vitória!");
            finaljogo.color = Color.green;
            yield return new WaitForSeconds(4f);
            SceneManager.LoadScene("Menu");
        }
        else
        {
            finaljogo.SetText("Derrota!");
            finaljogo.color = Color.red;
            yield return new WaitForSeconds(4f);
            SceneManager.LoadScene("Menu");
        }
    }
}
