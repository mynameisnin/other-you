using UnityEngine;
using System.Collections;

/// <summary>
/// 보스 AI 시스템 클래스.
/// 플레이어 추적, 거리 기반 공격 결정, 무작위 공격 패턴 실행,
/// 이동 및 방향 전환 애니메이션 제어를 담당
/// 물리 이동은 FixedUpdate를 사용
/// </summary>
public class BossAiSystem : MonoBehaviour
{
    #region 변수 선언

    // --- 컴포넌트 참조 ---
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // --- 플레이어 관련 ---
    [Header("플레이어 참조")]
    [Tooltip("플레이어의 Transform. 태그로 자동 검색!! 할당 금지!!!!!")]
    [SerializeField] private Transform player;
    [Tooltip("플레이어 자동 검색 시 사용할 태그 목록 (순서대로 검색)")]
    [SerializeField] private string[] playerTags = { "AdamCamPosition", "DevaCamPosition", "Player" };

    // --- 공격 설정 ---
    [Header("공격 설정")]
    [Tooltip("플레이어가 이 거리 안에 들어오면 공격을 시작합니다.")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private Transform attackRangePosition;
    [Tooltip("공격 후 다음 공격까지의 대기 시간 (초)")]
    [SerializeField] private float attackCooldown = 2f;
    [Tooltip("공격 애니메이션 시작 후 실제 데미지 판정까지의 시간 (초). 애니메이션과 정확히 동기화 필요.")]
    [SerializeField] private float attackTiming = 0.2f;

    // --- 공격 판정 ---
    [Header("공격 판정")]
    [Tooltip("각 공격(1, 2, 3)에 사용할 공격 판정 위치 Transform 배열. 반드시 3개를 순서대로 할당.")]
    [SerializeField] private Transform[] attackBoxes = new Transform[3];

    [Tooltip("공격 대상으로 인식할 레이어 마스크.")]
    [SerializeField] private LayerMask playerLayer;


    // --- 이동 설정 ---
    [Header("이동 설정")]
    [Tooltip("플레이어 추적 시 이동 속도")]
    [SerializeField] private float moveSpeed = 2f;

    // --- 내부 상태 변수 ---
    private bool isAttacking = false;      // 현재 공격 코루틴 실행 중 여부 플래그
    private bool isMoving = false;         // 현재 이동 애니메이션 재생 중 여부 플래그
    private bool facingRight = true;       // 현재 보스가 오른쪽을 바라보고 있는지 여부
    private Vector2 moveDirection = Vector2.zero; // FixedUpdate에서 사용할 이동 방향 벡터
    private bool shouldMove = false;       // FixedUpdate에서 이동을 적용할지 여부 플래그

    #endregion

    #region 유니티 생명주기 메서드

    private void Start()
    {
        // 필수 컴포넌트 가져오기 및 null 체크
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // NOTE: Animator와 Rigidbody2D는 이 AI의 핵심 기능에 필수적이므로 없으면 비활성화.
        if (animator == null || rb == null)
        {
            Debug.LogError($"필수 컴포넌트 누락 (Animator: {animator == null}, Rigidbody2D: {rb == null})", this);
            this.enabled = false;
            return;
        }
        // NOTE: SpriteRenderer는 Flip 기능에 필요합니다. 없으면 경고만 출력하고 기능은 제한.
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer 컴포넌트가 없어 Flip 기능이 제한될 수 있습니다.", this);
        }
        ValidateAttackBoxes(); // 배열 유효성 검사 함수 호출 


        // Rigidbody 설정: 2D 사이드뷰 게임에서 일반적인 설정
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 플레이어 초기 검색
        if (player == null)
        {
            FindPlayerByTags();
        }

    
        // facingRight = !spriteRenderer.flipX; // 스프라이트의 초기 flipX 상태를 기준으로 설정
    }

