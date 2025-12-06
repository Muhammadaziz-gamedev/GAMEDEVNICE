using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball_alien : MonoBehaviour
{
    // Start is called before the first frame update
    private float _speed = 4f;
    private float _chaseTime = 3f;
    private bool chasing = true;
    private float _timer = 0f;
    private Transform _target;

    void Start()
    {
        if (_target == null)
        {
            _target = GameObject.FindWithTag("Player").transform;
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer <= _chaseTime)
        {
            chasing = true;
        }
        else
        {
            StopChase();
        }
        if (chasing)
        {
            Vector3 chase = (_target.position - transform.position).normalized;
            transform.up = chase;
            transform.position += chase * _speed * Time.deltaTime;
        }
        else
        {
            transform.position += transform.up * _speed * Time.deltaTime;
        }
    }

    void StopChase()
    {
        chasing = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(this.gameObject);
            other.GetComponent<Player>().Damage();
        }
        else
        {
            return;
        }
    }
}
