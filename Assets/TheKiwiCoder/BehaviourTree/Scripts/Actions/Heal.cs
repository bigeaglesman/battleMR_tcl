using TheKiwiCoder;
using UnityEngine;

public class Heal : ActionNode
{
    public float healAmount = 20f; // ✅ 힐링량 설정
    public float healCooldown = 3.0f; // ✅ 힐 쿨타임 (기본 3초)
    //public float checkInterval = 1.0f; // ✅ 전투 상태 체크 간격
    private float lastHealTime = 0f;
    private UnitAnimator animator;
    //private float lastCheckTime = 0f;
    public GameObject healEffectPrefab;

    protected override void OnStart()
    {
        //Debug.Log("🩹 [Heal] 힐링 시작");
        animator = context.gameObject.GetComponent<UnitAnimator>();
    }

    protected override void OnStop()
    {
        //Debug.Log("🩹 [Heal] 힐링 종료");
    }

    protected override State OnUpdate()
    {
        if (blackboard.allyTarget == null)
        {
            //Debug.LogWarning("❌ [Heal] 힐링 실패: 대상 없음");
            return State.Failure;
        }

        Health allyHealth = blackboard.allyTarget.GetComponent<Health>();
        if (allyHealth == null)
        {
            //Debug.LogWarning("❌ [Heal] 힐링 실패: Health 컴포넌트 없음");
            return State.Failure;
        }

        // ✅ 힐 시 공격 범위(`attackRange`) 내에 있어야 유지됨
        float distance = Vector3.Distance(context.transform.position, blackboard.allyTarget.position);
        if (distance > blackboard.attackRange)
        {
            //Debug.LogWarning($"❌ [Heal] 힐 실패: 사거리 초과 (현재 거리: {distance}m, 최대 사거리: {blackboard.attackRange}m)");
            return State.Failure;
        }

        /*
        // ✅ 일정 시간마다 전투 상태 확인
        if (Time.time > lastCheckTime + checkInterval)
        {
            lastCheckTime = Time.time;
            if (!allyHealth.IsInCombat()) // ✅ 아군이 전투 중인지 확인
            {
                Debug.Log("⚠️ [Heal] 아군이 전투 중이 아님 → 힐 종료");
                return State.Success;
            }
        }
        */

        if (Time.time < lastHealTime + healCooldown) // ✅ 힐 쿨타임 확인
        {
            return State.Running;
        }

        // ✅ 체력 회복
        animator.PlayAttack();
        float newHealth = Mathf.Min(allyHealth.currentHealth + healAmount, allyHealth.maxHealth);
        allyHealth.currentHealth = newHealth;

        if (allyHealth.healthBar)
        {
            allyHealth.healthBar.SetHealthBarPercentage(newHealth / allyHealth.maxHealth);
        }

        

        lastHealTime = Time.time;
        //Debug.Log($"🩹 [Heal] {blackboard.allyTarget.name} 힐링 완료 (+{healAmount}), 현재 체력: {allyHealth.currentHealth}");
        if (healEffectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(healEffectPrefab, blackboard.allyTarget.position, Quaternion.identity);
            effect.transform.SetParent(blackboard.allyTarget); // 👈 유닛 이동 시 같이 움직이도록
            GameObject.Destroy(effect, 2f); // 👈 2초 후 자동 삭제
        }

        animator.StopAttack();

        return State.Success;
    }
}
