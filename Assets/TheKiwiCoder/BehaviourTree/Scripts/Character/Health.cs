using Fusion;
using Oculus.Interaction;
using System.Collections;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public NetworkRunner runner;

    public AllyUnitManager unitManager;

    public float maxHealth = 100f;
    [Networked] public float currentHealth { get; set; }

    public UIHealthBar healthBar; // 유닛 위에 표시되는 체력바 (World Space Canvas)

    public float lastDamageTime = -10f; // 마지막으로 피해 받은 시간
    public float combatDuration = 5f;   // 전투 유지 시간 (초)

    private UnitAnimator animator;

    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            runner = FindObjectOfType<NetworkRunner>();
            currentHealth = maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.SetHealthBarPercentage(1.0f); // 처음엔 체력 100%
        }
        else
        {
            //Debug.LogWarning($"[{gameObject.name}] healthBar 연결 안 됨");
        }
    }
    private void Start()
    {
        animator = gameObject.GetComponent<UnitAnimator>();

    }
    private void Update()
    {
        if (healthBar != null)
        {
            float percentage = currentHealth / maxHealth;
            healthBar.SetHealthBarPercentage(percentage);
        }
        
    }

    public void TakeDamage(float amount)
    {
        if (!HasStateAuthority)
            return;

        currentHealth -= amount;

        if (currentHealth < 0f)
            currentHealth = 0f;

        //Debug.Log($"{gameObject.name} 체력: {currentHealth}");


        lastDamageTime = Time.time; // 전투 시간 업데이트

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(int amount)
    {
        TakeDamage(amount);
    }

    public bool IsInCombat()
    {
        return (Time.time - lastDamageTime) < combatDuration;
    }

    void Die()
    {
        animator.PlayDie();
        StartCoroutine(DieDelay());
        unitManager.NotifyUnitDeath(gameObject);
        
        //Debug.Log($"{gameObject.name}이(가) 죽었습니다.");
        //Destroy(gameObject);
    }

    IEnumerator DieDelay()
    {
        yield return new WaitForSeconds(2f);
        runner.Despawn(Object);
    }
}
