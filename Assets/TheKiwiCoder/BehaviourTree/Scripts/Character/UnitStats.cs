using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public enum Stats
    {
        Idle,
        Run,
        Attack
    }

    [Header("현재 유닛 상태 (자동 변경됨)")]
    public Stats unitStats= Stats.Idle;

    [Header("현재 타겟 정보")]
    public Transform currentTarget;
    public string currentTargetName;

    [Header("기본 능력치")]
    public int baseAttackDamage = 10;
    public float baseAttackCooldown = 1.0f;

    [Header("현재 능력치 (자동 세팅)")]
    public int currentAttackDamage;
    public float currentAttackCooldown;

    void Awake()
    {
        currentAttackDamage = baseAttackDamage;
        currentAttackCooldown = baseAttackCooldown;
    }
}
