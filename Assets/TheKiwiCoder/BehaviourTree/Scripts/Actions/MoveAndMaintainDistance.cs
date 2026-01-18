using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;

public class MoveAndMaintainDistance : ActionNode
{
    private NavMeshAgent agent;
    public float preferredDistance = 15f; // ✅ 궁수가 유지할 적절한 거리
    public float tolerance = 3f; // ✅ 거리 조절 허용 오차

    protected override void OnStart()
    {
        agent = context.gameObject.GetComponent<NavMeshAgent>();
    }

    protected override void OnStop() // ✅ 누락된 OnStop() 추가 (에러 해결)
    {
        // 이동이 멈출 때 추가할 동작이 있으면 여기에 작성 가능
        agent.ResetPath(); // ✅ 이동 경로 초기화 (안전장치)
    }

    protected override State OnUpdate()
    {
        if (blackboard.enemyTarget == null)
        {
            return State.Failure;
        }

        float distance = Vector3.Distance(context.transform.position, blackboard.enemyTarget.position);

        // ✅ 적과 너무 가까우면 후퇴
        if (distance < preferredDistance - tolerance)
        {
            return MoveAwayFromTarget();
        }

        // ✅ 적과 너무 멀면 접근
        if (distance > preferredDistance + tolerance)
        {
            return MoveCloserToTarget();
        }

        // ✅ 적절한 거리 유지 중이면 성공 반환
        return State.Success;
    }

    private State MoveAwayFromTarget()
    {
        Vector3 direction = (context.transform.position - blackboard.enemyTarget.position).normalized;

        // ✅ 후퇴할 때 랜덤 방향 적용 (일직선 후퇴 방지)
        direction = Quaternion.Euler(0, Random.Range(-30, 30), 0) * direction;

        Vector3 newPosition = context.transform.position + direction * (preferredDistance - tolerance);

        if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            return State.Failure;
        }

        return State.Running;
    }

    private State MoveCloserToTarget()
    {
        Vector3 direction = (blackboard.enemyTarget.position - context.transform.position).normalized;
        Vector3 newPosition = context.transform.position + direction * (preferredDistance - tolerance);

        if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            return State.Failure;
        }

        return State.Running;
    }
}
