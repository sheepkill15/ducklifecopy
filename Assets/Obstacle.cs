using System;
using UnityEngine;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour
{
    public static float Speed = 3;
    private Transform _transform;
    public Text typeText;

    private void Start()
    {
        _transform = transform;
        typeText = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        _transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
        if (_transform.position.x < -10)
        {
            Spawner.Instance.ObstacleLeftTheScreen(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Spawner.Score++;
            Spawner.Instance.UpdateScore();
        }
    }
}
