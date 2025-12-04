using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public bool _isenemylaser;
    public bool _isUnusualLaser;
    public bool _playerBehind;
    public bool _isAlienBossLaser;
    public bool _isProjectileLaser;
    [SerializeField] private float _speed = 8f;
    private GameObject _enemy;
    private Player _player;

    void Start()
    {
    _enemy = GameObject.FindWithTag("Enemy");
    _player  = GameObject.FindWithTag("Player").GetComponent<Player>();
        if(transform.root.CompareTag("Boss"))
        {
            _isAlienBossLaser = true;
            _isProjectileLaser=false;
            _isenemylaser = false;
            _isUnusualLaser =false;
        }
    }

    void Update()
    {
        if (_isProjectileLaser)
        {
            projectilemove();
            return;
        }
        if (_isAlienBossLaser)
        {
            alienbossmove();
            return;
        }
        if (_isenemylaser)
        {
            Movedown();
            return;
        }
        if (_isUnusualLaser || _playerBehind)
        {
            Moveup();
            return;
        }
    Moveup();
    }

    void Moveup()
    {
    transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y > 10f)
            Destroy(this.gameObject);
    }

    void alienbossmove()
    {
    transform.position += transform.up * _speed * Time.deltaTime;
        if(transform.position.y < -10f || transform.position.x < -11f || transform.position.x > 11f)
            Destroy(gameObject);
    }

    void Movedown()
    {
    transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -10f)
            Destroy(this.gameObject);
    }

    void projectilemove()
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

    public void assignenemylaser()
    {
        _isenemylaser = true;
        _playerBehind = false;
        _isUnusualLaser = false;
    }

    public void assignunusuallaser()
    {
        _isUnusualLaser = true;
    }

    public void assignplayerbehind()
    {
        _isenemylaser = false;
        _isUnusualLaser = false;
        _playerBehind = true;
    }

    public void assignprojectilelaser()
    {
        _isenemylaser = false;
        _isUnusualLaser = false;
        _playerBehind = false;
        _isProjectileLaser = true;
    }

    public void assignalienbosslaser()
    {
        _isUnusualLaser = false;
        _isProjectileLaser = false;
        _isenemylaser = false;
        _isAlienBossLaser = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
    if (!_isenemylaser && !_playerBehind && !_isUnusualLaser && !_isAlienBossLaser)
        {
            if(other.CompareTag("Boss"))
            {
                Alien_boss_enemy boss = other.GetComponent<Alien_boss_enemy>();
                if(boss != null)
                {
                    boss.Damage();
                    Destroy(this.gameObject);
                }
            }
        }
    if(_isAlienBossLaser && other.CompareTag("Boss"))
        return;
    if(_isenemylaser)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if(player != null) player.Damage();
            Destroy(this.gameObject);
        }
        if(other.CompareTag("PowerUp"))
        {
            Destroy(other.gameObject);
        }
    }
        if(_isAlienBossLaser)
        {
            if(other.CompareTag("Player"))
            {
            Player player = other.GetComponent<Player>();
                if(player != null)
                    {
                    player.Damage();
                    player._isslow =true;
                    player.sloweffect();
                    Destroy(this.gameObject);
                    }
            }
        }
    }
}

