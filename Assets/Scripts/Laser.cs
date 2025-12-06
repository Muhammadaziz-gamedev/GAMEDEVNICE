using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] 
    private bool _isEnemyLaser;
    [SerializeField] 
    private bool _isUnusualLaser;
    [SerializeField] 
    private bool _playerBehind;
    [SerializeField] 
    private bool _isAlienBossLaser;
    [SerializeField] 
    private bool _isProjectileLaser;

    [SerializeField] 
    private float _speed = 8f;
    private GameObject _enemy;
    private Player _player;

    void Start()
    {
        _enemy = GameObject.FindWithTag("Enemy");
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (transform.root.CompareTag("Boss"))
        {
            _isAlienBossLaser = true;
            _isProjectileLaser = false;
            _isEnemyLaser = false;
            _isUnusualLaser = false;
        }
    }

    void Update()
    {
        if (_isProjectileLaser)
        {
            ProjectileMove();
            return;
        }
        if (_isAlienBossLaser)
        {
            AlienBossMove();
            return;
        }
        if (_isEnemyLaser)
        {
            MoveDown();
            return;
        }
        if (_isUnusualLaser || _playerBehind)
        {
            MoveUp();
            return;
        }
        MoveUp();
    }

    public bool IsEnemyLaser()
    {
        return _isEnemyLaser;
    }

    public bool IsUnusualLaser()
    {
        return _isUnusualLaser;
    }

    public bool isPlayerBehind()
    {
        return _playerBehind;
    }
    
    public bool IsAlienBossLaser()
    {
        return _isAlienBossLaser;
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y > 10f)
            Destroy(this.gameObject);
    }

    void AlienBossMove()
    {
        transform.position += transform.up * _speed * Time.deltaTime;
        if (transform.position.y < -10f || transform.position.x < -11f || transform.position.x > 11f)
            Destroy(gameObject);
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -10f)
            Destroy(this.gameObject);
    }

    void ProjectileMove()
    {
        if (_enemy == null)
            return;
        if (_enemy.transform.position == null)
        {
            Debug.Log("bruh position");
            return;
        }
        Vector3 direction = (_enemy.transform.position - transform.position).normalized;
        Quaternion look = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, 5f * Time.deltaTime);
        transform.Translate(direction * _speed * Time.deltaTime, Space.Self);
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
        _playerBehind = false;
        _isUnusualLaser = false;
    }

    public void AssignUnusualLaser()
    {
        _isUnusualLaser = true;
    }

    public void AssignPlayerBehind()
    {
        _isEnemyLaser = false;
        _isUnusualLaser = false;
        _playerBehind = true;
    }

    public void AssignProjectileLaser()
    {
        _isEnemyLaser = false;
        _isUnusualLaser = false;
        _playerBehind = false;
        _isProjectileLaser = true;
    }

    public void AssignAlienBossLaser()
    {
        _isUnusualLaser = false;
        _isProjectileLaser = false;
        _isEnemyLaser = false;
        _isAlienBossLaser = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isEnemyLaser && !_playerBehind && !_isUnusualLaser && !_isAlienBossLaser)
        {
            if (other.CompareTag("Boss"))
            {
                Alien_boss_enemy boss = other.GetComponent<Alien_boss_enemy>();
                if (boss != null)
                {
                    boss.Damage();
                    Destroy(this.gameObject);
                }
            }
        }
        if (_isAlienBossLaser && other.CompareTag("Boss"))
            return;
        if (_isEnemyLaser)
        {
            if (other.CompareTag("Player"))
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.Damage();
                }
                Destroy(this.gameObject);
            }
            if (other.CompareTag("PowerUp"))
            {
                Destroy(other.gameObject);
            }
        }
        if (_isAlienBossLaser)
        {
            if (other.CompareTag("Player"))
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.Damage();
                    player.SlowEffect();
                    Destroy(this.gameObject);
                }
            }
        }
    }
}

