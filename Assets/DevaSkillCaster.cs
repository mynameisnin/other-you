using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevaSkillCaster : MonoBehaviour
{
    public GameObject skillPrefab;
    public Transform spawnPoint; // ��ȯ ��ġ 
    public float skillSpawnInterval = 0.2f;
    private bool isCasting = false;

    public void StartCastingSkill()
    {
        if (!isCasting)
        {
            isCasting = true;
            StartCoroutine(CastSkillCoroutine());
        }
    }

    public void StopCastingSkill()
    {
        isCasting = false;
    }

    private IEnumerator CastSkillCoroutine()
    {
        while (isCasting)
        {
            GameObject skill = Instantiate(skillPrefab, spawnPoint.position, Quaternion.identity);

            // ���� �ݿ�
            SpriteRenderer devaSR = GetComponent<SpriteRenderer>();
            if (devaSR != null && devaSR.flipX)
            {
                Vector3 scale = skill.transform.localScale;
                scale.x *= -1;
                skill.transform.localScale = scale;
            }

            yield return new WaitForSeconds(skillSpawnInterval);
        }
    }
}