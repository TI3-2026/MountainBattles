using UnityEngine;

public class Player_Behavior : MonoBehaviour
{    
    private Rigidbody rb;
    private Manager_Battle manager_battle;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        manager_battle = Manager_Battle.Instance;
    }

    public void Velocity(float velocity) {
        if (Mathf.Abs(transform.position.x) >= manager_battle.positionToWin) {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }
        rb.linearVelocity = new Vector3(velocity, rb.linearVelocity.y, 0);
    }
}