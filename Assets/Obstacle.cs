using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static float Speed = 3;
    private Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        _transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
        if (_transform.position.x < -10)
        {
            Spawner.Instance.ObstacleLeftTheScreen(gameObject);
        }
    }
}
