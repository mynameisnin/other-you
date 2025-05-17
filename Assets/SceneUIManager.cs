using UnityEngine;
// using DG.Tweening; // DOTween�� ������� �����Ƿ� �ּ� ó�� �Ǵ� ���� ����

public class SceneUIManager : MonoBehaviour
{
    [Header("Scene Specific UI Elements")]
    // �ν����Ϳ��� �� ���� DeathPanel (���� �� ��Ȱ��ȭ ���¿��� ��)�� �Ҵ��մϴ�.
    public GameObject deathPanelUI;

    void Awake()
    {
        // ���� �� DeathPanelUI�� �ǵ��� �ʱ� ���·� ���� (��Ȱ��ȭ)
        if (deathPanelUI != null)
        {
            deathPanelUI.SetActive(false);
        }
        else
        {
            Debug.LogError("SceneUIManager�� DeathPanelUI�� �Ҵ���� �ʾҽ��ϴ�! �ν����Ϳ��� �Ҵ����ּ���.");
        }
    }

    // �� �޼ҵ�� HurtPlayer�� ȣ���Ͽ� DeathPanelUI�� Ȱ��ȭ�մϴ�.
    public void ShowManagedDeathPanel()
    {
        if (deathPanelUI != null)
        {
            deathPanelUI.SetActive(true);
            Debug.Log("DeathPanel UI Ȱ��ȭ�� by SceneUIManager");
        }
        else
        {
            Debug.LogError("SceneUIManager�� DeathPanelUI�� �Ҵ���� �ʾ����� Ȱ��ȭ �õ��� �־����ϴ�!");
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