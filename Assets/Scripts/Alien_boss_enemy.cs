using System.Security;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Xml.Serialization;

public class Alien_boss_enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5;
    private float _lives = 5f;
    [SerializeField]
    public AudioClip _explosion;
    private float _canFire = 3.0f;
    private float _fireRate = 1.0f;
    public AudioSource _iaudioSource;
    private bool _isdead;
    [SerializeField]
    private GameObject _alienBossLaser;
    public bool _isalienBossLaser;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _alienFire;
    private SpawnManager spawnManager;
    [SerializeField]
    private GameObject _chaser;
    private float _shotCount = 0;
    void Start()
    {
        Player player = GetComponent<Player>();
        _iaudioSource = GetComponent<AudioSource>();
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    void Update()
    {
        Movement();
        shoot();
    }

    void Movement()
    {
        Vector3 move = new Vector3(0, -_speed * Time.deltaTime, 0); // only Y axis
        transform.position += move; // absolute world movement
        if (transform.position.y <= 0)
        _speed = 0;
    }

    void shoot()
    {
    if (Time.time <= _canFire || _isdead) return;
        _fireRate = Random.Range(3f, 5f);
        _canFire = Time.time + _fireRate;
        GameObject laserPrefab;
    if (_shotCount % 2 == 0)
        laserPrefab = _alienBossLaser;
    else
        laserPrefab = _chaser;
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        foreach (var l in laser.GetComponentsInChildren<Laser>())
        l.assignalienbosslaser();
        _shotCount++;
    }

    public void Damage()
    {
        _lives--;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Laser laser = other.GetComponent<Laser>();
    if(other.CompareTag("Laser") && laser != null && !laser._isenemylaser && !laser._isAlienBossLaser)
    {
        Destroy(other.gameObject);
        _lives--;
        _iaudioSource.PlayOneShot(_explosion);
        Debug.Log("boss is damaged from player laser");
    }

    if(other.CompareTag("Boss") && _isalienBossLaser == true)
        {
            return;
        }
    if(other.tag == ("Player"))
        {
            Player player = other.GetComponent<Player>();
            _lives--;
            player.Damage();
            _iaudioSource.PlayOneShot(_explosion);
            Debug.Log("boss got damage by player");
        }
    if(_lives <= 0)
        {
            Destroy(gameObject);
            _isdead = true;
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            spawnManager.onbossDestroyed();
        }
    }
}