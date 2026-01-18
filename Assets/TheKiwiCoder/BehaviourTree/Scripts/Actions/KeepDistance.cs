using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;

public class KeepDistance : ActionNode
{
    public float safeDistance = 10f; // ✅ 유지할 거리
    private NavMeshAgent agent;

    protected override void OnStart()
    {
        agent = context.gameObject.GetComponent<NavMeshAgent>();
        agent.isStopped = false; // ✅ 강제 이동 활성화
        agent.ResetPath(); // ✅ 이전 이동 경로 삭제
    }

    protected override void OnStop()
    {
        //Debug.Log("🛑 [KeepDistance] 후퇴 중단");
    }

    protected override State OnUpdate()
    {
        if (blackboard.enemyTarget == null)
        {
            //Debug.LogWarning("❌ [KeepDistance] 후퇴 실패: 적 없음");
            return State.Failure;
        }

        Health myHealth = context.gameObject.GetComponent<Health>();
        if (myHealth == null)
        {
            //Debug.LogWarning("⚠️ [KeepDistance] 전투 상태 체크 실패: Health 컴포넌트 없음");
            return State.Failure;
        }

        float distance = Vector3.Distance(context.transform.position, blackboard.enemyTarget.position);


        // ✅ 전투 중이면 KeepDistance 유지
        if (myHealth.IsInCombat())
        {
            if (distance >= safeDistance)
            {
                //Debug.Log("✅ [KeepDistance] 전투 중 → 거리 유지 완료");
                return State.Running; // ✅ 전투 중이므로 계속 실행 유지
            }

            Vector3 direction = (context.transform.position - blackboard.enemyTarget.position).normalized;
            Vector3 newPosition = context.transform.position + direction * (safeDistance * 1.5f);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(newPosition, out hit, 5.0f, NavMesh.AllAreas))
            {
                newPosition = hit.position;
            }
            else
            {
                //Debug.LogWarning("⚠️ [KeepDistance] 후퇴 실패: 이동 가능한 위치 없음");
                return State.Failure;
            }

            agent.SetDestination(newPosition);
            //Debug.Log($"🏃‍♂️ [KeepDistance] 전투 중 → 거리 유지 중... (현재 거리: {distance}m, 목표 거리: {safeDistance}m)");

            return State.Running;
        }

        // ✅ 전투가 끝나면 Success 반환 → 랜덤 이동 실행
        //Debug.Log("⚠️ [KeepDistance] 전투 종료 → 랜덤 이동 시작");
        return State.Success;
    }
}
