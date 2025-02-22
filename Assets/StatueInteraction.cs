using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D 사용을 위해 필요

public class StatueInteraction : MonoBehaviour
{
    private Light2D statueLight;
    private bool isActivated = false; // 한 번 붉은색으로 바뀌었는지 확인하는 변수
    private bool isPlayerNearby = false; // 플레이어가 근처에 있는지 확인

    public Color blueColor = Color.blue;
    public Color redColor = Color.red;
    public ParticleSystem featherEffect; // 깃털 파티클 시스템

    void Start()
    {
        statueLight = GetComponent<Light2D>();
        statueLight.color = blueColor; // 초기 색상 설정

        if (featherEffect != null)
        {
            featherEffect.Stop(); // 시작할 때 파티클 정지
        }
    }

    void Update()
    {
        // 플레이어가 근처에 있고 ↑ 키를 누르면 활성화
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.UpArrow) && !isActivated)
        {
            statueLight.color = redColor; // 붉은빛으로 변경
            isActivated = true; // 한 번 활성화되면 유지

            if (featherEffect != null)
            {
                featherEffect.Play(); // 깃털 효과 실행
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // 플레이어가 동상 근처에 왔을 때만 체크
        {
            isPlayerNearby = true; // 플레이어가 근처에 있음을 표시
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // 플레이어가 멀어지면
        {
            isPlayerNearby = false; // 범위에서 벗어남
        }
    }
}
