using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Cam_shake : MonoBehaviour
{
    private Vector3 _position;
    void Start()
    {
        _position = transform.localPosition;
    }
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(Shakecoroutine(duration ,magnitude));
    }
    private IEnumerator Shakecoroutine(float duration, float magnitude)
    {
        float elapsed = 0;
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                transform.localPosition = _position + new Vector3(x, y, 0);
                elapsed += Time.deltaTime;
                yield return null;
            }
        transform.localPosition = _position;
    }
}
