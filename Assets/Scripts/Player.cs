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
    [SerializeField] public bool _isSlow = false;
    private float _slowMultiplier;
    private float _normalSpeed;
    [SerializeField]
    private float _speedMultiplier = 2f;
    [Header("Laser & PowerUps")]
    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _bruhPrefab;
    private bool _isUnusualLaserActive = false;
    private bool _isTripleShotActive = false;

    [Header("Lives & Shield")]
    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private bool _shieldActive = false;
    [SerializeField]
    private int _livesShield = 3;

    [SerializeField]
    private GameObject _shieldVisual;

    [SerializeField]
    private GameObject _leftEngine;

    [SerializeField]
    private GameObject _rightEngine;
    [Header("Audio")]
    [SerializeField]
    private AudioClip _laserSound;

    [SerializeField]
    private AudioClip _explosionSound;

    [SerializeField]
    private AudioClip _powerUpSound;

    private AudioSource _audioSource;

    [Header("Score & UI")]
    [SerializeField]
    private int _score = 0;

    private UIManager _uiManager;
    private SpawnManager _spawnManager;
    [SerializeField]
    private float _fireRate = 4f;
    private float _canFire = -1f;
    [SerializeField]
    private GameObject _thruster;
    private float _shiftDuration = 5f;
    private float _canShift = -1f;
    private float _thrusterEnd = 0f;
    private float _cooldownShift = 5f;
    private bool _isShifting = false;
    [SerializeField]
    private GameObject _redShield, _greenShield;
    private int _ammoCount = 15;
    [SerializeField]
    private GameObject _explosion;
    private Cam_shake _camera;
    private bool _minusPowerUp;
    private bool _isProjectileLaser = false;
    void Start()
    {
        _camera = Camera.main.GetComponent<Cam_shake>();
        _normalSpeed = _speed;
        transform.position = Vector3.zero;
        _spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas")?.GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        if (_spawnManager == null) Debug.LogError("SpawnManager is NULL!");
        if (_uiManager == null) Debug.LogError("UIManager is NULL! Attach UIManager to Canvas!");
        if (_audioSource == null) Debug.LogError("AudioSource is NULL! Attach AudioSource to Player!");
    }

    void Update()
    {
        HandleMovement();

        if (Input.GetKey(KeyCode.Space) && Time.time > _canFire)
        {
            Shoot();
        }
        CanShift();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (_minusPowerUp == false)
        {
            Vector3 direction = new Vector3(horizontal, vertical, 0);
            transform.Translate(direction * _normalSpeed * Time.deltaTime);
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
            _uiManager.UpdateAmmo(_ammoCount);
            if (_isProjectileLaser)
            {
                _laserPrefab.GetComponent<Laser>().AssignProjectileLaser();
            }
            if (_isUnusualLaserActive)
            {
                GameObject unusualLaser = Instantiate(_bruhPrefab, transform.position, Quaternion.identity);
                Laser laser = unusualLaser.GetComponent<Laser>();
                if (laser != null) laser.AssignUnusualLaser();
            }
            else if (_isTripleShotActive)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            }
            if (_audioSource != null)
                _audioSource.PlayOneShot(_laserSound);
        }
    }

    public void Damage()
    {
        _camera.Shake(0.5f, 0.4f);
        if (_shieldActive)
        {
            _livesShield--;
            if (_livesShield == 2)
            {
                _shieldVisual.SetActive(false);
                _greenShield.SetActive(true);
                _redShield.SetActive(false);
            }
            else if (_livesShield == 1)
            {
                _greenShield.SetActive(false);
                _redShield.SetActive(true);
            }
            else if (_livesShield < 1)
            {
                _redShield.SetActive(false);
                _greenShield.SetActive(false);
                _shieldVisual.SetActive(false);
                _shieldActive = false;
            }
            return;
        }
        _lives--;
        if (_uiManager != null)
            _uiManager.UpdateLives(_lives);
        if (_lives == 2) _rightEngine.SetActive(true);
        if (_lives == 1) _leftEngine.SetActive(true);
        if (_lives < 1)
        {
            if (_audioSource != null)
                _normalSpeed = 0;
            Instantiate(_explosion, transform.position, Quaternion.identity);
            _audioSource.PlayOneShot(_explosionSound);
            _spawnManager.OnPlayerDeath();
            Destroy(gameObject, 0.99f);
        }
    }

    public void TripleShotActive()
    {
        if (_audioSource != null)
            _audioSource.PlayOneShot(_powerUpSound);
        _isUnusualLaserActive = false;
        _isTripleShotActive = true;
        StartCoroutine(TripleShotRoutine());
    }

    public void UnusualLaserActive()
    {
        if (_audioSource != null)
            _audioSource.PlayOneShot(_powerUpSound);
        _isTripleShotActive = false;
        _isUnusualLaserActive = true;
        StartCoroutine(UnusuallaserRoutine());
    }

    IEnumerator UnusuallaserRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isUnusualLaserActive = false;
    }

    public void AddAmmo(int ammo)
    {
        if (_audioSource != null) _audioSource.PlayOneShot(_powerUpSound);
        _ammoCount = ammo + 15;
    }

    public void AddHealth()
    {
        if (_audioSource != null) _audioSource.PlayOneShot(_powerUpSound);
        _lives++;
        if (_uiManager != null)
        {
            _uiManager.UpdateLives(_lives);
        }
    }

    public void ProjectileMove()
    {
        _isProjectileLaser = true;
        StartCoroutine(ProjectileMoveRoutine());
    }

    IEnumerator ProjectileMoveRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isProjectileLaser = false;
    }

    IEnumerator TripleShotRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        if (_audioSource != null)
            _audioSource.PlayOneShot(_powerUpSound);

        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostRoutine());
    }

    public void MinusPowerUp()
    {
        if (_audioSource != null)
            _audioSource.PlayOneShot(_powerUpSound);
        _minusPowerUp = true;
        _normalSpeed = 0;
        StartCoroutine(minuspowerupRoutine());
    }

    IEnumerator minuspowerupRoutine()
    {
        yield return new WaitForSeconds(5f);
        _minusPowerUp = false;
        _normalSpeed = _speed;
    }

    IEnumerator SpeedBoostRoutine()
    {
        yield return new WaitForSeconds(5f);
        _speed /= _speedMultiplier;
    }

    public void ShieldBoostActive()
    {
        if (_audioSource != null)
            _audioSource.PlayOneShot(_powerUpSound);

        _shieldActive = true;
        _greenShield.SetActive(false);
        _redShield.SetActive(false);
        _shieldVisual.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        if (_uiManager != null)
        {
            _uiManager.UpdateScore(_score);
        }
        else
        {
            Debug.LogError("UIManager is NULL! Cannot update score.");
        }

    }

    private void CanShift()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > _canShift)
        {
            _thruster.SetActive(true);
            _normalSpeed = _shift;
            _thrusterEnd = Time.time + _shiftDuration;
            _canShift = _thrusterEnd + _cooldownShift;
            _isShifting = true;
        }
        if (_isShifting && Time.time >= _thrusterEnd)
        {
            _thruster.SetActive(false);
            _normalSpeed = _speed;
            _isShifting = false;
        }
    }

    public void SlowEffect()
    {
        if (_isSlow == false) return;
        _isSlow = true;
        _normalSpeed *= _slowMultiplier;
        StartCoroutine(Slowroutine());
    }

    IEnumerator Slowroutine()
    {
        yield return new WaitForSeconds(3.0f);
        _isSlow = false;
        _normalSpeed = _speed;
    }
}