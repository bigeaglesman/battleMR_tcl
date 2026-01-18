using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace TheKiwiCoder
{
    // 전사 유닛의 공격 동작을 정의하는 행동 트리 노드
    public class WarriorAttack : ActionNode
    {
        private float lastAttackTime;      // 마지막 공격 시간
        private UnitAnimator animator;
        // 애니메이션 제어용 컴포넌트
        private UnitStats stats;           // 유닛의 스탯 정보
        private bool attacked = true;  //공격 했는지 기록. - 현수

        public GameObject particle;
        // 노드가 처음 시작될 때 호출됨
        protected override void OnStart()
        {
            if (animator == null)
            {
                animator = context.gameObject.GetComponent<UnitAnimator>();
            }
            //if (animator != null)
            //{
            //    animator.enabled = false;
            //}

            if (stats == null)
            {
                stats = context.gameObject.GetComponent<UnitStats>();
            }

            //Debug.Log("[WarriorAttack] OnStart: 초기화 완료");
        }

        // 노드가 중단될 때 호출됨
        protected override void OnStop()
        {
            //if (animator != null)
            //{
            //    animator.enabled = true;
            //}

            //Debug.Log("[WarriorAttack] OnStop: 애니메이터 활성화");
        }

        // 매 프레임마다 호출되며 행동의 결과 상태를 반환
        protected override State OnUpdate()
        {
            if (blackboard.enemyTarget == null)
            {
                //Debug.Log("[WarriorAttack] 실패: 타겟이 없음");
                return State.Failure;
            }

            float distance = Vector3.Distance(context.transform.position, blackboard.enemyTarget.position);
            if (distance > blackboard.attackRange)
            {
                //Debug.Log($"[WarriorAttack] 실패: 타겟이 사거리 밖 (거리: {distance}, 사거리: {blackboard.attackRange})");
                return State.Failure;
            }

            Health targetHealth = blackboard.enemyTarget.GetComponent<Health>();
            if (targetHealth == null || targetHealth.currentHealth <= 0)
            {
                //Debug.Log("[WarriorAttack] 실패: 타겟의 체력이 0이거나 Health 컴포넌트 없음");
                blackboard.enemyTarget = null;
                return State.Failure;
            }

            if (Time.time > lastAttackTime + stats.currentAttackCooldown*0.5)   //0.5에 공격하도록 수정.-현수
            {
                attacked = !attacked;
                if (!attacked)   //요기랑 아래괄호 열기,닫기 0.5 마다 공격해야하니 0:true니 공격x 0.5:false니 공격 - 현수
				{
                    //Debug.Log("[WarriorAttack] 공격 수행!");
                    PerformAttack(targetHealth);
                    return State.Running;
                }
            }

            //Debug.Log("[WarriorAttack] 대기 중: 공격 쿨타임 진행 중");
            return State.Running;
        }

        // 실제 공격을 수행하는 함수
        private void PerformAttack(Health target)
        {
            animator.PlayAttack();
            lastAttackTime = Time.time;
            Instantiate(particle, target.transform.position, Quaternion.identity);
            target.TakeDamage(stats.currentAttackDamage);

            //Debug.Log($"[WarriorAttack] 대상에게 데미지 {stats.currentAttackDamage} 가함");

            Health myHealth = context.gameObject.GetComponent<Health>();
            if (myHealth != null)
            {
                myHealth.lastDamageTime = Time.time;
                //Debug.Log("[WarriorAttack] 내 lastDamageTime 갱신");
            }

            

            //if (animator != null)
            //{
            //    animator.enabled = true;
            //    //Debug.Log("[WarriorAttack] 애니메이터 활성화");
            //}
        }
    }
}
