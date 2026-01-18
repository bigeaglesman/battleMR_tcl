using UnityEngine;
using System.Collections;

namespace TheKiwiCoder
{
    public class SkillWarrior : ActionNode
    {
        [Header("스킬 설정")]
        public float skillCooldown = 10f;
        private float lastSkillTime;

        [Header("버프 효과")]
        public int bonusAttackDamage = 20;
        public float bonusAttackSpeed = -0.5f;
        public float bonusHealth = 50f;
        public float buffDuration = 10f;

        private bool isBuffActive = false;
        private float buffEndTime;

        private UnitStats stats;
        private Health health;

        private Vector3 originalScale;
        public Vector3 buffedScale = new Vector3(1.5f, 1.5f, 1.5f);
        public float scaleDuration = 0.3f;

        public GameObject particle;
        private GameObject active_Particle;

        private UnitAnimator animator;

        protected override void OnStart()
        {
            if (stats == null)
                stats = context.gameObject.GetComponent<UnitStats>();

            if (health == null)
                health = context.gameObject.GetComponent<Health>();

            if(animator == null)
                animator = context.gameObject.GetComponent<UnitAnimator>();
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (isBuffActive && Time.time >= buffEndTime)
            {
                //Debug.Log($"🛑 [{context.gameObject.name}] 전사 버프 종료됨");
                RemoveBuff();
                lastSkillTime = Time.time;
            }

            if (!isBuffActive && Time.time > lastSkillTime + skillCooldown)
            {
                //Debug.Log($"💪 [{context.gameObject.name}] 전사 스킬 발동!");
                ApplyBuff();
                buffEndTime = Time.time + buffDuration;
                return State.Success;
            }

            return State.Failure;
        }

        private void ApplyBuff()
        {
            // ✅ 현재 스케일을 실제 버프 적용 직전에 저장
            originalScale = context.transform.localScale;

            active_Particle = Instantiate(particle, context.transform.position, Quaternion.identity);

            if (stats != null)
            {
                stats.currentAttackDamage += bonusAttackDamage;
                stats.currentAttackCooldown += bonusAttackSpeed;
                //Debug.Log($"▶️ 공격력 +{bonusAttackDamage}, 쿨타임 {bonusAttackSpeed}");
            }

            if (health != null)
            {
                health.maxHealth += bonusHealth;
                health.currentHealth += bonusHealth;

                if (health.healthBar != null)
                    health.healthBar.SetHealthBarPercentage(health.currentHealth / health.maxHealth);

                //Debug.Log($"▶️ 체력 +{bonusHealth} → 현재 체력: {health.currentHealth}");
            }


            if(animator != null)
            {
                animator.PlaySkill();
            }
            else
            {
                Debug.Log("SkillWarrior: No Animator");
            }

            isBuffActive = true;

            CoroutineRunner.Instance.StartCoroutine(
                AnimateScale(context.transform, originalScale, buffedScale, scaleDuration));
        }

        private void RemoveBuff()
        {
            if (active_Particle != null)
            {
                Destroy(active_Particle);
                active_Particle = null;
            }

            if (stats != null)
            {
                stats.currentAttackDamage -= bonusAttackDamage;
                stats.currentAttackCooldown -= bonusAttackSpeed;
                //Debug.Log($"⏹️ 공격력 원상복구, 쿨타임 원상복구");
            }

            if (health != null)
            {
                health.maxHealth -= bonusHealth;

                if (health.currentHealth > health.maxHealth)
                    health.currentHealth = health.maxHealth;

                if (health.healthBar != null)
                    health.healthBar.SetHealthBarPercentage(health.currentHealth / health.maxHealth);

                //Debug.Log($"⏹️ 체력 원상복구 → 현재 체력: {health.currentHealth}");
            }

            isBuffActive = false;

            CoroutineRunner.Instance.StartCoroutine(
                AnimateScale(context.transform, context.transform.localScale, originalScale, scaleDuration));

            
        }

        private IEnumerator AnimateScale(Transform target, Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                target.localScale = Vector3.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            target.localScale = to;
        }
    }
}
