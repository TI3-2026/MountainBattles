using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class Manager_Level : MonoBehaviour
{
    //Singleton
    public static Manager_Level Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    [Header("References")]
    public Alpinista alpinista;
    public GameObject canvas;
    public TextMeshProUGUI finaljogo;

    [Header("Auto Preenchivel")]
    public List<CartoesScript> cartoes_list;
    public CameraScript camScript;
    
    [Header("Prefabs")]
    public GameObject prefab_MemoryBlock;

    [Header("Settings")]
    public Transform cartoes_pivot;

    // Generation Variables
    public float block_row_offset = 2.25f;
    private int blocks_row = 2;
    public float block_column_offset = 4.4f;
    private int blocks_column = 7;

    private int duos_blocks;
    private int[] block_values;
    public string[] InfoMontanhas = { "VINSON – É a montanha mais fria entre os Sete Cumes.",
    "DENALI – O Denali é geologicamente descrito como um enorme bloco de granito.",
    "ACONCÁGUA – O Aconcágua faz parte da Cordilheira dos Andes.",
    "EVEREST – O Everest cresce cerca de 4 milímetros por ano.",
    "ELBRUS – O Elbrus é um vulcão adormecido.",
    "KILIMANJARO – Por conta de sua neve, “Kilimanjaro” significa “Montanha Branca”.",
    "PIRÂMIDE DE CARSTENSZ – Sua escalada exige técnicas de rapel e escalada em rocha."};
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

        GenerateLevel();
    }

    private void GenerateLevel()
    {
        duos_blocks = blocks_column * blocks_row / 2;
        block_values = new int[blocks_column * blocks_row];
        
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
        for (int i = 0; i < blocks_column; i++)
        {
            for (int j = 0; j < blocks_row; j++)
            {
                Vector3 block_position = new Vector3(i * block_column_offset, 0, j * block_row_offset);

                GameObject block = Instantiate(prefab_MemoryBlock, cartoes_pivot);
                CartoesScript block_behavior = block.GetComponent<CartoesScript>();

                block.transform.localPosition = block_position;
                int value = block_values[block_index];
                block_behavior.block_value = value;
                block_behavior.SetCard(Montanhas[value], InfoMontanhas[value]);
                block_index++;
                cartoes_list.Add(block_behavior);
            }
        }
        StartCoroutine(ShowAllThenHide());
    }

    public void BlockClicked(int value=-1, CartoesScript block=null)
    {
        // Error Handling
        if (block == null) return;

        // Block clicked
        if (first_block_value == -1)
        {
            first_block_value = value;
            first_block = block;
        }
        else if (second_block_value == -1)
        {
            second_block_value = value;
            second_block = block;

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
        first_block.Match(match);
        second_block.Match(match);

        if (match) {
            Vector3 pivot_position = cartoes_pivot.localPosition;

            yield return new WaitForSeconds(0.5f);

            cartoes_pivot.localPosition = new Vector3(100, 100, 100); // Sumir com o objeto da tela
            yield return new WaitForSeconds(0.25f);
            camScript.MostrarPlayer();
            yield return new WaitForSeconds(camScript.animacao_duracao+0.25f);

            alpinista.AcertouCarta();
            yield return new WaitForSeconds(alpinista.movimento_duracao+0.25f);

            camScript.MostrarMontanha();
            yield return new WaitForSeconds(camScript.animacao_duracao+0.25f);
            cartoes_pivot.localPosition = pivot_position;
        }
        
        yield return new WaitForSeconds(1.2f);
        onCardsEnabled.Invoke(true);

        first_block_value = -1;
        second_block_value = -1;
    }

    private IEnumerator ShowAllThenHide()
    {
        onCardsEnabled.Invoke(false);
        yield return new WaitForSeconds(5f);
        foreach (var cartao in cartoes_list)
        {
            cartao.Flip(false);
        }
        yield return new WaitForSeconds(0.3f); 
        onCardsEnabled.Invoke(true);
    }

    private IEnumerator AcabarJogo()
    {
        yield return new WaitForSeconds(1f);
        canvas.SetActive(true);
        if (DeuMatch == 7 && alpinista.alpinistavtr.y > 20)
        {
            finaljogo.SetText("Vitória!");
            finaljogo.color = Color.green;
            yield return new WaitForSeconds(4f);
            SceneManager.LoadScene("Menu");
        }
        else if (DeuMatch == 7 && alpinista.alpinistavtr.y < 20)
        {
            finaljogo.SetText("Derrota!");
            finaljogo.color = Color.red;
            yield return new WaitForSeconds(4f);
            SceneManager.LoadScene("Menu");
        }
    }
}
