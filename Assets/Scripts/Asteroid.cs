using System.Collections;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 3.0f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Boss"))
        {
            return;
        }
        if (other.CompareTag("Laser"))
        {
            _spawnManager.StartSpawning();
            Destroy(other.gameObject);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}     
