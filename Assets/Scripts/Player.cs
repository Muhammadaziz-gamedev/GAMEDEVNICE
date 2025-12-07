using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float _speed = 3.5f;
    private float _shift = 9.5f;
    [SerializeField] 
    private bool isSlow = false;
    private float _slowMultiplier;
    private float normal_speed;
    [SerializeField]
    private float _speedMultiplier = 2f;
    [Header("Laser & PowerUps")]
    [SerializeField]
    private GameObject laserPrefab;

    [SerializeField]
    private GameObject tripleShotPrefab;
    [SerializeField]
    private GameObject bruhPrefab;
    private bool isUnusualLaserActive = false;
    private bool isTripleShotActive = false;

    [Header("_lives & Shield")]
    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private bool shieldActive = false;
    [SerializeField]
    private int _livesShield = 3;

    [SerializeField]
    private GameObject shieldVisual;

    [SerializeField]
    private GameObject leftEngine;

    [SerializeField]
    private GameObject rightEngine;
    [Header("Audio")]
    [SerializeField]
    private AudioClip laserSound;

    [SerializeField]
    private AudioClip explosionSound;

    [SerializeField]
    private AudioClip powerUpSound;
    private AudioSource audioSource;

    [Header("_score & UI")]
    [SerializeField]
    private int _score = 0;

    private UIManager uiManager;
    private SpawnManager spawnManager;
    [SerializeField]
    private float _fireRate = 0.4f;
    private float _canFire = -1f;
    [SerializeField]
    private GameObject thruster;
    private float _shiftDuration = 5f;
    private float can_shift = -1f;
    private float thrusterEnd = 0f;
    private float cooldown_shift = 5f;
    private bool is_shifting = false;
    [SerializeField]
    private GameObject redShield, greenShield;
    private int _ammoCount = 15;
    [SerializeField]
    private GameObject explosion;
    private Cam_shake _camera;
    private bool minusPowerUp;
    private bool isProjectileLaser = false;
    void Start()
    {
        _camera = Camera.main.GetComponent<Cam_shake>();
        normal_speed = _speed;
        transform.position = Vector3.zero;
        spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        uiManager = GameObject.Find("Canvas")?.GetComponent<UIManager>();
        audioSource = GetComponent<AudioSource>();
        if (spawnManager == null) Debug.LogError("SpawnManager is NULL!");
        if (uiManager == null) Debug.LogError("UIManager is NULL! Attach UIManager to Canvas!");
        if (audioSource == null) Debug.LogError("AudioSource is NULL! Attach AudioSource to Player!");
    }

    void Update()
    {
        HandleMovement();

        if (Input.GetKey(KeyCode.Space) && Time.time > _canFire)
        {
            Shoot();
        }
        Can_shift();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (minusPowerUp == false)
        {
            Vector3 direction = new Vector3(horizontal, vertical, 0);
            transform.Translate(direction * normal_speed * Time.deltaTime);
        }
        float clampedY = Mathf.Clamp(transform.position.y, -3.95f, 5.75f);
        transform.position = new Vector3(transform.position.x, clampedY, 0);

        if (transform.position.x < -11.1f) transform.position = new Vector3(11.05f, transform.position.y, 0);
        if (transform.position.x > 11.05f) transform.position = new Vector3(-11.1f, transform.position.y, 0);
    }

    void Shoot()
    {
        if (_ammoCount > 0)
        {
            _canFire = Time.time + _fireRate;
            _ammoCount--;
            uiManager.UpdateAmmo(_ammoCount);
            if (isProjectileLaser)
            {
                laserPrefab.GetComponent<Laser>().AssignProjectileLaser();
            }
            if (isUnusualLaserActive)
            {
                GameObject unusualLaser = Instantiate(bruhPrefab, transform.position, Quaternion.identity);
                Laser laser = unusualLaser.GetComponent<Laser>();
                if (laser != null) laser.AssignUnusualLaser();
            }
            else if (isTripleShotActive)
            {
                Instantiate(tripleShotPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(laserPrefab, transform.position, Quaternion.identity);
            }
            if (audioSource != null)
                audioSource.PlayOneShot(laserSound);
        }
    }

    public void Damage()
    {
        _camera.Shake(0.5f, 0.4f);
        if (shieldActive)
        {
            _livesShield--;
            if (_livesShield == 2)
            {
                shieldVisual.SetActive(false);
                greenShield.SetActive(true);
                redShield.SetActive(false);
            }
            else if (_livesShield == 1)
            {
                greenShield.SetActive(false);
                redShield.SetActive(true);
            }
            else if (_livesShield < 1)
            {
                redShield.SetActive(false);
                greenShield.SetActive(false);
                shieldVisual.SetActive(false);
                shieldActive = false;
            }
            return;
        }
        _lives--;
        if (uiManager != null)
            uiManager.UpdateLives(_lives);
        if (_lives == 2) rightEngine.SetActive(true);
        if (_lives == 1) leftEngine.SetActive(true);
        if (_lives < 1)
        {
            if (audioSource != null)
                normal_speed = 0;
            Instantiate(explosion, transform.position, Quaternion.identity);
            audioSource.PlayOneShot(explosionSound);
            spawnManager.OnPlayerDeath();
            Destroy(gameObject, 0.99f);
        }
    }

    public void TripleShotActive()
    {
        if (audioSource != null)
            audioSource.PlayOneShot(powerUpSound);
        isUnusualLaserActive = false;
        isTripleShotActive = true;
        StartCoroutine(TripleShotRoutine());
    }

    public void UnusualLaserActive()
    {
        if (audioSource != null)
            audioSource.PlayOneShot(powerUpSound);
        isTripleShotActive = false;
        isUnusualLaserActive = true;
        StartCoroutine(UnusuallaserRoutine());
    }

    IEnumerator UnusuallaserRoutine()
    {
        yield return new WaitForSeconds(5f);
        isUnusualLaserActive = false;
    }

    public void AddAmmo(int ammo)
    {
        if (audioSource != null) audioSource.PlayOneShot(powerUpSound);
        _ammoCount = ammo + 15;
    }

    public void AddHealth()
    {
        if (audioSource != null) audioSource.PlayOneShot(powerUpSound);
        _lives++;
        if (uiManager != null)
        {
            uiManager.UpdateLives(_lives);
        }
    }

    public void ProjectileMove()
    {
        isProjectileLaser = true;
        StartCoroutine(ProjectileMoveRoutine());
    }

    IEnumerator ProjectileMoveRoutine()
    {
        yield return new WaitForSeconds(5f);
        isProjectileLaser = false;
    }

    IEnumerator TripleShotRoutine()
    {
        yield return new WaitForSeconds(5f);
        isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        if (audioSource != null)
            audioSource.PlayOneShot(powerUpSound);

        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostRoutine());
    }

    public void MinusPowerUp()
    {
        if (audioSource != null)
            audioSource.PlayOneShot(powerUpSound);
        minusPowerUp = true;
        normal_speed = 0;
        StartCoroutine(MinuspowerupRoutine());
    }

    IEnumerator MinuspowerupRoutine()
    {
        yield return new WaitForSeconds(5f);
        minusPowerUp = false;
        normal_speed = _speed;
    }

    IEnumerator SpeedBoostRoutine()
    {
        yield return new WaitForSeconds(5f);
        _speed /= _speedMultiplier;
    }

    public void ShieldBoostActive()
    {
        if (audioSource != null)
            audioSource.PlayOneShot(powerUpSound);

        shieldActive = true;
        greenShield.SetActive(false);
        redShield.SetActive(false);
        shieldVisual.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        if (uiManager != null)
        {
            uiManager.UpdateScore(_score);
        }
        else
        {
            Debug.LogError("UIManager is NULL! Cannot update _score.");
        }

    }

    private void Can_shift()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > can_shift)
        {
            thruster.SetActive(true);
            normal_speed = _shift;
            thrusterEnd = Time.time + _shiftDuration;
            can_shift = thrusterEnd + cooldown_shift;
            is_shifting = true;
        }
        if (is_shifting && Time.time >= thrusterEnd)
        {
            thruster.SetActive(false);
            normal_speed = _speed;
            is_shifting = false;
        }
    }

    public void SlowEffect()
    {
        if (isSlow == false) return;
        isSlow = true;
        normal_speed *= _slowMultiplier;
        StartCoroutine(Slowroutine());
    }

    IEnumerator Slowroutine()
    {
        yield return new WaitForSeconds(3.0f);
        isSlow = false;
        normal_speed = _speed;
    }
}