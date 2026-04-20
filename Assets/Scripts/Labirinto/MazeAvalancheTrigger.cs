using UnityEngine;

public class MazeAvalancheTrigger : MonoBehaviour
{
    public MazeGameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Failed");
            manager.PlayerFailed();
        }
    }
}