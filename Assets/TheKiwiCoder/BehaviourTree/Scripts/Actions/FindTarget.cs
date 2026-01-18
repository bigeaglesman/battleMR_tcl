using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class FindTarget : ActionNode
{
    public float detectionRadius = 5f; // 탐색 범위
    public bool findEnemy = true; // ✅ 적 탐색 여부
    public bool findAlly = false; // ✅ 아군 탐색 여부 (힐러용)

    protected override void OnStart()
    {
        //Debug.Log("🔍 [FindTarget] 실행 시작");
    }

    protected override void OnStop()
    {
        //Debug.Log("🔍 [FindTarget] 실행 중단");
    }

    protected override State OnUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(context.transform.position, detectionRadius);
        //Debug.Log($"🛰️ [FindTarget] 주변 탐색 범위: {detectionRadius}m, 감지된 콜라이더 수: {colliders.Length}");

        Transform closestEnemy = null;
        float closestEnemyDistance = float.MaxValue;

        Transform lowestHealthAlly = null;
        float lowestAllyHealth = float.MaxValue;

        Transform fallbackAlly = null;
        float closestAllyDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            float distance = Vector3.Distance(context.transform.position, collider.transform.position);

            if (findEnemy && collider.CompareTag("Enemy"))
            {
                if (distance < closestEnemyDistance)
                {
                    closestEnemyDistance = distance;
                    closestEnemy = collider.transform;
                }
            }

            if (findAlly && collider.CompareTag("Ally") && collider.transform != context.transform)
            {
                Health allyHealth = collider.GetComponent<Health>();
                if (allyHealth != null)
                {
                    //Debug.Log($"🧍‍♂️ [FindTarget] 아군 탐지: {collider.name}, 체력: {allyHealth.currentHealth}/{allyHealth.maxHealth}");

                    if (allyHealth.currentHealth < allyHealth.maxHealth)
                    {
                        if (allyHealth.currentHealth < lowestAllyHealth)
                        {
                            lowestAllyHealth = allyHealth.currentHealth;
                            lowestHealthAlly = collider.transform;
                        }
                    }

                    if (distance < closestAllyDistance)
                    {
                        closestAllyDistance = distance;
                        fallbackAlly = collider.transform;
                    }
                }
            }
        }

        // 적 타겟 갱신
        if (findEnemy)
        {
            blackboard.enemyTarget = closestEnemy;

            if (closestEnemy != null)
            {
                //UnitStats에도 현재 타겟 저장
                UnitStats stats = context.gameObject.GetComponent<UnitStats>();
                if (stats != null)
                {
                    stats.currentTarget = closestEnemy;
                    stats.currentTargetName = closestEnemy.name;
                }
                //Debug.Log($"🎯 [FindTarget] 적 타겟 설정: {closestEnemy.name}");
            }
        }

        // 아군 타겟 갱신
        if (findAlly)
        {
            Transform newTarget = lowestHealthAlly != null ? lowestHealthAlly : fallbackAlly;

            if (blackboard.allyTarget == null)
            {
                blackboard.allyTarget = newTarget;
                //Debug.Log($"🆕 [FindTarget] 아군 타겟 최초 설정: {newTarget?.name}");
            }
            else
            {
                Health current = blackboard.allyTarget.GetComponent<Health>();
                Health incoming = newTarget != null ? newTarget.GetComponent<Health>() : null;

                if (incoming != null && current != null)
                {
                    //Debug.Log($"🔁 [FindTarget] 아군 비교 → 현재: {current.name}({current.currentHealth}), 후보: {incoming.name}({incoming.currentHealth})");

                    if (incoming.currentHealth < current.currentHealth)
                    {
                        blackboard.allyTarget = newTarget;
                        //Debug.Log($"✅ [FindTarget] 아군 타겟 변경: {newTarget.name}");
                    }
                }
            }
        }

        if ((findEnemy && blackboard.enemyTarget != null) || (findAlly && blackboard.allyTarget != null))
        {
            //Debug.Log("✅ [FindTarget] 타겟 탐색 성공");
            return State.Success;
        }

        //Debug.LogWarning("❌ [FindTarget] 탐색 실패 - 유효한 대상 없음");
        return State.Failure;
    }
}
