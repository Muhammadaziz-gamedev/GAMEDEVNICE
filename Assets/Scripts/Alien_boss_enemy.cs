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
    private AudioClip explosion;
    private float _canFire = 3.0f;
    private float _fireRate = 1.0f;
    private AudioSource iaudioSource;
    private bool _isDead;
    [SerializeField]
    private GameObject alienBossLaser;
    private bool isAlienBossLaser;
    [SerializeField]
    private GameObject explosionPrefab;
    private SpawnManager spawnManager;
    [SerializeField]
    private GameObject chaser;
    private float shotCount = 0;
    void Start()
    {
        Player player = GetComponent<Player>();
        iaudioSource = GetComponent<AudioSource>();
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    void Update()
    {
        Movement();
        Shoot();
    }

    void Movement()
    {
        Vector3 move = new Vector3(0, -_speed * Time.deltaTime, 0); // only Y axis
        transform.position += move; // absolute world movement
        if (transform.position.y <= 0)
            _speed = 0;
    }

    void Shoot()
    {
        if (Time.time <= _canFire || _isDead) return;
        _fireRate = Random.Range(3f, 5f);
        _canFire = Time.time + _fireRate;
        GameObject laserPrefab;
        if (shotCount % 2 == 0)
            laserPrefab = alienBossLaser;
        else
            laserPrefab = chaser;
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        foreach (var l in laser.GetComponentsInChildren<Laser>())
            l.AssignAlienBossLaser();
        shotCount++;
    }

    public void Damage()
    {
        _lives--;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Laser laser = other.GetComponent<Laser>();
        if (other.CompareTag("Laser") && laser != null && laser.IsEnemyLaser() && laser.IsAlienBossLaser())
        {
            Destroy(other.gameObject);
            _lives--;
            iaudioSource.PlayOneShot(explosion);
            Debug.Log("boss is damaged from player laser");
        }

        if (other.CompareTag("Boss") && isAlienBossLaser == true)
        {
            return;
        }
        if (other.tag == ("Player"))
        {
            Player player = other.GetComponent<Player>();
            _lives--;
            player.Damage();
            iaudioSource.PlayOneShot(explosion);
            Debug.Log("boss got damage by player");
        }
        if (_lives <= 0)
        {
            Destroy(gameObject);
            _isDead = true;
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            spawnManager.OnBossDestroyed();
        }
    }
}