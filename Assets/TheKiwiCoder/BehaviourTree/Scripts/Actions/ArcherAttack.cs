using TheKiwiCoder;
using UnityEngine;

public class ArcherAttack : ActionNode
{
    public GameObject projectilePrefab; // ✅ 투사체 프리팹
    public float attackCooldown = 1.5f; // ✅ 공격 쿨타임
    private float lastAttackTime = -999f; // ✅ 초기화
    private UnitAnimator animator;
    

    protected override void OnStart()
    {
        // 초기화 로직 필요 시 추가 가능
        animator = context.gameObject.GetComponent<UnitAnimator>();
    }

    protected override void OnStop()
    {
        // 공격 중단 로직 필요 시 추가 가능
        
    }

    protected override State OnUpdate()
    {
        if (blackboard.enemyTarget == null)
        {
            return State.Failure;
        }

        float distance = Vector3.Distance(context.transform.position, blackboard.enemyTarget.position);
        if (distance > blackboard.attackRange)
        {
            return State.Failure;
        }

        // ✅ 쿨타임이 지나지 않았으면 Running 상태 유지 (추가)
        if (Time.time < lastAttackTime + attackCooldown)
        {
            return State.Running;
        }

        // ✅ 투사체 발사
        FireProjectile();
        lastAttackTime = Time.time; // ✅ 쿨타임 갱신
        return State.Success;
    }

    private void FireProjectile()
    {
        if (projectilePrefab != null)
        {
            Vector3 spawnPosition = context.transform.position + Vector3.up * 1.5f;

            GameObject projectile = GameObject.Instantiate(projectilePrefab, spawnPosition, Quaternion.FromToRotation(Vector3.up, Vector3.forward));
            Projectile projScript = projectile.GetComponent<Projectile>();

            if (projScript != null)
            {
                animator.PlayAttack();
                //Vector3 targetPosition = blackboard.enemyTarget.position + Vector3.up * 2.4f;
                //projScript.SetTarget(blackboard.enemyTarget.position);
                projScript.SetTarget(blackboard.enemyTarget);
                
            }
        }
        //Debug.Log($"🏹 {context.gameObject.name} 투사체 발사!");
    }
}
