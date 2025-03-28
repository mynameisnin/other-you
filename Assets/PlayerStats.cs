using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; // ? 싱글톤 적용

    [Header("Character Stats")]
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100; // ? 다음 레벨까지 필요한 경험치
    public int statPoints = 0;  // ? 스탯 포인트

    public int attackPower = 10;
    public int defense = 5;
    public int maxHealth = 100;
    public int currentHealth;

    public int maxEnergy = 100; // ? 최대 에너지 추가
    public int currentEnergy;   // ? 현재 에너지 추가

    [Header("Level Up Effect Settings")]
    public GameObject levelUpEffectPrefab; //  레벨업 이펙트 프리팹
    public Transform levelUpEffectPosition; // 이펙트를 생성할 위치 (Inspector에서 설정 가능)
    public float effectDuration = 2f; // 이펙트 지속 시간

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;

        currentMana = maxMana; //  마나도 최대치로 초기화

        if (HurtPlayer.Instance != null)
            HurtPlayer.Instance.UpdateHealthUI();

        if (EnergyBarUI.Instance != null)
            EnergyBarUI.Instance.RefreshFromPlayerStats();

        if (ManaBarUI.Instance != null)
            ManaBarUI.Instance.UpdateManaBar(currentMana); //  마나 UI 초기화
    }


    //  체력 회복 함수 추가
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);



        Debug.Log($"체력 회복! 현재 체력: {currentHealth} / {maxHealth}");
    }

    //  PlayerExperience에서 호출하는 레벨업 함수
    //  레벨업 처리
    public void LevelUp()
    {
        level++;
        statPoints += 3; // ? 스탯 포인트 지급

        Debug.Log($"레벨 업! 현재 레벨: {level}, 남은 스탯 포인트: {statPoints}");

        //  경험치 필요량 업데이트
        if (PlayerExperience.Instance != null)
        {
            experienceToNextLevel = PlayerExperience.Instance.xpToNextLevel;
        }
        // 레벨업 이펙트 실행
        PlayLevelUpEffect();
    }

    private void PlayLevelUpEffect()
    {
        if (levelUpEffectPrefab != null && levelUpEffectPosition != null)
        {
            GameObject effect = Instantiate(levelUpEffectPrefab, levelUpEffectPosition.position, Quaternion.identity);
            effect.transform.SetParent(levelUpEffectPosition); // 부모 설정하여 위치를 따라가도록 함

            float fadeDuration = 1.5f; // 페이드 아웃 지속 시간
            float moveDistance = 1f; // 위로 이동할 거리

            Sequence levelUpEffectSequence = DOTween.Sequence();
            levelUpEffectSequence.Append(effect.transform.DOMoveY(effect.transform.position.y + moveDistance, fadeDuration));

   
                //  2. SpriteRenderer가 있는 경우 (2D 게임 오브젝트용)
                SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    levelUpEffectSequence.Join(spriteRenderer.DOFade(0, fadeDuration));
                }

            

            // 애니메이션 완료 후 삭제
            levelUpEffectSequence.OnComplete(() => Destroy(effect));

            Debug.Log("레벨업 이펙트 실행!");
        }
        else
        {
            Debug.LogWarning("레벨업 이펙트 프리팹 또는 위치가 설정되지 않았습니다.");
        }
    }


    public void IncreaseMaxEnergy()
    {
        if (statPoints > 0)
        {
            maxEnergy += 10;
            currentEnergy = maxEnergy;
            statPoints--;

            Debug.Log($"최대 에너지 증가! 현재 에너지: {maxEnergy}, 남은 스탯 포인트: {statPoints}");

            if (EnergyBarUI.Instance != null)
            {
                EnergyBarUI.Instance.UpdateMaxEnergy(maxEnergy); // ? UI 업데이트
            }
        }
    }
        // ? 스탯 증가 함수
        public void IncreaseAttack()
    {
        if (statPoints > 0)
        {
            attackPower += 2;
            statPoints--;
            Debug.Log($"공격력 증가! 현재 공격력: {attackPower}, 남은 스탯 포인트: {statPoints}");
        }
    }

    public void IncreaseDefense()
    {
        if (statPoints > 0)
        {
            defense += 1;
            statPoints--;
            Debug.Log($"방어력 증가! 현재 방어력: {defense}, 남은 스탯 포인트: {statPoints}");
        }
    }

    public void IncreaseMaxHealth()
    {
        if (statPoints > 0)
        {
            maxHealth += 10;
            currentHealth = maxHealth;

            //  체력 UI만 업데이트
            if (HurtPlayer.Instance != null)
            {
                HurtPlayer.Instance.UpdateHealthUI();
            }

            if (HealthBarUI.Instance != null)
            {
                HealthBarUI.Instance.UpdateMaxHealth(maxHealth);
            }

            statPoints--;
            Debug.Log($"최대 체력 증가! 현재 체력: {maxHealth}, 남은 스탯 포인트: {statPoints}");
        }
    }

    public void IncreaseEnergy()
    {
        if (statPoints > 0)
        {
            maxEnergy += 10; //  스탯을 찍을 때만 최대 에너지 증가
            currentEnergy = maxEnergy; //  최대 에너지가 증가하면 현재 에너지도 회복
            statPoints--;

            Debug.Log($"최대 에너지 증가! 현재 에너지: {maxEnergy}, 남은 스탯 포인트: {statPoints}");

            //  EnergyBarUI에 업데이트 요청
            if (EnergyBarUI.Instance != null)
            {
                EnergyBarUI.Instance.UpdateMaxEnergy(maxEnergy);
            }
        }
        else
        {
            Debug.Log("스탯 포인트가 부족합니다!");
        }
    }
    public void SetCurrentEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(amount, 0, maxEnergy);
        EnergyBarUI.Instance?.RefreshFromPlayerStats(); //  UI에 직접 값 전달하지 않고 동기화 요청만
    }
    public int maxMana = 100;
    public int currentMana;

    public void ReduceMana(int amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        ManaBarUI.Instance?.UpdateManaBar(currentMana);
    }

    public void RecoverMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        ManaBarUI.Instance?.UpdateManaBar(currentMana);
    }

    public bool HasEnoughMana(int amount)
    {
        return currentMana >= amount;
    }

    public void SetCurrentMana(int amount)
    {
        currentMana = Mathf.Clamp(amount, 0, maxMana);
        ManaBarUI.Instance?.UpdateManaBar(currentMana);
    }

    public void IncreaseMaxMana()
    {
        if (statPoints > 0)
        {
            maxMana += 10;
            currentMana = maxMana;
            statPoints--;

            ManaBarUI.Instance?.UpdateMaxMana(maxMana);
        }
    }

}
