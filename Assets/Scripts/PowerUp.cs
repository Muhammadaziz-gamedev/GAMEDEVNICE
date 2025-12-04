    using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private int _powerUpID;
    [SerializeField] private AudioClip _clip;
    private Player _player;
    private float _speeda = 6f;

    void Update()
    {
    _player = FindObjectOfType<Player>();
    transform.Translate(Vector3.down * _speed * Time.deltaTime);
    if (transform.position.y < -4.5f) 
        Destroy(gameObject);
    if (Input.GetKey(KeyCode.C))
        {
            Vector3 direction= (_player.transform.position - transform.position).normalized;
            transform.Translate(direction * _speeda * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
        Player player = other.GetComponent<Player>();
        AudioSource.PlayClipAtPoint(_clip, transform.position);
            if (player != null)
            {
            switch (_powerUpID)
                {
                    case 0: player.TripleShotActive(); break;
                    case 1: player.SpeedBoostActive(); break;
                    case 2: player.ShieldBoostActive(); break;
                    case 4: player.AddAmmo(15); break;
                    case 5: player.Addhealth(); break;
                    case 6: player.unusuallaseractive(); break;
                    case 7: player.minuspowerup(); break;
                    case 8: player.projectilemove(); break;
                }
            Destroy(gameObject);
            }
        }
    }
}
