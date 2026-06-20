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
        rb.linearVelocity = new Vector3(velocity, rb.linearVelocity.y, 0);
    }
}