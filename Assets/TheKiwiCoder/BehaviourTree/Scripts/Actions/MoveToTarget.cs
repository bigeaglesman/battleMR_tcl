using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;

public class MoveToTarget : ActionNode
{
    private NavMeshAgent agent;
    private float updateInterval = 0.5f;
    private float lastUpdateTime;
    private int i = 0;
    private int id = 0; //Todo 외부or어딘가에 id라는 값을 소환시에 저장. 소환시 0부터 증가하는 인덱스를 3으로 나눈값.
    private UnitAnimator animator;
    protected override void OnStart()
    {
        agent = context.gameObject.GetComponent<NavMeshAgent>();
        animator = context.gameObject.GetComponent<UnitAnimator>();

        Transform target = GetTarget(); // ✅ 이동 대상 자동 선택
        if (target == null)
        {
            //Debug.LogWarning("❌ [MoveToTarget] 이동 실패: 대상 없음");
            return;
        }

        agent.stoppingDistance = blackboard.attackRange;
        lastUpdateTime = Time.time;

        //Debug.Log($"🚶‍♂️ [MoveToTarget] {target.name}을 향해 이동 시작...");
        animator.PlayRun();
        agent.SetDestination(target.position);
    }

    protected override void OnStop()
    {
        if (agent.hasPath)
        {
            agent.ResetPath();
        }
        //Debug.Log("🛑 [MoveToTarget] 이동 중단");
        animator.StopRun();
    }

    protected override State OnUpdate()
    {
        Transform target = GetTarget(); // ✅ 최신 목표 확인
        if (target == null)
        {
            //Debug.Log("❌ [MoveToTarget] 이동 실패: 타겟이 사라짐");
            return State.Failure;
        }





        if (Time.time - lastUpdateTime > updateInterval)
        {
            i++;
            if (i >= 3) i = 0;
            if (i == id)
            {
                float distance = Vector3.Distance(context.transform.position, target.position);
                if (distance <= agent.stoppingDistance)
                {
                    //Debug.Log($"✅ [MoveToTarget] {target.name}에게 도착! 이동 완료.");
                    return State.Success;
                }
                //Debug.Log($"📏 [MoveToTarget] 거리 확인: {distance}m (목표: {target.name}, 사거리: {agent.stoppingDistance}m)");
                agent.SetDestination(target.position);
            }
            lastUpdateTime = Time.time;
        }

        return State.Running;
    }

    private Transform GetTarget()
    {
        Transform target = blackboard.enemyTarget != null ? blackboard.enemyTarget : blackboard.allyTarget;
        if (target == null)
        {
            //Debug.LogWarning("⚠️ [MoveToTarget] 이동할 대상 없음!");
        }
        return target;
    }
}
