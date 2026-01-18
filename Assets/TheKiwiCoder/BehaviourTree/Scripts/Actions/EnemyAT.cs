using UnityEngine;

namespace TheKiwiCoder
{
    public class EnemyAT : ActionNode
    {
        private float lastAttackTime;
        public float attackCooldown = 1.5f;
        public int attackDamage = 10;

        protected override void OnStart()
        {
            //Debug.Log("🔹 아군 공격 시작");
        }

        protected override void OnStop()
        {
            //Debug.Log("🔹 아군 공격 중단");
        }

        protected override State OnUpdate()
        {
            if (blackboard.allyTarget == null)
            {
                //Debug.LogWarning("❌ 공격 실패: 대상 없음");
                return State.Failure;
            }

            float distance = Vector3.Distance(context.transform.position, blackboard.allyTarget.position);
            if (distance > blackboard.attackRange)
            {
                //Debug.LogWarning("❌ 공격 실패: 사거리 밖 → 추격");
                return State.Failure;
            }

            Health targetHealth = blackboard.allyTarget.GetComponent<Health>();
            if (targetHealth == null || targetHealth.currentHealth <= 0)
            {
                //Debug.LogWarning("❌ 공격 실패: 대상 사망");
                blackboard.allyTarget = null;
                return State.Failure;
            }

            if (Time.time > lastAttackTime + attackCooldown)
            {
                PerformAttack(targetHealth);
                return State.Success;
            }

            return State.Running;
        }

        private void PerformAttack(Health target)
        {
            lastAttackTime = Time.time;
            target.TakeDamage(attackDamage);

            /*
            //자신도 전투 상태로 진입
            Health selfHealth = context.gameObject.GetComponent<Health>();
            if (selfHealth != null)
            {
                selfHealth.lastDamageTime = Time.time;
            }*/

            //Debug.Log($"🔥 {blackboard.allyTarget.name} 체력 감소: {attackDamage}");
        }
    }
}