    // Update: 매 프레임 호출, 주로 상태 확인 및 행동 결정 (Decision Making)
    private void Update()
    {
        // 플레이어 참조 유효성 검사 (매 프레임 중요)
        if (!IsPlayerValid())
        {
            // 플레이어를 잃었을 경우 처리
            StopMovement(); // 이동 중단
            if (isAttacking)
            {
                // NOTE: 진행 중인 공격 코루틴을 중단시킵니다. 게임 디자인에 따라 다를 수 있음.
                StopAllCoroutines(); // 모든 코루틴 중단 (AttackRoutine 포함)
                isAttacking = false; // 공격 상태 해제
            }
            FindPlayerByTags(); // 플레이어 다시 찾기 시도
            return; // 유효한 플레이어가 없으면 더 이상 진행하지 않음
        }

        // 플레이어와의 거리 계산
        float distance = Vector2.Distance(transform.position, player.position);
        if (attackRangePosition != null)
        {
            attackRangePosition.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
        // 공격 중이 아닐 때만 이동 또는 공격 결정
        if (!isAttacking)
        {
            // 공격 범위 밖: 이동 준비
            if (distance > attackRange)
            {
                // 플레이어를 향하는 방향 벡터 계산 후 이동 준비 함수 호출
                PrepareMove((player.position - transform.position).normalized);
            }
            // 공격 범위 안: 이동 멈추고 공격 시작
            else
            {
                StopMovement(); // 공격 전 이동 멈춤 (필수)
                StartCoroutine(AttackRoutine()); // 공격 코루틴 시작
            }
        }
    }

    // FixedUpdate: 고정된 시간 간격으로 호출, 주로 물리 관련 처리 (Movement Application)
    private void FixedUpdate()
    {
        // Update에서 이동하기로 결정했다면 (shouldMove == true) 실제 속도 적용
        if (shouldMove)
        {
            // NOTE: Y축 속도는 현재 값을 유지하여 중력 등의 영향을 받도록 합니다.
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
        }

    }

    #endregion

    #region 이동 및 방향 전환

    // 이동 준비 함수: 이동 방향 저장, 이동 상태 및 애니메이션 플래그 설정, 방향 전환 체크
    private void PrepareMove(Vector2 direction)
    {
        // FixedUpdate에서 사용할 이동 방향 업데이트
        moveDirection = direction;
        // FixedUpdate에서 이동을 적용하도록 플래그 설정
        shouldMove = true;

        // 이동 애니메이션 상태 관리
        if (!isMoving)
        {
            isMoving = true;
            animator.SetBool("Move", true); // "Move" 파라미터를 true로 설정 (Animator Controller 필요)
        }

        // 방향 전환 필요 여부 체크 및 처리 함수 호출
        HandleFlip(direction.x);
    }

    // 이동 중단 함수: 이동 상태 및 애니메이션 플래그 해제, 물리 속도 중지
    private void StopMovement()
    {
        // 이동 중이었을 때만 처리
        if (isMoving || shouldMove) // shouldMove도 false로 만들어 FixedUpdate에서 이동 안 하도록 함
        {
            isMoving = false;
            shouldMove = false;
            animator.SetBool("Move", false); // "Move" 파라미터를 false로 설정
            rb.velocity = new Vector2(0, rb.velocity.y); // 수평 속도 즉시 0으로 설정
        }
    }

    // 방향 전환 필요 여부 감지 및 Flip 함수 호출
    private void HandleFlip(float horizontalDirection)
    {
        // NOTE: 0.01f 와 같은 작은 임계값을 사용하여 방향 전환이 너무 민감하게 반응하지 않도록 해야함.
        // 오른쪽으로 가야하는데 왼쪽 보고 있을 때
        if (horizontalDirection > 0.01f && !facingRight)
            Flip();
        // 왼쪽으로 가야하는데 오른쪽 보고 있을 때
        else if (horizontalDirection < -0.01f && facingRight)
            Flip();
    }

    // 실제 방향 전환 로직: 상태 업데이트, 스프라이트 및 모든 공격 박스 위치 반전
    private void Flip()
    {
        // 내부 방향 상태 업데이트
        facingRight = !facingRight;

        // 스프라이트 렌더러 FlipX 설정
        if (spriteRenderer != null)
        {
            // NOTE: 이 로직은 원본 스프라이트가 오른쪽을 보도록 디자인
            // facingRight = true (오른쪽 봄) => flipX = false
            // facingRight = false (왼쪽 봄) => flipX = true
            spriteRenderer.flipX = !facingRight;
        }

        // 모든 공격 박스(Attack Boxes)의 로컬 X 위치 반전
        if (attackBoxes != null)
        {
            foreach (Transform box in attackBoxes)
            {
                // WARNING: 각 공격 박스는 반드시 이 보스 오브젝트의 '자식'이어야 localPosition 반전이 올바르게 동작.
                if (box != null && box.parent == transform)
                {
                    // 간단하게 현재 로컬 X 위치의 부호만 반전
                    box.localPosition = new Vector3(-box.localPosition.x, box.localPosition.y, box.localPosition.z);
                }
                // else: 자식이 아닌 경우 경고를 출력하거나 다른 처리 필요
            }
        }

        // CONSIDER: 방향 전환 애니메이션 추가
        // animator.SetTrigger("Flip");
    }

    #endregion

    #region 공격 로직

    // 공격 전체 흐름을 관리하는 코루틴
    private IEnumerator AttackRoutine()
    {
        // 공격 시작 상태 설정 및 이동 중단
        isAttacking = true;
        StopMovement(); // 공격 중에는 움직이지 않음

        // 무작위 공격 패턴 선택 (1, 2, 3 중 하나)
        int randomAttackIndex = Random.Range(1, 4);
        string attackTrigger = $"Attack{randomAttackIndex}"; // 사용할 애니메이터 트리거 이름
        animator.SetTrigger(attackTrigger); // 해당 공격 애니메이션 시작

        // 공격 판정 타이밍까지 대기
        yield return new WaitForSeconds(attackTiming);

        // 실제 데미지 판정 실행
        TryHitPlayer(randomAttackIndex); // 선택된 공격 인덱스에 맞는 판정 시도

        // 공격 애니메이션 종료 대기
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"));

        // 공격 쿨다운 대기
        yield return new WaitForSeconds(attackCooldown);

        // 공격 가능 상태로 복귀
        isAttacking = false;
    }

    // 실제 공격 판정 처리 함수
    private void TryHitPlayer(int attackIndex)
    {
        // 사용할 공격 박스 인덱스 계산 (1부터 시작하는 attackIndex를 0부터 시작하는 배열 인덱스로 변환)
        int boxArrayIndex = attackIndex - 1;

        // 배열 및 해당 인덱스의 유효성 검사 꼭 확인!!!
        if (attackBoxes == null || boxArrayIndex < 0 || boxArrayIndex >= attackBoxes.Length || attackBoxes[boxArrayIndex] == null)
        {
            Debug.LogError($"공격 판정 실패: AttackBoxes[{boxArrayIndex}] (Attack {attackIndex})가 유효하지 않습니다.", this);
            return; // 유효하지 않으면 판정 중단
        }

        // 현재 공격에 사용할 Attack Box Transform 가져오기
        Transform currentAttackBox = attackBoxes[boxArrayIndex];


    }

    #endregion

    #region 보조 기능

    // 플레이어 참조가 유효한지 (null 아니고 활성화 상태인지) 확인
    private bool IsPlayerValid()
    {
        // NOTE: player?. 은 null 조건부 연산자입니다. player가 null이면 바로 false 반환.
        return player != null && player.gameObject.activeInHierarchy;
    }

    // 설정된 태그 목록으로 활성화된 플레이어 오브젝트 찾기
    private void FindPlayerByTags()
    {
        GameObject foundPlayer = null;
        foreach (string tag in playerTags)
        {
            GameObject obj = GameObject.FindGameObjectWithTag(tag);
            if (obj != null && obj.activeInHierarchy)
            {
                foundPlayer = obj;
                break; // 찾으면 더 이상 검색하지 않음
            }
        }

        // 찾은 플레이어 설정 또는 기존 플레이어 참조 제거
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
            // Debug.Log($"플레이어 찾음: {player.name}", this); // 필요 시 로그 활성화
        }
        else if (player != null) // 이전에 플레이어가 있었는데 이제 못 찾는 경우
        {
            player = null; // 참조 제거
                           // Debug.LogWarning("플레이어 미아. 다음 Update에서 다시 검색합니다.", this); // 필요 시 로그 활성화
        }
    }

