using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;

public class KeepDistance2 : ActionNode
{
    public float detectionRadius = 10f;  // 주변 적 탐색 반경
    public float retreatStep = 3f;       // 한번에 후퇴하는 거리
    private NavMeshAgent agent;

    protected override void OnStart()
    {
        agent = context.gameObject.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        Debug.Log("✅ [KeepDistance2] 초기화 완료, NavMeshAgent 설정됨");
    }

    protected override void OnStop()
    {
        Debug.Log("🛑 [KeepDistance2] 행동 중단");
    }

    protected override State OnUpdate()
    {
        // 모든 주변 콜라이더 탐색
        Collider[] colliders = Physics.OverlapSphere(context.transform.position, detectionRadius);
        Debug.Log($"👀 [KeepDistance2] 탐지된 전체 콜라이더 수: {colliders.Length}");

        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;

        // 콜라이더 중 적 태그가 있는 가장 가까운 유닛 탐색
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(context.transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = collider.transform;
                }
            }
        }

        if (closestEnemy == null)
        {
            Debug.Log("✅ [KeepDistance2] 주변에 적이 없음. Success 반환.");
            return State.Success;
        }

        Debug.Log($"🎯 [KeepDistance2] 가장 가까운 적 발견: {closestEnemy.name}, 거리: {closestDistance}");

        // 적과 반대 방향으로 후퇴 위치 계산
        Vector3 retreatDirection = (context.transform.position - closestEnemy.position).normalized;
        Vector3 retreatPosition = context.transform.position + retreatDirection * retreatStep;
        Debug.Log($"🚶 [KeepDistance2] 후퇴 시도 위치: {retreatPosition}");

        if (NavMesh.SamplePosition(retreatPosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Debug.Log($"🏃‍♂️ [KeepDistance2] 후퇴 가능. 목적지 설정됨: {hit.position}");
            return State.Running;
        }

        Debug.LogWarning("❌ [KeepDistance2] 유효한 NavMesh 위치를 찾을 수 없음. 후퇴 실패.");
        return State.Failure;
    }
}
