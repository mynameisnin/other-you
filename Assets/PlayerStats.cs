using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; //  �̱��� ����

    [Header("Character Stats")]
    public int level = 1;         // ���� ����
    public int experience = 0;    // ���� ����ġ
    public int experienceToNextLevel = 100; // ������ �ʿ� ����ġ

    public int attackPower = 10;  // ���ݷ�
    public int defense = 5;       // ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // ����ġ �߰� �� ������ ó��
    public void GainExperience(int amount)
    {
        experience += amount;

        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    // ������ ó��
    private void LevelUp()
    {
        experience -= experienceToNextLevel;
        level++;
        experienceToNextLevel += 50; // ������ �� �ʿ� ����ġ ����

        Debug.Log($"������! ���� ����: {level}");
    }
}