    // AttackBoxes 배열 유효성 검사 함수
    private void ValidateAttackBoxes()
    {
        if (attackBoxes == null || attackBoxes.Length != 3)
        {
            Debug.LogError($"AttackBoxes 배열 설정 오류: Null 또는 크기가 3이 아님 (현재 크기: {attackBoxes?.Length ?? 0})", this);
            // 필요 시 this.enabled = false; 처리
            return;
        }
        for (int i = 0; i < attackBoxes.Length; i++)
        {
            if (attackBoxes[i] == null)
            {
                Debug.LogError($"AttackBoxes[{i}]가 비어있습니다. 인스펙터에서 할당해야 합니다.", this);
            }
            else if (attackBoxes[i].parent != transform)
            {
                // WARNING: 자식이 아니면 Flip 로직이 제데로 작동 안함.
                Debug.LogWarning($"AttackBoxes[{i}] ({attackBoxes[i].name})는 이 오브젝트의 자식이 아닙니다. Flip 시 위치 문제가 발생할 수 있습니다.", attackBoxes[i]);
            }
        }
    }


    #endregion

    #region 디버그 시각화

    // 씬(Scene) 뷰에서 선택했을 때 Gizmos 그리기
    private void OnDrawGizmosSelected()
    {
        // 공격 가능 범위 (빨간색 원)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackRangePosition.position, attackRange);


    }

