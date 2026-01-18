using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public float lifetime = 5f;

    private Transform target; // 🎯 적 Transform

    public GameObject particle;
    
    void Start()
    {
        Destroy(gameObject, lifetime); // 수명 끝나면 제거
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z); // Y축 유지
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            transform.LookAt(targetPos); // 시선도 계속 타겟 향함

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                HitTarget();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }

    void HitTarget()
    {
        if (target != null)
        {
            Health health = target.GetComponent<Health>();
            if (health != null)
            {
                if (particle != null)
                {
                    Instantiate(particle, target.position, Quaternion.identity);
                }
                health.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == target)
        {
            HitTarget();
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
