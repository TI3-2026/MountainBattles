using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Manager_Battle : MonoBehaviour
{
    // Singleton
    public static Manager_Battle Instance { get; private set; }
    private void Awake() 
    {         
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this;
    }

    [Header("References")]
    public HUD_Behavior hud;
    public Player_Behavior enemy;
    public Player_Behavior player;

    [Header("Level Control")]
    public float positionToWin = 6f;
    public float finalMovement = 0f;
    public float Tempo = 0f;

    [Header("Player Variables")]
    public float playerStrength = 0;
    public float playerStrengthBurst = 0f;
    public float playerStrengthLoose = 0f;
    public float playerStrengthMax = 20f;
    public float playerStrengthMin = 0f;

    [Header("Enemy Variables")]
    public float enemyStrength = 0;
    public float enemyStrengthBase = 0f;
    public float enemyStrengthVariation = 0;

    private void Update() {
        CalculatePlayerStrength();
        CalculateEnemyStrength();
    }

    private void FixedUpdate() {
        ApplyStrengths();
        Tempo += Time.deltaTime;
        if(Tempo >= 0.05f)
        {
            hud.UpdateSlider(playerStrength - enemyStrength);
            Tempo = 0f;
        }
    }

    private void CalculatePlayerStrength() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (hud.CheckSkillCheck())
            {
                // Acertou Skill Check
                playerStrength += playerStrengthBurst;
                if (playerStrength > playerStrengthMax) playerStrength = playerStrengthMax;
            }
            else
            {
                // Penalidade (Errar SkillCheck)
                playerStrength -= playerStrengthBurst / 2;
            }
        }

        playerStrength -= playerStrengthLoose * Time.deltaTime;
        if (playerStrength < playerStrengthMin) playerStrength = playerStrengthMin;
    }

    private void CalculateEnemyStrength() {
        enemyStrength = Random.Range(
            enemyStrengthBase - enemyStrengthVariation,
            enemyStrengthBase + enemyStrengthVariation
        );
    }

    private void ApplyStrengths() {
        finalMovement = playerStrength - enemyStrength;
        //hud.UpdateSlider(playerStrength - enemyStrength);

        player.Velocity(finalMovement);
        enemy.Velocity(finalMovement);
    }
}