    #endregion
    // ==============================================================
    // 개발자 참고 사항
    // ==============================================================
    /*
     * 1. 공격 판정 방식:
     *    - 대신, 각 공격 애니메이션(Attack1, Attack2, Attack3)의 특정 프레임에
     *      애니메이션 이벤트(Animation Event)를 설정하여,
     *      해당 공격에 맞는 AttackDamageBox 자식 오브젝트의 Collider (Is Trigger)를 활성화/비활성화.
     *    - 실제 플레이어와의 충돌 감지 및 데미지 처리는 AttackDamageBox 오브젝트에
     *      별도로 추가된 스크립트 (예: AttackHitbox.cs - 이름은 예시) 내의 OnTriggerEnter2D 등에서 수행
     *
     * 2. 데미지 값 관리:
     *    - 중요: 이 BossAiSystem 스크립트에 직접적인 데미지 변수를 추가하거나 할당하지 마!!
     *    - 각 공격 패턴(Attack1, Attack2, Attack3)은 서로 다른 데미지 값을 가질 수 있어야하니까
     *    - 데미지 값은 각 AttackDamageBox에 연결된 스크립트 또는 관련 데이터에서 관리되어야해.
     *
     * 3. AttackBoxes 배열:
     *    - 인스펙터의 attackBoxes 배열에는 반드시 3개의 Transform(AttackDamageBox1, 2, 3 순서)을
     *      할당해야 하며, 이들은 모두 이 보스 오브젝트의 자식이어야 Flip 기능이 제대로 동작해.
     *      
     *                              코드 수정시 개발자 참고사항 수정 부탁
     */
}