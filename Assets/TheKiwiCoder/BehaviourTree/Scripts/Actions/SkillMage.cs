using UnityEngine;
using TheKiwiCoder;

public class SkillMage : ActionNode
{
    private MageSkillManager manager;
    private bool hasStarted = false;
    private UnitAnimator animator;


    protected override void OnStart()
    {
        hasStarted = false;
        manager = context.gameObject.GetComponent<MageSkillManager>();
        animator = context.gameObject.GetComponent<UnitAnimator>();

        Debug.Log($"[SkillMage] OnStart - 유닛: {context.gameObject.name}");
    }

    protected override void OnStop()
    {
        Debug.Log($"[SkillMage] OnStop - 유닛: {context.gameObject.name}");
    }

    protected override State OnUpdate()
    {
        if (manager == null)
        {
            Debug.LogWarning("[SkillMage] MageSkillManager 없음");
            return State.Failure;
        }

        // 일반 마법사
        if (manager.mageType == MageSkillManager.MageType.Normal)
        {
            if (MageSkillManager.IsHeroCastingNearby(context.transform.position))
            {
                Debug.Log($"[SkillMage] 일반 마법사 {context.gameObject.name} 보조 중");
                return State.Running;
            }
            else
            {
                Debug.Log($"[SkillMage] 일반 마법사 {context.gameObject.name} 보조 아님");
                return State.Failure;
            }
        }

        // 영웅 마법사
        if (blackboard.enemyTarget == null)
        {
            Debug.Log($"[SkillMage] {context.gameObject.name} 타겟 없음");
            return State.Failure;
        }

        if (MageSkillManager.IsCasting())
        {
            Debug.Log($"[SkillMage] {context.gameObject.name} 시전 중 대기");
            return State.Running;
        }

        if (!hasStarted && manager.IsCooldownReady())
        {
            Debug.Log($"[SkillMage] {context.gameObject.name} 스킬 시전 시작");
            animator.PlaySkill(5f);

            manager.TryStartJointCast(blackboard.enemyTarget);
            hasStarted = true;
            return State.Running;
        }

        if (!manager.IsCooldownReady())
        {
            Debug.Log($"[SkillMage] {context.gameObject.name} 쿨타임 중");
            return State.Failure;
        }

        Debug.Log($"[SkillMage] {context.gameObject.name} 시전 완료");
        

        return hasStarted ? State.Success : State.Failure;
    }
}
