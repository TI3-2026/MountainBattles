using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MazeGameManager : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public TMP_Text timerTMP;
    public Text timerLegacy;

    [Header("Configurações 3D")]
    public float wallHeight = 2.5f;
    public float floorThickness = 0.1f;

    [Header("Labirinto")]
    [Range(9, 101)] public int mazeWidth = 21;
    [Range(9, 101)] public int mazeHeight = 21;
    public float cellSize = 1f;
    public Vector3 mazeOffset = Vector3.zero;

    [Header("Tempo")]
    public float timeLimit = 30f;

    [Header("Avalanche (Perigo)")]
    public float avalancheSpeed = 2f;
    public float avalancheSizeZ = 5f; // Espessura da "parede" de perigo

    [Header("Materiais/Cores")]
    public Material wallMaterial;
    public Material floorMaterial;
    public Material goalMaterial;
    public Material avalancheMaterial;

    private int[,] maze;
    private Transform mazeRoot;
    private GameObject avalancheObject;
    private bool avalancheStarted;
    private bool resetting;
    private float timeLeft;

    private Vector2Int startCell = new Vector2Int(1, 1);
    private Vector2Int goalCell;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        mazeWidth = MakeOdd(Mathf.Max(mazeWidth, 9));
        mazeHeight = MakeOdd(Mathf.Max(mazeHeight, 9));

        GenerateNewMaze();
    }

    private void Update()
    {
        if (resetting) return;

        if (!avalancheStarted)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                StartAvalanche();
            }
        }
        else if (avalancheObject != null)
        {
            // Move a avalanche para frente (eixo Z positivo)
            avalancheObject.transform.position += Vector3.forward * avalancheSpeed * Time.deltaTime;
        }

        UpdateTimerUI();
    }

    public void GenerateNewMaze()
    {
        resetting = false;
        avalancheStarted = false;
        timeLeft = timeLimit;

        if (mazeRoot != null) Destroy(mazeRoot.gameObject);
        if (avalancheObject != null) Destroy(avalancheObject);

        mazeRoot = new GameObject("MazeRoot").transform;
        mazeRoot.SetParent(transform);

        CreateMazeData();
        CarveMaze();
        ChooseGoalCell();
        BuildMazeVisual();
        ResetPlayerPosition();
        UpdateTimerUI();
    }

    private void CreateMazeData()
    {
        maze = new int[mazeWidth, mazeHeight];
        for (int x = 0; x < mazeWidth; x++)
            for (int y = 0; y < mazeHeight; y++)
                maze[x, y] = 1; // 1 = Parede
    }

    private void CarveMaze()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        maze[startCell.x, startCell.y] = 0;
        stack.Push(startCell);

        Vector2Int[] directions = { new Vector2Int(0, 2), new Vector2Int(0, -2), new Vector2Int(2, 0), new Vector2Int(-2, 0) };

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = new List<Vector2Int>();

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (next.x > 0 && next.x < mazeWidth - 1 && next.y > 0 && next.y < mazeHeight - 1 && maze[next.x, next.y] == 1)
                    neighbors.Add(next);
            }

            if (neighbors.Count > 0)
            {
                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
                maze[(current.x + chosen.x) / 2, (current.y + chosen.y) / 2] = 0;
                maze[chosen.x, chosen.y] = 0;
                stack.Push(chosen);
            }
            else stack.Pop();
        }
    }

    private void ChooseGoalCell()
    {
        int bestDistance = -1;
        for (int x = 1; x < mazeWidth - 1; x++)
        {
            for (int y = 1; y < mazeHeight - 1; y++)
            {
                if (maze[x, y] == 0)
                {
                    int dist = Mathf.Abs(x - startCell.x) + Mathf.Abs(y - startCell.y);
                    if (dist > bestDistance) { bestDistance = dist; goalCell = new Vector2Int(x, y); }
                }
            }
        }
    }

    private void BuildMazeVisual()
    {
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeHeight; z++)
            {
                if (maze[x, z] == 1)
                {
                    // REDUÇÃO DE ESCALA (0.99f): Evita que o player "tranque" nas emendas das paredes
                    Vector3 wallScale = new Vector3(cellSize * 0.99f, wallHeight, cellSize * 0.99f);
                    CreateCube($"Wall_{x}_{z}", x, wallHeight / 2f, z, wallScale, wallMaterial, true);
                }
                else
                {
                    CreateCube($"Floor_{x}_{z}", x, -floorThickness / 2f, z, new Vector3(cellSize, floorThickness, cellSize), floorMaterial, true);
                }
            }
        }

        // Objetivo com material Unlit (Brilhante) para não ter sombras
        GameObject goal = CreateCube("Goal", goalCell.x, 0.5f, goalCell.y, new Vector3(cellSize * 0.6f, 1f, cellSize * 0.6f), goalMaterial, false);
        Renderer goalRender = goal.GetComponent<Renderer>();
        goalRender.material = new Material(Shader.Find("Unlit/Color"));
        goalRender.material.color = Color.green;

        Collider collider = goal.GetComponent<Collider>();
        collider.isTrigger = true;
        collider.enabled = true;
        goal.AddComponent<MazeGoalTrigger>().manager = this;
    }

    private GameObject CreateCube(string name, float x, float y, float z, Vector3 scale, Material mat, bool solid)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(mazeRoot);
        cube.transform.position = CellToWorld(x, y, z);
        cube.transform.localScale = scale;
        
        if (mat != null) cube.GetComponent<Renderer>().material = mat;
        if (!solid) cube.GetComponent<Collider>().enabled = false; // Chão não precisa de colisor se o player tiver gravidade num plano grande

        // Se for o chão, ativamos o colisor para o player não cair
        if (name.Contains("Floor")) cube.GetComponent<Collider>().enabled = true;

        return cube;
    }

    private void StartAvalanche()
    {
        avalancheStarted = true;
        avalancheObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        avalancheObject.name = "Avalanche";
        
        float totalWidth = mazeWidth * cellSize;
        
        // ALTURA DA AVALANCHE: Definimos uma altura grande (ex: 5) para garantir que pegue o player
        float avalancheHeight = 5f; 
        avalancheObject.transform.localScale = new Vector3(totalWidth, avalancheHeight, avalancheSizeZ);
        
        float centerX = mazeOffset.x + (mazeWidth * cellSize / 2f) - (cellSize / 2f);
        float startZ = mazeOffset.z - (avalancheSizeZ / 2f) - cellSize;
        
        // POSIÇÃO Y: Colocamos a base da avalanche no nível do chão
        avalancheObject.transform.position = new Vector3(centerX, avalancheHeight / 2f - 0.1f, startZ);
        
        if (avalancheMaterial != null) avalancheObject.GetComponent<Renderer>().material = avalancheMaterial;
        
        // Configura o Trigger
        Collider col = avalancheObject.GetComponent<Collider>();
        col.isTrigger = true;
        avalancheObject.AddComponent<MazeAvalancheTrigger>().manager = this;
    }

    private void ResetPlayerPosition()
    {
        if (player == null) return;
        player.position = CellToWorld(startCell.x, 0.5f, startCell.y); // 0.5f para ficar sobre o chão

        if (player.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void UpdateTimerUI()
    {
        string text = avalancheStarted ? "PERIGO!" : Mathf.CeilToInt(timeLeft) + "s";
        if (timerTMP != null) timerTMP.text = text;
        if (timerLegacy != null) timerLegacy.text = text;
    }

    private Vector3 CellToWorld(float x, float y, float z)
    {
        return new Vector3(mazeOffset.x + x * cellSize, y, mazeOffset.z + z * cellSize);
    }

    private int MakeOdd(int value) => value % 2 == 0 ? value + 1 : value;

    // Métodos chamados pelos Triggers
    public void PlayerFailed() { if (!resetting) { resetting = true; GenerateNewMaze(); } }
    public void PlayerWon() { if (!resetting) { resetting = true; GenerateNewMaze(); } }
}