using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class EnemyFT : ActionNode
{
    public float detectionRadius = 5f;

    protected override void OnStart()
    {
        //Debug.Log("🔍 아군 탐색 시작");
    }

    protected override void OnStop()
    {
        //Debug.Log("🔍 아군 탐색 중단");
    }

    protected override State OnUpdate()
    {
        if (blackboard.allyTarget != null)
        {
            //Debug.Log("✅ 기존 아군 유지");
            return State.Success;
        }

        //Debug.Log("🔍 FindAlly 실행 중...");

        Collider[] colliders = Physics.OverlapSphere(context.transform.position, detectionRadius);

        Transform closestAlly = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Ally"))
            {
                float distance = Vector3.Distance(context.transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestAlly = collider.transform;
                }
            }
        }

        if (closestAlly != null)
        {
            blackboard.allyTarget = closestAlly;
            //Debug.Log($"🎯 아군 발견: {blackboard.allyTarget.name}");
            /*
            //자신도 전투 진입
            Health selfHealth = context.gameObject.GetComponent<Health>();
            if (selfHealth != null)
            {
                selfHealth.lastDamageTime = Time.time;
            }
            */
            return State.Success;
        }

        //Debug.Log("❌ EnemyFT 실패: 아군 없음");
        return State.Failure;
    }
}
