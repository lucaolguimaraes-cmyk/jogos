using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public VictoryScreen victoryScreen;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            victoryScreen.ShowVictory();
        }
    }
}
