using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 0.8f); // 자동 삭제
    }
}
