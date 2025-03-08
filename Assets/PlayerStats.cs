using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; //  싱글톤 적용

    [Header("Character Stats")]
    public int level = 1;         // 현재 레벨
    public int experience = 0;    // 현재 경험치
    public int experienceToNextLevel = 100; // 레벨업 필요 경험치

    public int attackPower = 10;  // 공격력
    public int defense = 5;       // 방어력

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // 경험치 추가 및 레벨업 처리
    public void GainExperience(int amount)
    {
        experience += amount;

        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    // 레벨업 처리
    private void LevelUp()
    {
        experience -= experienceToNextLevel;
        level++;
        experienceToNextLevel += 50; // 레벨업 시 필요 경험치 증가

        Debug.Log($"레벨업! 현재 레벨: {level}");
    }
}