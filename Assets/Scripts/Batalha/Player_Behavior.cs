using UnityEngine;

public class GolemComportamento : MonoBehaviour
{    
    private Rigidbody rb;
    private ManagerBattle manager_battle;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        manager_battle = ManagerBattle.Instance;
    }

    public void Velocity(float velocity) {
        if (Mathf.Abs(transform.position.x) >= manager_battle.positionToWin) {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }
        rb.linearVelocity = new Vector3(velocity, rb.linearVelocity.y, 0);
    }
}