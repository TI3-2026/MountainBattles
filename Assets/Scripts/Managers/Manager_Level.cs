using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Manager_Level : MonoBehaviour
{
    public Alpinista alpinista;
    [Header("Prefabs")]
    [SerializeField] private GameObject prefab_MemoryBlock;

    [Header("Settings")]
    [SerializeField] private GameObject block_position_origin;

    // Generation Variables
    [SerializeField] private float block_row_offset = 2.25f;
    private int blocks_row = 3;
    [SerializeField] private float block_column_offset = 4.4f;
    private int blocks_column = 6;

    private int duos_blocks;
    private int[] block_values;

    // Level Control Variables
    public UnityEvent<bool> onCardsEnabled;

    public int first_block_value = -1;
    private Block_Behavior first_block;
    public int second_block_value = -1;
    private Block_Behavior second_block;
    public int Erro = 0;
    public int DeuMatch = 0;

    // ==================== Functions ====================

    private void Start()
    {
        Manager_Game.Instance.manager_level = this;

        GenerateLevel();
    }
    private void Update()
    {
        if(DeuMatch == 9 && alpinista.alpinistavtr.y > 20)
        {
            Debug.Log("Vit�ria!");
            SceneManager.LoadScene("Menu");
        }
        else if(DeuMatch == 9 && alpinista.alpinistavtr.y < 20)
        {
            Debug.Log("Derrota!");
            SceneManager.LoadScene("Menu");
        }
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

        /* For Debugging:

        string text = "";   
        for (int i = 0; i < block_values.Length; i++){
            text += block_values[i] + " ";
        }
        Debug.Log(text);
        */
        
        // Create blocks
        int block_index = 0;
        for (int i = 0; i < blocks_column; i++)
        {
            for (int j = 0; j < blocks_row; j++)
            {
                Vector3 block_position = new Vector3(i * block_column_offset, 0, j * block_row_offset);

                GameObject block = Instantiate(prefab_MemoryBlock, block_position_origin.transform);
                Block_Behavior block_behavior = block.GetComponent<Block_Behavior>();

                block.transform.localPosition = block_position;
                block_behavior.block_value = block_values[block_index];
                block_behavior.SetNumber(block_values[block_index]);
                block_index++;
            }
        }
    }

    public void BlockClicked(int value=-1, Block_Behavior block=null)
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
        if (first_block_value == second_block_value)
        {
            Debug.Log("Match!");
            DeuMatch++;
            Erro = 0;
            alpinista.AcertouCarta();
            StartCoroutine(WaitForCardsReset(true));
        }
        else
        {
            Debug.Log("UnMatch!");
            if(alpinista.alpinistavtr.y > alpinista.inicio.y)
            {
                Erro++;
            }
            alpinista.ErrouCarta(ref Erro);
            StartCoroutine(WaitForCardsReset(false));
        }
    }

    private IEnumerator WaitForCardsReset(bool match)
    {
        yield return new WaitForSeconds(1f);
        first_block.Match(match);
        second_block.Match(match);

        onCardsEnabled.Invoke(true);

        first_block_value = -1;
        second_block_value = -1;
    }
}
