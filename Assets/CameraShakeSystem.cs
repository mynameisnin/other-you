using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeSystem : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPostion = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f , 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPostion.x + x, originalPostion.y + y, originalPostion.z);

            elapsed += Time.deltaTime;

            yield return null;

        }
        transform.localPosition = originalPostion;
    }    
}
