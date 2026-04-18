using UnityEngine;

public class Manager_Game : MonoBehaviour
{
    public static Manager_Game Instance { get; private set; }
    private void Awake() 
    {         
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }


    [Header("References")]
    public Manager_Level manager_level;
}