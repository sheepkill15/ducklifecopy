using UnityEngine;

public class Fail : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Spawner.Instance.GameOver();
        }
    }
}
