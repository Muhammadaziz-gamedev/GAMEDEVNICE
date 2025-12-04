using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] 
    private Text _scoreText;
    [SerializeField] 
    private Image _livesImage;
    [SerializeField] 
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _gameoverText;
    [SerializeField]
    private Text _restartText;
    private SpawnManager spawnManager;
    private GameManager _gameManager;
    [SerializeField]
    private Text _ammoCountText;
    [SerializeField]
    private Text _completed;
    [SerializeField]
    private Text _bossWave;

    void Start()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if(spawnManager == null) Debug.Log("spawnmanager is null");
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("GameManager is NULL! Attach GameManager to a GameObject in the scene.");
        _gameoverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
    }

    public void boss()
    {
    if(spawnManager._waveCount == 5)
        {
            _bossWave.gameObject.SetActive(true);
            StartCoroutine(BosswaveFlickerRoutine());
        }
    if(spawnManager._bossDestroyed == true && spawnManager._stopSpawning == true)
        {
            _completed.gameObject.SetActive(true);
        }
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateAmmo(int ammo)
    {
        _ammoCountText.text = "Ammo: " + ammo;
    }

    public void UpdateLives(int currentLives)
    {
    if (currentLives >= 0 && currentLives < _liveSprites.Length)
        _livesImage.sprite = _liveSprites[currentLives];
    if (currentLives == 0)
        {
            _gameoverText.gameObject.SetActive(true);
            _restartText.gameObject.SetActive(true);
            _gameManager.GameOver();
            StartCoroutine(GameOverFlickerRoutine());
        }
    }

    private IEnumerator GameOverFlickerRoutine()
    {
    while (true)
        {
            _gameoverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameoverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
    private IEnumerator BosswaveFlickerRoutine()
    {
    while (true)
        {
            _bossWave.text = _bossWave.text;
            yield return new WaitForSeconds(0.5f);
            _bossWave.text = "";
            yield return new WaitForSeconds(0.5f);
            _bossWave.text = _bossWave.text;
            yield return new WaitForSeconds(0.5f);
            _bossWave.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
