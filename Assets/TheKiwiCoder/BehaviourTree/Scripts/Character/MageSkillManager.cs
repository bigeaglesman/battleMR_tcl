using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSkillManager : MonoBehaviour
{
    public enum MageType { Hero, Normal }
    public MageType mageType = MageType.Normal;

    [Header("Casting Settings")]
    [SerializeField] private float castRadius = 10f;
    [SerializeField] private float castDuration = 5f;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpawnHeight = 8f;

    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownDuration = 10f; // ✅ 인스펙터에서 수정 가능
    private float lastCastTime = -999f;

    private static bool isCasting = false;
    public static Vector3? castingHeroPosition = null;

    public bool IsCooldownReady()
    {
        bool ready = Time.time >= lastCastTime + cooldownDuration;
        Debug.Log($"🧪 [{gameObject.name}] 쿨타임 체크: {(ready ? "Ready" : "Not Ready")} ({Time.time - lastCastTime:F2}s 경과)");
        return ready;
    }

    public void StartCooldown()
    {
        lastCastTime = Time.time;
        Debug.Log($"⏱️ [{gameObject.name}] 쿨타임 시작됨: {lastCastTime:F2}");
    }

    public static bool IsHeroCastingNearby(Vector3 unitPosition, float radius = 12f)
    {
        if (!isCasting || castingHeroPosition == null) return false;
        float dist = Vector3.Distance(unitPosition, castingHeroPosition.Value);
        bool inRange = dist <= radius;
        Debug.Log($"📡 일반 마법사 거리 체크: {dist:F2}m → {(inRange ? "도움" : "무시")}");
        return inRange;
    }

    public void TryStartJointCast(Transform target)
    {
        if (mageType != MageType.Hero) return;
        if (isCasting || !IsCooldownReady())
        {
            Debug.Log($"🚫 [{gameObject.name}] 시전 불가: 쿨타임 중 또는 이미 시전 중");
            return;
        }

        List<MageSkillManager> participants = FindNearbyMages();
        Vector3 targetPosition = target != null ? target.position : transform.forward * 10f;

        Debug.Log($"🔥 [{gameObject.name}] 합동 시전 시작 - 참여자 수: {participants.Count}");
        StartCoroutine(JointCastRoutine(participants, targetPosition));
        StartCooldown();
    }

    List<MageSkillManager> FindNearbyMages()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, castRadius);
        List<MageSkillManager> mages = new List<MageSkillManager>();

        foreach (var col in colliders)
        {
            MageSkillManager mage = col.GetComponent<MageSkillManager>();
            if (mage != null && mage.enabled)
            {
                mages.Add(mage);
            }
        }

        return mages;
    }

    IEnumerator JointCastRoutine(List<MageSkillManager> participants, Vector3 targetPosition)
    {
        isCasting = true;
        castingHeroPosition = transform.position;

        foreach (var mage in participants)
        {
            mage.EnterCastState();
        }

        Debug.Log($"🌀 [{gameObject.name}] {castDuration:F1}초간 시전 중...");
        yield return new WaitForSeconds(castDuration);

        CreateFireball(targetPosition, participants.Count);

        foreach (var mage in participants)
        {
            mage.ExitCastState();
        }

        isCasting = false;
        castingHeroPosition = null;

        Debug.Log($"✅ [{gameObject.name}] 시전 종료");
    }

    void CreateFireball(Vector3 targetPosition, int mageCount)
    {
        Vector3 spawnPos = transform.position + Vector3.up * fireballSpawnHeight;
        GameObject fireball = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        float scaleMultiplier = 1f + 1f * (mageCount - 1);
        fireball.transform.localScale *= scaleMultiplier;

        FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
        if (projectile != null)
        {
            projectile.damage = Mathf.RoundToInt(projectile.damage * scaleMultiplier);
            projectile.SetShooter(this.transform);
            projectile.SetFragmentCountFromScale(scaleMultiplier);

            Collider fireballCollider = fireball.GetComponent<Collider>();
            if (fireballCollider != null)
            {
                foreach (var mage in FindNearbyMages())
                {
                    Collider mageCollider = mage.GetComponent<Collider>();
                    if (mageCollider != null)
                    {
                        Physics.IgnoreCollision(fireballCollider, mageCollider);
                    }
                }
            }

            Rigidbody rb = fireball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                Vector3 dir = (targetPosition - spawnPos).normalized;
                rb.linearVelocity = dir * 10f;
            }
        }

        Debug.Log($"💥 [{gameObject.name}] 파이어볼 발사! 크기 x{scaleMultiplier:F1}, 대상 위치: {targetPosition}");
    }

    void EnterCastState()
    {
        Debug.Log($"⏸️ [{gameObject.name}] 보조 유닛 시전 대기 상태 진입");
    }

    void ExitCastState()
    {
        Debug.Log($"▶️ [{gameObject.name}] 보조 유닛 행동 재개");
    }

    public static bool IsCasting()
    {
        return isCasting;
    }
}
