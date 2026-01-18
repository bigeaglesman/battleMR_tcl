using UnityEngine;

namespace TheKiwiCoder
{
    public class Attack : ActionNode
    {
        private float lastAttackTime;
        public float attackCooldown = 1.5f;
        public int attackDamage = 10;
        private UnitAnimator animator;

        protected override void OnStart()
        {
            //Debug.Log("🔹 공격 시작");

            // Animator import
            if (animator == null)
            {
                animator = context.gameObject.GetComponent<UnitAnimator>();
            }

            //if (animator != null)
            //{
            //    animator.enabled = false;
            //}
        }

        protected override void OnStop()
        {
            //Debug.Log("🔹 공격 중단");

            //✅ Animator가 존재할 경우만 활성화
            //if (animator != null)
            //{
            //    animator.enabled = true;
            //}
        }

        protected override State OnUpdate()
        {
            if (blackboard.enemyTarget == null)  // ✅ 기존 blackboard.target → blackboard.enemyTarget 변경
            {
                //Debug.LogWarning("❌ 공격 실패: 대상 없음");
                return State.Failure;
            }

            float distance = Vector3.Distance(context.transform.position, blackboard.enemyTarget.position);
            if (distance > blackboard.attackRange)
            {
                //Debug.LogWarning("❌ 공격 실패: 사거리 밖 → 추격");
                return State.Failure;
            }

            Health targetHealth = blackboard.enemyTarget.GetComponent<Health>();
            if (targetHealth == null || targetHealth.currentHealth <= 0)
            {
                //Debug.LogWarning("❌ 공격 실패: 대상 사망");
                blackboard.enemyTarget = null;
                return State.Failure;
            }

            if (Time.time > lastAttackTime + attackCooldown)
            {
                PerformAttack(targetHealth);
                return State.Running;
            }

            return State.Running;
        }

        private void PerformAttack(Health target)
        {
            lastAttackTime = Time.time;
            target.RPC_TakeDamage(attackDamage);
            //Debug.Log($"🔥 {blackboard.enemyTarget.name} 체력 감소: {attackDamage}");
            animator.PlayAttack();

            // ✅ 공격할 때도 전투 상태 유지 (전사 & 궁수 모두 적용)
            Health myHealth = context.gameObject.GetComponent<Health>();
            if (myHealth != null)
            {
                myHealth.lastDamageTime = Time.time;  // ✅ 공격을 하면 전투 상태로 유지됨
            }
            


            // ✅ 공격이 끝나면 Animator 다시 활성화
            //if (animator != null)
            //{
            //    animator.enabled = true;
            //}
        }
    }
}
