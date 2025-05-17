using UnityEngine;
// using DG.Tweening; // DOTween을 사용하지 않으므로 주석 처리 또는 삭제 가능

public class SceneUIManager : MonoBehaviour
{
    [Header("Scene Specific UI Elements")]
    // 인스펙터에서 이 씬의 DeathPanel (시작 시 비활성화 상태여도 됨)을 할당합니다.
    public GameObject deathPanelUI;

    void Awake()
    {
        // 시작 시 DeathPanelUI를 의도한 초기 상태로 설정 (비활성화)
        if (deathPanelUI != null)
        {
            deathPanelUI.SetActive(false);
        }
        else
        {
            Debug.LogError("SceneUIManager에 DeathPanelUI가 할당되지 않았습니다! 인스펙터에서 할당해주세요.");
        }
    }

    // 이 메소드는 HurtPlayer가 호출하여 DeathPanelUI를 활성화합니다.
    public void ShowManagedDeathPanel()
    {
        if (deathPanelUI != null)
        {
            deathPanelUI.SetActive(true);
            Debug.Log("DeathPanel UI 활성화됨 by SceneUIManager");
        }
        else
        {
            Debug.LogError("SceneUIManager에 DeathPanelUI가 할당되지 않았지만 활성화 시도가 있었습니다!");
        }
    }
    public void OnClick_Respawn()
    {
        if (HurtPlayer.Instance != null)
        {
            HurtPlayer.Instance.RespawnPlayer();
            
        }
    }
}