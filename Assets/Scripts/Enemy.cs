using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour  
{
    [SerializeField] private float _speed = 3f;
    private Player _player;
    private Animator _anim;
    [SerializeField] private GameObject _explosionAnim;
    [SerializeField] private AudioClip _destroyedClip;
    [SerializeField] private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFire = 1.0f;
    private float _amptitude = 7;
    private AudioSource _audioSource;
    private bool _isDead;
    private bool _isShieldActive;
    [SerializeField] private GameObject _enemyShield;
    private float _waveOffset;
    private float _ramDistance = 5f;
    private float _normalSpeed = 3f;
    private bool _isRamming = false;
    private float _runDistance = 3f;
    private float _runSpeed= 5f;
    private bool _isRunning = false;
    private float _canFirePowerUpLaser = 0f;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _waveOffset = UnityEngine.Random.Range(0f, 10f);
    }

    void Update()
    {
        if (transform.position.y < -6f)
        {
            NotifyDestroyed();
            Destroy(gameObject);
            return;
        }
        if (_player == null)
            return;
        if(_isRunning == true)
        {
            running();
        }
        if(_isRamming == true)
        {
            ramming();
        }
        Move();
        Shoot();
    }
    void Shoot()
    {
        if (Time.time > _canFire && _isDead == false)
        {
            _fireRate = UnityEngine.Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser laserScript = enemyLaser.GetComponent<Laser>();
            if (transform.position.y > _player.transform.position.y)
                laserScript.assignenemylaser();
            else
                laserScript.assignplayerbehind();
        }
    }
    void Move()
    {
        if (_isRamming && _player != null)
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            transform.position += direction * _speed * Time.deltaTime;
        }
        float x = Mathf.Sin((Time.time + _waveOffset) * _speed) * _amptitude;
        transform.position = new Vector3(x, transform.position.y - _speed * Time.deltaTime, 0);
        if (transform.position.y < -6f)
        {
            Destroy(this.gameObject);
            NotifyDestroyed();
        }
    PowerUp[] powerUps = FindObjectsOfType<PowerUp>();
        foreach (PowerUp powerup in powerUps)
        {
            if (Mathf.Abs(transform.position.x - powerup.transform.position.x) < 0.5f &&
            powerup.transform.position.y < transform.position.y)
                {
                    if (_canFirePowerUpLaser <= Time.time)
                    {
                        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                        Laser laserScript = enemyLaser.GetComponent<Laser>();
                        laserScript.assignenemylaser();
                        _canFirePowerUpLaser = Time.time + 1f; // 1 second cooldown so it doesn't spam
                    }
                }
        }
    }

    public void setshieldtrue()
    {
        _isShieldActive = true;
        _enemyShield.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isShieldActive)
            {
                _isShieldActive = false;
                _enemyShield.SetActive(false);
                return;
            }
            else
            {
                _player.Damage();
                PlayDestroyedSound();
                _anim.SetTrigger("OnEnemyDeath");
                _speed = 0f;
                NotifyDestroyed();
                Destroy(gameObject, 0.99f);
                _isDead = true;
            }
        }
        else if (other.CompareTag("Laser"))
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser == null)
            return;
            if (laser._isenemylaser)
            return;
            if (laser._playerBehind)
            return;
            if (_isShieldActive)
            {
                _isShieldActive = false;
                _enemyShield.SetActive(false);
                return;
            }
            Destroy(other.gameObject);
            if (_player != null)
                _player.AddScore(10);
            _anim.SetTrigger("OnEnemyDeath");
            PlayDestroyedSound();
            NotifyDestroyed();
            Destroy(GetComponent<Collider2D>());
            _speed = 0f;
            Destroy(gameObject, 0.99f);
            _isDead = true;
        }
        if (other.CompareTag("Boss"))
        {
            return;
        }
    }

    private void PlayDestroyedSound()
    {
        if (_audioSource != null && _destroyedClip != null)
            _audioSource.PlayOneShot(_destroyedClip);
    }

    void NotifyDestroyed()
    {
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager != null)
            spawnManager.OnEnemyDestroyed();
    }

    void  ramming()
    {   
        float distancetoplayer = Vector3.Distance(transform.position, _player.transform.position);
        if(distancetoplayer <= _ramDistance)
            _isRamming = true;
        else
        {
            _speed = _normalSpeed;
        }
    }

    void running()
    {
    Laser[] lasers = FindObjectsOfType<Laser>();
    foreach (Laser laser in lasers)
    {
        if (!laser._isenemylaser)
        {
            float distance = Vector3.Distance(transform.position, laser.transform.position);
            if (distance <= _runDistance)
            {
                _speed = _runSpeed;
                return;
            }
        }
    _speed = _normalSpeed;
    }
    }

    public void isrunningactive()
    {
        _isRunning = true;
    }

    public void isrammingactive()
    {
        _isRamming = true;
    }
}