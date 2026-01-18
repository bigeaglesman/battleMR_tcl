using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMT : ActionNode
{
    private NavMeshAgent agent;
    private float updateInterval = 1.5f;
    private float lastUpdateTime;

    protected override void OnStart()
    {
        agent = context.gameObject.GetComponent<NavMeshAgent>();

        if (blackboard.allyTarget == null)
        {
            //Debug.LogWarning("❌ 이동 실패: 대상 없음");
            return;
        }

        agent.stoppingDistance = blackboard.attackRange;
        lastUpdateTime = Time.time;

        //Debug.Log("🚶‍♂️ 아군을 향해 이동 시작...");
        agent.SetDestination(blackboard.allyTarget.position);
    }

    protected override void OnStop()
    {
        if (agent.hasPath)
        {
            agent.ResetPath();
        }
        //Debug.Log("🛑 이동 중단");
    }

    protected override State OnUpdate()
    {
        if (blackboard.allyTarget == null)
        {
            //Debug.Log("❌ 이동 실패: 타겟이 사라짐");
            return State.Failure;
        }

        float distance = Vector3.Distance(context.transform.position, blackboard.allyTarget.position);

        if (distance <= agent.stoppingDistance)
        {
            //Debug.Log("✅ 목표 도착! 이동 완료.");
            return State.Success;
        }

        if (Time.time - lastUpdateTime > updateInterval)
        {
            agent.SetDestination(blackboard.allyTarget.position);
            lastUpdateTime = Time.time;
        }

        return State.Running;
    }
}
