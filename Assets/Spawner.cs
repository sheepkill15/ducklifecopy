using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;

    public Text scoreText;
    public Text highScoreText;
    
    public Transform floor;

    private readonly Transform[] _obstacles = new Transform[10];
    
    
    private Transform _transform;

    public int atOnce = 3;

    public static int Score;
    
    public static Spawner Instance;

    public GameObject gameOverScreen;

    private float _distance = Obstacle.Speed;

    private void Awake()
    {
        Instance = this;
        _transform = transform;
        for (var i = 0; i < _obstacles.Length; i++)
        {
            _obstacles[i] = Instantiate(prefab, _transform).transform;
            _obstacles[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        CheckObstacles();
    }

    private void Update()
    {
        Obstacle.Speed += 0.05f * Time.deltaTime;
        _distance += 0.025f * Time.deltaTime;
    }

    private void CheckObstacles()
    {
        var countSpawned = _obstacles.Count(obj => obj.gameObject.activeInHierarchy);
        if (countSpawned > atOnce) return;
        for (var i = 0; i < _obstacles.Length && countSpawned <= atOnce; i++)
        {
            if (_obstacles[i].gameObject.activeInHierarchy) continue;
            var variety = Random.Range(0, Jump.Instance.jumpHeights.Length);
            Respawn(_obstacles[i], variety);
            countSpawned++;
        }
    }
    
    private void Respawn(Transform obj, int variety)
    {
        var lastPosition = _obstacles.OrderBy(o => o.position.x).LastOrDefault()?
                               .position
                           ?? _transform.position;
        var height = Jump.Instance.jumpHeights[variety] - 1;
        var position = floor.position;
        // lastPosition.y = position.y + 0.5f + height * 0.5f;
        lastPosition.y = position.y + height + 1.75f;
        lastPosition.x += 10f * _distance / atOnce;
        obj.gameObject.GetComponent<Obstacle>().typeText.text = (variety + 1).ToString();
        obj.gameObject.SetActive(true);
        obj.position = lastPosition;
    }

    public void UpdateScore()
    {
        scoreText.text = $"PONT: {Score}";
    }

    public void ObstacleLeftTheScreen(GameObject obst)
    {
        obst.SetActive(false);
        CheckObstacles();
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;

        int highscore = PlayerPrefs.GetInt("legtobb_pont", 0);
        if (Score > highscore)
        {
            PlayerPrefs.SetInt("legtobb_pont", Score);
            highscore = Score;
        }

        highScoreText.text = $"Eddigi legmagasabb pontszám: {highscore}";
    }

    public void Restart()
    {
        gameOverScreen.SetActive(false);
        Score = 0;
        UpdateScore();
        Jump.Instance.Reset();
        Time.timeScale = 1;
        foreach (var item in _obstacles)
        {
            item.gameObject.SetActive(false);
        }
        Obstacle.Speed = 3;
        _distance = Obstacle.Speed;
        
        CheckObstacles();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
