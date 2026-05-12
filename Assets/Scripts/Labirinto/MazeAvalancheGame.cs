using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MazeAvalancheGame3D : MonoBehaviour
{
    [Header("Labirinto")]
    [Range(7, 25)] public int columns = 21;
    [Range(12, 40)] public int visibleRows = 24;
    [Range(10, 24)] public int rowsOnScreen = 14;
    public float cellSize = 1.2f;
    public float scrollSpeed = 2.2f;

    [Header("Geração")]
    [Range(2, 8)] public int maxActivePaths = 5;
    [Range(0f, 1f)] public float branchChance = 0.65f;
    [Range(0f, 1f)] public float extraOpenChance = 0.12f;
    [Range(0f, 1f)] public float closeWideAreaChance = 0.08f;

    [Header("Visual 3D")]
    public float wallHeight = 1.2f;
    public float floorThickness = 0.15f;
    public float playerHeight = 0.6f;
    public float playerWidthScale = 0.55f;

    [Header("Câmera")]
    public float cameraHeight = 30f;
    public float cameraPadding = 1.2f;
    public Color backgroundColor = new Color(0.78f, 0.87f, 0.96f);

    [Header("Avalanche")]
    public float avalancheDelay = 30f;
    public float avalancheSpeed = 2.8f;
    public float avalancheThickness = 2.2f;
    public float avalancheHeight = 1.5f;

    [Header("Jogo")]
    public float restartDelay = 1.2f;
    public float moveRepeatDelay = 0.12f;
    public float swipeMinDistance = 40f;

    [Header("Cores")]
    public Color wallColor = new Color(0.32f, 0.38f, 0.50f);
    public Color floorColor = new Color(0.70f, 0.74f, 0.82f);
    public Color playerColor = new Color(0.85f, 0.25f, 0.18f);
    public Color avalancheColor = new Color(0.96f, 0.97f, 1f);

    [Header("Materiais / Texturas (opcional)")]
    public Material wallMaterial;
    public Material floorMaterial;
    public Material playerMaterial;
    public Material avalancheMaterial;

    private List<int[]> rows = new List<int[]>();
    private List<int> activePaths = new List<int>();

    private GameObject[,] wallPool;
    private GameObject floorObject;
    private GameObject playerObject;
    private GameObject avalancheObject;

    private Camera cam;
    private Transform worldRoot;

    private Canvas canvas;
    private Text timerText;
    private Text scoreText;
    private Text gameOverText;

    private int playerCol;
    private int playerRow;

    private float scrollOffset;
    private float avalancheTimer;
    private float avalancheZ;
    private float moveCooldown;

    private int distanceScore;
    private bool avalancheStarted;
    private bool gameOver;
    private bool initialized;

    private bool dragging;
    private Vector2 dragStart;

    private void Start()
    {
        initialized = false;

        columns = MakeOdd(Mathf.Max(columns, 7));
        visibleRows = Mathf.Max(visibleRows, 12);
        rowsOnScreen = Mathf.Clamp(rowsOnScreen, 10, visibleRows);

        worldRoot = new GameObject("World").transform;
        worldRoot.SetParent(transform);

        SetupCamera();
        SetupLighting();
        SetupUI();
        SetupFloor();
        SetupWallPool();
        SetupPlayer();
        SetupAvalanche();

        RestartGame();

        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        if (moveCooldown > 0f)
            moveCooldown -= Time.deltaTime;

        if (gameOver)
            return;

        HandleInput();
        UpdateScrolling();
        UpdateAvalanche();
        UpdateVisuals();
        UpdateUI();
    }

    private void SetupCamera()
    {
        cam = Camera.main;

        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }

        float worldWidth = columns * cellSize;
        float screenLength = rowsOnScreen * cellSize;

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = backgroundColor;
        cam.orthographic = true;

        cam.transform.position = new Vector3(0f, cameraHeight, screenLength * 0.5f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        float sizeByWidth = (worldWidth * 0.5f) / cam.aspect + cameraPadding;
        float sizeByLength = (screenLength * 0.5f) + cameraPadding;

        cam.orthographicSize = Mathf.Max(sizeByWidth, sizeByLength);
    }

    private void SetupLighting()
    {
        Light[] existingLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        if (existingLights != null && existingLights.Length > 0)
            return;

        GameObject lightObj = new GameObject("Directional Light");
        Light dirLight = lightObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.intensity = 1.2f;
        dirLight.color = Color.white;
        lightObj.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
    }

    private void SetupUI()
    {
        GameObject canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        scoreText = CreateUIText(
            "ScoreText",
            font,
            28,
            TextAnchor.UpperLeft,
            new Vector2(20f, -18f),
            new Vector2(400f, 60f)
        );

        timerText = CreateUIText(
            "TimerText",
            font,
            28,
            TextAnchor.UpperRight,
            new Vector2(-20f, -18f),
            new Vector2(420f, 60f)
        );

        gameOverText = CreateUIText(
            "GameOverText",
            font,
            48,
            TextAnchor.MiddleCenter,
            Vector2.zero,
            new Vector2(800f, 120f)
        );

        gameOverText.text = "";
    }

    private Text CreateUIText(string objName, Font font, int size, TextAnchor anchor, Vector2 anchoredPos, Vector2 boxSize)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.SetParent(canvas.transform, false);

        Text txt = obj.AddComponent<Text>();
        txt.font = font;
        txt.fontSize = size;
        txt.alignment = anchor;
        txt.color = Color.black;
        txt.text = "";

        RectTransform rt = txt.rectTransform;

        if (anchor == TextAnchor.UpperLeft)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
        }
        else if (anchor == TextAnchor.UpperRight)
        {
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
        }
        else
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
        }

        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = boxSize;

        return txt;
    }

    private void SetupFloor()
    {
        floorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floorObject.name = "Floor";
        floorObject.transform.SetParent(worldRoot);

        float worldWidth = columns * cellSize;
        float worldLength = visibleRows * cellSize;

        floorObject.transform.position = new Vector3(0f, -floorThickness * 0.5f, worldLength * 0.5f);
        floorObject.transform.localScale = new Vector3(worldWidth, floorThickness, worldLength + cellSize * 4f);

        Collider col = floorObject.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        ApplyVisual(floorObject.GetComponent<Renderer>(), floorMaterial, floorColor);
    }

    private void SetupWallPool()
    {
        wallPool = new GameObject[visibleRows, columns];

        for (int r = 0; r < visibleRows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = "Wall_" + r + "_" + c;
                wall.transform.SetParent(worldRoot);
                wall.transform.localScale = new Vector3(cellSize, wallHeight, cellSize);

                ApplyVisual(wall.GetComponent<Renderer>(), wallMaterial, wallColor);

                wallPool[r, c] = wall;
            }
        }
    }

    private void SetupPlayer()
    {
        playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        playerObject.name = "Player";
        playerObject.transform.SetParent(worldRoot);
        playerObject.transform.localScale = new Vector3(cellSize * playerWidthScale, playerHeight, cellSize * playerWidthScale);

        Collider col = playerObject.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        ApplyVisual(playerObject.GetComponent<Renderer>(), playerMaterial, playerColor);
    }

    private void SetupAvalanche()
    {
        avalancheObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        avalancheObject.name = "Avalanche";
        avalancheObject.transform.SetParent(worldRoot);

        float worldWidth = columns * cellSize + cellSize * 2f;
        avalancheObject.transform.localScale = new Vector3(worldWidth, avalancheHeight, avalancheThickness);

        Collider col = avalancheObject.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        ApplyVisual(avalancheObject.GetComponent<Renderer>(), avalancheMaterial, avalancheColor);
        avalancheObject.SetActive(false);
    }

    private void ApplyVisual(Renderer rend, Material mat, Color fallbackColor)
    {
        if (rend == null)
            return;

        if (mat != null)
            rend.sharedMaterial = mat;
        else
            rend.material.color = fallbackColor;
    }

    private void RestartGame()
    {
        CancelInvoke();

        rows.Clear();
        activePaths.Clear();

        scrollOffset = 0f;
        avalancheTimer = avalancheDelay;
        avalancheStarted = false;
        avalancheZ = TopRowZ() + cellSize * 3f;
        distanceScore = 0;
        gameOver = false;
        dragging = false;
        moveCooldown = 0f;

        if (gameOverText != null)
            gameOverText.text = "";

        AddPathIfValid(activePaths, columns / 4, 2);
        AddPathIfValid(activePaths, columns / 2, 2);
        AddPathIfValid(activePaths, columns - 1 - (columns / 4), 2);

        if (columns >= 19)
            AddPathIfValid(activePaths, Random.Range(2, columns - 2), 2);

        if (activePaths.Count == 0)
            activePaths.Add(columns / 2);

        for (int i = 0; i < visibleRows; i++)
            rows.Add(GenerateRow());

        playerRow = visibleRows - 4;
        playerCol = FindOpenCellNearCenter(playerRow);

        UpdateVisuals();
        UpdateUI();
    }

    private int[] GenerateRow()
    {
        int[] row = new int[columns];

        for (int i = 0; i < columns; i++)
            row[i] = 1;

        row[0] = 1;
        row[columns - 1] = 1;

        if (activePaths == null || activePaths.Count == 0)
            activePaths = new List<int> { columns / 2 };

        List<int> currentPaths = new List<int>(activePaths);
        currentPaths.Sort();

        List<int> nextPaths = new List<int>();

        for (int i = 0; i < currentPaths.Count; i++)
        {
            int start = currentPaths[i];
            int next = Mathf.Clamp(start + Random.Range(-1, 2), 1, columns - 2);

            int tries = 0;
            while (IsTooCloseToOtherPaths(nextPaths, next, 2) && tries < 6)
            {
                next = Mathf.Clamp(start + Random.Range(-2, 3), 1, columns - 2);
                tries++;
            }

            AddPathIfValid(nextPaths, next, 2);

            int min = Mathf.Min(start, next);
            int max = Mathf.Max(start, next);

            for (int c = min; c <= max; c++)
                OpenCell(row, c);

            OpenCell(row, next);

            if (Random.value < 0.30f)
                OpenCell(row, next - 1);

            if (Random.value < 0.30f)
                OpenCell(row, next + 1);

            if (nextPaths.Count < maxActivePaths && Random.value < branchChance)
            {
                int dir = Random.value < 0.5f ? -1 : 1;
                int branch = Mathf.Clamp(next + dir * Random.Range(1, 3), 1, columns - 2);

                if (!IsTooCloseToOtherPaths(nextPaths, branch, 2))
                {
                    int bmin = Mathf.Min(next, branch);
                    int bmax = Mathf.Max(next, branch);

                    for (int c = bmin; c <= bmax; c++)
                        OpenCell(row, c);

                    AddPathIfValid(nextPaths, branch, 2);
                }
            }
        }

        nextPaths.Sort();

        for (int i = 0; i < nextPaths.Count - 1; i++)
        {
            int a = nextPaths[i];
            int b = nextPaths[i + 1];
            int gap = b - a;

            if (gap <= 4 && Random.value < 0.80f)
            {
                for (int c = a; c <= b; c++)
                    OpenCell(row, c);
            }
            else if (gap <= 6 && Random.value < 0.35f)
            {
                for (int c = a; c <= b; c++)
                    OpenCell(row, c);
            }
        }

        for (int c = 1; c < columns - 1; c++)
        {
            if (row[c] == 0)
                continue;

            bool touchesOpen = row[c - 1] == 0 || row[c + 1] == 0;

            if (touchesOpen && Random.value < extraOpenChance)
                row[c] = 0;
        }

        for (int c = 2; c < columns - 3; c++)
        {
            if (row[c - 1] == 0 && row[c] == 0 && row[c + 1] == 0 && row[c + 2] == 0)
            {
                if (Random.value < closeWideAreaChance)
                    row[c + 1] = 1;
            }
        }

        int openCount = 0;
        for (int c = 1; c < columns - 1; c++)
        {
            if (row[c] == 0)
                openCount++;
        }

        if (openCount < 3)
        {
            int emergencyA = Mathf.Clamp(columns / 2 - 2, 1, columns - 2);
            int emergencyB = Mathf.Clamp(columns / 2 + 2, 1, columns - 2);

            OpenCell(row, emergencyA);
            OpenCell(row, emergencyB);

            AddPathIfValid(nextPaths, emergencyA, 2);
            AddPathIfValid(nextPaths, emergencyB, 2);
        }

        if (nextPaths.Count == 0)
            nextPaths.Add(columns / 2);

        while (nextPaths.Count > maxActivePaths)
            nextPaths.RemoveAt(Random.Range(0, nextPaths.Count));

        activePaths = nextPaths;

        return row;
    }

    private void OpenCell(int[] row, int col)
    {
        if (col > 0 && col < columns - 1)
            row[col] = 0;
    }

    private bool IsTooCloseToOtherPaths(List<int> list, int x, int minDistance)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (Mathf.Abs(list[i] - x) < minDistance)
                return true;
        }

        return false;
    }

    private void AddPathIfValid(List<int> list, int x, int minDistance)
    {
        x = Mathf.Clamp(x, 1, columns - 2);

        if (!IsTooCloseToOtherPaths(list, x, minDistance))
            list.Add(x);
    }

    private int FindOpenCellNearCenter(int rowIndex)
    {
        if (rows == null || rows.Count == 0)
            return columns / 2;

        rowIndex = Mathf.Clamp(rowIndex, 0, rows.Count - 1);

        int center = columns / 2;

        for (int offset = 0; offset < columns; offset++)
        {
            int left = center - offset;
            int right = center + offset;

            if (left >= 1 && left <= columns - 2 && rows[rowIndex][left] == 0)
                return left;

            if (right >= 1 && right <= columns - 2 && rows[rowIndex][right] == 0)
                return right;
        }

        return center;
    }

    private void HandleInput()
    {
        if (moveCooldown > 0f)
            return;

        if (HandleKeyboard())
            return;

        HandleTouchAndMouse();
    }

    private bool HandleKeyboard()
    {
        if (Keyboard.current == null)
            return false;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            return TryMove(-1, 0);

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            return TryMove(1, 0);

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            return TryMove(0, -1);

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            return TryMove(0, 1);

        return false;
    }

    private void HandleTouchAndMouse()
    {
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                dragging = true;
                dragStart = touch.position.ReadValue();
            }

            if (dragging && touch.press.wasReleasedThisFrame)
            {
                dragging = false;
                ProcessSwipe(touch.position.ReadValue());
            }

            if (!touch.press.isPressed)
                dragging = false;

            return;
        }

        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                dragging = true;
                dragStart = Mouse.current.position.ReadValue();
            }

            if (dragging && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                dragging = false;
                ProcessSwipe(Mouse.current.position.ReadValue());
            }
        }
    }

    private void ProcessSwipe(Vector2 endPos)
    {
        Vector2 delta = endPos - dragStart;

        if (delta.magnitude < swipeMinDistance)
            return;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0f)
                TryMove(1, 0);
            else
                TryMove(-1, 0);
        }
        else
        {
            if (delta.y > 0f)
                TryMove(0, -1);
            else
                TryMove(0, 1);
        }
    }

    private bool TryMove(int dx, int dy)
    {
        int nextCol = playerCol + dx;
        int nextRow = playerRow + dy;

        if (nextCol < 1 || nextCol > columns - 2)
            return false;

        if (nextRow < 0 || nextRow >= visibleRows)
            return false;

        if (rows[nextRow][nextCol] == 1)
            return false;

        playerCol = nextCol;
        playerRow = nextRow;
        moveCooldown = moveRepeatDelay;
        return true;
    }

    private void UpdateScrolling()
    {
        scrollOffset += scrollSpeed * Time.deltaTime;

        while (scrollOffset >= cellSize)
        {
            scrollOffset -= cellSize;

            if (rows.Count > 0)
                rows.RemoveAt(0);

            rows.Add(GenerateRow());

            playerRow -= 1;
            distanceScore += 1;

            if (playerRow < 0)
            {
                Lose();
                return;
            }
        }
    }

    private void UpdateAvalanche()
    {
        if (!avalancheStarted)
        {
            avalancheTimer -= Time.deltaTime;

            if (avalancheTimer <= 0f)
            {
                avalancheTimer = 0f;
                avalancheStarted = true;
                avalancheObject.SetActive(true);
            }
        }
        else
        {
            avalancheZ -= avalancheSpeed * Time.deltaTime;

            float playerZ = RowToZ(playerRow);
            float avalancheBottomEdge = avalancheZ - avalancheThickness * 0.5f;

            if (playerZ >= avalancheBottomEdge)
                Lose();
        }
    }

    private void Lose()
    {
        if (gameOver)
            return;

        gameOver = true;

        if (gameOverText != null)
            gameOverText.text = "GAME OVER";

        Invoke(nameof(RestartGame), restartDelay);
    }

    private void UpdateVisuals()
    {
        if (rows == null || rows.Count < visibleRows || wallPool == null || playerObject == null || avalancheObject == null)
            return;

        for (int r = 0; r < visibleRows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                bool isWall = rows[r][c] == 1;
                GameObject wall = wallPool[r, c];

                wall.SetActive(isWall);

                if (isWall)
                {
                    wall.transform.position = new Vector3(
                        ColToX(c),
                        wallHeight * 0.5f,
                        RowToZ(r)
                    );
                }
            }
        }

        playerObject.transform.position = new Vector3(
            ColToX(playerCol),
            playerHeight * 0.5f,
            RowToZ(playerRow)
        );

        avalancheObject.transform.position = new Vector3(
            0f,
            avalancheHeight * 0.5f,
            avalancheZ
        );

        avalancheObject.SetActive(avalancheStarted);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Distância: " + distanceScore;

        if (timerText != null)
        {
            if (!avalancheStarted)
                timerText.text = "Avalanche em: " + Mathf.CeilToInt(avalancheTimer) + "s";
            else
                timerText.text = "AVALANCHE!";
        }
    }

    private float ColToX(int col)
    {
        return (col - (columns - 1) * 0.5f) * cellSize;
    }

    private float RowToZ(int row)
    {
        return (visibleRows - 1 - row) * cellSize + scrollOffset;
    }

    private float TopRowZ()
    {
        return (visibleRows - 1) * cellSize + scrollOffset;
    }

    private int MakeOdd(int value)
    {
        return value % 2 == 0 ? value + 1 : value;
    }
}