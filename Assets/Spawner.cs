using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject scoreObject;

    public Text scoreText;
    
    public Transform floor;

    private readonly Tuple<Transform, Transform>[] _objectPool = new Tuple<Transform, Transform>[10];
    private readonly Transform[] _scores = new Transform[10];
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
        for (var i = 0; i < _objectPool.Length; i++)
        {
            _objectPool[i] =
                new Tuple<Transform, Transform>(Instantiate(prefab, _transform).transform, Instantiate(prefab, _transform).transform);
            _objectPool[i].Item1.gameObject.SetActive(false);
            _objectPool[i].Item2.gameObject.SetActive(false);
            _scores[i] = Instantiate(scoreObject, _transform).transform;
            _scores[i].gameObject.SetActive(false);
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
        var countSpawned = _objectPool.Count(obj => obj.Item1.gameObject.activeInHierarchy);
        if (countSpawned > atOnce) return;
        for (var i = 0; i < _objectPool.Length && countSpawned <= atOnce; i++)
        {
            if (_objectPool[i].Item1.gameObject.activeInHierarchy) continue;
            var variety = Random.Range(0, Jump.Instance.jumpHeights.Length);
            Respawn(_objectPool[i], variety);
            countSpawned++;
        }
    }
    
    private void Respawn(Tuple<Transform, Transform> obj, int variety)
    {
        var lastPosition = _objectPool.OrderBy(o => o.Item1.position.x).LastOrDefault()?.Item1
                               .position
                           ?? _transform.position;
        var height = Jump.Instance.jumpHeights[variety] - 1;
        var position = floor.position;
        lastPosition.y = position.y + 0.5f + height * 0.5f;
        var (item1, item2) = obj;
        item1.position = lastPosition + new Vector3(10f * _distance / atOnce, 0, 0);
        item1.localScale = new Vector3(1, height, 1); 
        item1.gameObject.SetActive(true);
        var newHeight = 50;
        lastPosition.y += newHeight * 0.5f;
        item2.position = lastPosition + new Vector3(10f * _distance / atOnce, height * 0.5f + 2.5f, 0);
        item2.localScale = new Vector3(1, newHeight, 1); 
        item2.gameObject.SetActive(true);

        var scoreObj = _scores.First(score => !score.gameObject.activeInHierarchy);

        scoreObj.position = lastPosition + new Vector3(10f * _distance / atOnce, -newHeight*0.5f + 1.25f, 0);
        scoreObj.gameObject.SetActive(true);
    }

    public void UpdateScore()
    {
        scoreText.text = $"Score: {Score}";
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
    }

    public void Restart()
    {
        gameOverScreen.SetActive(false);
        Score = 0;
        UpdateScore();
        Jump.Instance.Reset();
        Time.timeScale = 1;
        foreach (var (item1, item2) in _objectPool)
        {
            item1.gameObject.SetActive(false);
            item2.gameObject.SetActive(false);
        }

        foreach (var score in _scores)
        {
            score.gameObject.SetActive(false);
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
