using TheKiwiCoder;
using UnityEngine;

public class SkillHeal : ActionNode
{
    public float skillCooldown = 10f;
    private float lastSkillTime = -Mathf.Infinity;
    public GameObject healEffectPrefab;

    private UnitAnimator animator;


    protected override void OnStart() {
        animator = context.gameObject.GetComponent<UnitAnimator>();
    }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        // 쿨타임 안 됐으면 실행하지 않음
        if (Time.time - lastSkillTime < skillCooldown)
            return State.Failure;

        animator.PlaySkill();
        HealAllies();
        
        lastSkillTime = Time.time;
        return State.Success;

    }
    /*
    private void HealAllies()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");

        foreach (GameObject ally in allies)
        {
            Health health = ally.GetComponent<Health>();
            if (health != null)
            {
                float healAmount = 50f;
                health.currentHealth = Mathf.Min(health.currentHealth + healAmount, health.maxHealth);

                if (health.healthBar != null)
                {
                    float percentage = health.currentHealth / health.maxHealth;
                    health.healthBar.SetHealthBarPercentage(percentage);
                }

                //Debug.Log($"{ally.name} 힐 받음. 현재 체력: {health.currentHealth}");
            }
        }

        //Debug.Log("힐 스킬 사용 완료!");
    }*/
    private void HealAllies()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");

        foreach (GameObject ally in allies)
        {
            Health health = ally.GetComponent<Health>();
            if (health != null)
            {
                float healAmount = 50f;
                health.currentHealth = Mathf.Min(health.currentHealth + healAmount, health.maxHealth);

                if (health.healthBar != null)
                {
                    float percentage = health.currentHealth / health.maxHealth;
                    health.healthBar.SetHealthBarPercentage(percentage);
                }

                // ✅ 힐 이펙트 생성
                if (healEffectPrefab != null)
                {
                    GameObject effect = GameObject.Instantiate(healEffectPrefab, ally.transform.position, Quaternion.identity);
                    effect.transform.SetParent(ally.transform); // 이동에 따라다니게
                    GameObject.Destroy(effect, 2f);
                }
            }
        }
    }
}
