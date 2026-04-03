using UnityEngine;

public class Manager_Level : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject prefab_MemoryBlock;

    [Header("Settings")]
    [SerializeField] private GameObject block_position_origin;
    [SerializeField] private float block_offset;
    [SerializeField] private int blocks_row;
    [SerializeField] private int blocks_column;

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        for (int i = 0; i < blocks_row; i++)
        {
            for (int j = 0; j < blocks_column; j++)
            {
                Vector3 block_position = new Vector3(i * block_offset, 0, j * block_offset);

                GameObject block = Instantiate(prefab_MemoryBlock, block_position_origin.transform);
                block.transform.localPosition = block_position;
            }
        }
    }
}
