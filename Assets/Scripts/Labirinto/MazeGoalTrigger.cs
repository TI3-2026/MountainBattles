using UnityEngine;

public class MazeGoalTrigger : MonoBehaviour
{
    public MazeGameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Won");
            manager.PlayerWon();
        }
    }
}