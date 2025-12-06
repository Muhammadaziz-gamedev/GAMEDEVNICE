using UnityEngine;

public class Enemyboss : MonoBehaviour
{
    [SerializeField] 
    private float _speed = 0.5f, _amplitude = 1f;
    [SerializeField] 
    private int _lives = 1;
    [SerializeField] 
    private GameObject _bossFirePrefab;
    private float _nextFireTime;
    [SerializeField] 
    private Transform firePoint;

    void Start()
    {
        _nextFireTime = Time.time + Random.Range(2f, 3f);
    }

    void Update()
    {
        Move();
        Shoot();
    }

    void Move()
    {
        float x = Mathf.Sin(Time.time * _speed) * _amplitude;
        transform.position = new Vector3(x, transform.position.y, 0);
    }

    void Shoot()
    {
        if (Time.time >= _nextFireTime)
        {
            Instantiate(_bossFirePrefab, firePoint.position, Quaternion.identity);
            _nextFireTime = Time.time + Random.Range(5f, 7f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            Damage();
        }
        else if (other.CompareTag("Laser"))
        {
            Damage();
            Destroy(other.gameObject);
        }
    }

    void Damage()
    {
        _lives--;
        if (_lives <= 0) Destroy(gameObject);
    }
}
