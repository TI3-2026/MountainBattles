using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SimpleTouchPlayerNew : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float speed = 5f;
    
    [Header("Configurações de Drag (Toque/Mouse)")]
    public float deadZone = 20f;
    public float maxDragDistance = 100f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 startPointerPos;
    private bool isDragging = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Importante para não capotar em 3D
        rb.freezeRotation = true;
        
        // Garante que o player use detecção contínua para não atravessar paredes
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Update()
    {
        // 1. Resetamos o input a cada frame
        moveInput = Vector2.zero;

        // 2. Prioridade 1: Teclado (Movimento enquanto pressionado)
        Vector2 keyboardDir = GetKeyboardInput();
        
        if (keyboardDir != Vector2.zero)
        {
            // Se houver teclado, usamos ele e ignoramos o mouse/touch
            moveInput = ToFourDirections(keyboardDir);
            isDragging = false; // Cancela drag se usar teclado
        }
        else
        {
            // 3. Prioridade 2: Mouse/Touch
            HandlePointerInput();
        }
    }

    private void FixedUpdate()
    {
        // No 3D: moveInput.x é o eixo X, moveInput.y é o eixo Z
        // Mantemos a velocidade Y do Rigidbody para respeitar a gravidade
        Vector3 velocity = new Vector3(moveInput.x * speed, rb.linearVelocity.y, moveInput.y * speed);
        rb.linearVelocity = velocity;
    }

    private Vector2 GetKeyboardInput()
    {
        if (Keyboard.current == null) return Vector2.zero;

        Vector2 dir = Vector2.zero;

        // .isPressed retorna true APENAS enquanto a tecla está abaixada
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) dir.y += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) dir.y -= 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) dir.x -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) dir.x += 1f;

        return dir.normalized;
    }

    private void HandlePointerInput()
    {
        bool pointerDown = false;
        Vector2 currentPointerPos = Vector2.zero;

        // Verifica Touch ou Mouse
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            pointerDown = true;
            currentPointerPos = Touchscreen.current.primaryTouch.position.ReadValue();
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame) startPointerPos = currentPointerPos;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            pointerDown = true;
            currentPointerPos = Mouse.current.position.ReadValue();
            if (Mouse.current.leftButton.wasPressedThisFrame) startPointerPos = currentPointerPos;
        }

        if (pointerDown)
        {
            isDragging = true;
            Vector2 delta = currentPointerPos - startPointerPos;
            
            if (delta.magnitude > deadZone)
            {
                // Normaliza o arraste dentro do limite máximo
                Vector2 clampedDelta = Vector2.ClampMagnitude(delta, maxDragDistance);
                moveInput = ToFourDirections(clampedDelta.normalized);
            }
            else
            {
                moveInput = Vector2.zero;
            }
        }
        else
        {
            isDragging = false;
            moveInput = Vector2.zero;
        }
    }

    private Vector2 ToFourDirections(Vector2 input)
    {
        if (input == Vector2.zero) return Vector2.zero;

        // Compara qual eixo teve maior movimento para travar em 4 direções
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return new Vector2(Mathf.Sign(input.x), 0f);
        }
        else
        {
            return new Vector2(0f, Mathf.Sign(input.y));
        }
    }
}