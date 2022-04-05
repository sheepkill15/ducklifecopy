using System;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public float[] jumpHeights;

    private Rigidbody2D _rb;

    private bool _onGround;

    public static Jump Instance;

    private Transform _transform;

    private Vector3 _startPos;
    
    private void Awake()
    {
        _transform = transform;
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _startPos = _transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_transform.position.y < -5)
        {
            Spawner.Instance.GameOver();
        }
        if (!_onGround) return;
        for (var i = 0; i < jumpHeights.Length; i++)
        {
            if (!Input.GetKeyDown(KeyCode.Alpha1 + i)) continue;
            JumpTo(jumpHeights[i]);
            return;
        }
    }

    public void Reset()
    {
        _transform.position = _startPos;
        _rb.velocity = Vector2.zero;
    }

    private void JumpTo(float height)
    {
        var jumpForce = Mathf.Sqrt(2 * height * Mathf.Abs(Physics2D.gravity.y));
        _rb.velocity = new Vector2(0, jumpForce);
        _onGround = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Map"))
        {
            _onGround = true;
        }
        // else if (other.gameObject.CompareTag("Obstacle"))
        // {
        //     Spawner.Instance.GameOver();
        // }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Score")) return;
        Spawner.Score++;
        Spawner.Instance.UpdateScore();
    }
}
