using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager I;
    public PlayerInputAction action;
    
    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(I.gameObject);

        action = new PlayerInputAction();
        action.Enable();
    }

    private void OnEnable()
    {
         action.Enable();
    }
    private void OnDisable()
    {
        action.Disable();
    }
    private void OnDestroy()
    {
        action.Disable();
    }
}