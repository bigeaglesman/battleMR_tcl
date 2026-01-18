using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float damageRadius = 3.0f;
    public int damage = 50;
    public float lifeTime = 5f;
    public string targetTag = "Enemy";

    [Header("Explosion Fragment Settings")]
    public GameObject fragmentPrefab;     // ✅ 큐브 프리팹
    public int fragmentCount = 10;        // ✅ 초기값 (스케일에 따라 조정됨)
    public float fragmentForce = 3f;      // ✅ 튀어 나가는 힘
    public float fragmentDamage = 2f;     // ✅ 큐브 데미지 (기본값)

    private bool hasExploded = false;
    private Transform shooterTransform;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetShooter(Transform shooter)
    {
        shooterTransform = shooter;

        Collider myCollider = GetComponent<Collider>();
        Collider shooterCollider = shooter.GetComponent<Collider>();

        if (myCollider && shooterCollider)
        {
            Physics.IgnoreCollision(myCollider, shooterCollider);
        }
    }

    // ✅ 외부에서 파편 개수를 설정
    public void SetFragmentCountFromScale(float scaleMultiplier)
    {
        fragmentCount = Mathf.RoundToInt(10 * scaleMultiplier);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;
        hasExploded = true;

        DealAOEDamage();
        SpawnFragments();

        Destroy(gameObject);
    }

    void DealAOEDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag(targetTag)) continue;

            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

    void SpawnFragments()
    {
        if (fragmentPrefab == null) return;

        for (int i = 0; i < fragmentCount; i++)
        {
            Vector3 dir = Random.onUnitSphere;
            Vector3 spawnPos = transform.position + dir * 0.5f;
            GameObject fragment = Instantiate(fragmentPrefab, spawnPos, Quaternion.identity);

            // ✅ 랜덤 크기 부여
            float scale = Random.Range(0.3f, 0.7f);
            fragment.transform.localScale = Vector3.one * scale;

            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(dir * fragmentForce, ForceMode.Impulse);
            }

            FragmentDamage frag = fragment.GetComponent<FragmentDamage>();
            if (frag != null)
            {
                frag.damage = Random.Range(2, 4); // ✅ 2~3 데미지
                frag.targetTag = targetTag;
            }

            Destroy(fragment, 3f);
        }
    }
}
