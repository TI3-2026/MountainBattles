using Unity.VisualScripting;
using UnityEngine;

public class Manager_Level : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject prefab_MemoryBlock;

    [Header("Settings")]
    [SerializeField] private GameObject block_position_origin;

    // Generation Variables
    private float block_row_offset = 3.4f;
    private int blocks_row = 3;
    private float block_column_offset = 1.7f;
    private int blocks_column = 6;

    private int duos_blocks;
    private int[] block_values;

    // Level Control Variables

    private int first_block_value;
    private int second_block_value;

    // ==================== Functions ====================

    private void Start()
    {
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
                block_index++;
            }
        }
    }

    public void BlockClicked() {
        
    }
}
