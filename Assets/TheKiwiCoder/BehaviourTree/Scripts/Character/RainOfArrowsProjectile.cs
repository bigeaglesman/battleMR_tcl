using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
public class RainOfArrowsProjectile : MonoBehaviour
{
    public float damageRadius = 1.5f;
    public int damage = 15;
    public float lifeTime = 5f;

    public Transform shooterTransform;
    public string targetTag = "Enemy";
    public GameObject particle;


    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetShooter(Transform shooter)
    {
        shooterTransform = shooter;

        Collider myCol = GetComponent<Collider>();
        if (myCol == null) return;

        // 1. 화살이 자신을 발사한 유닛과 충돌하지 않도록 설정
        Collider shooterCol = shooter.GetComponent<Collider>();
        if (shooterCol != null)
        {
            Physics.IgnoreCollision(myCol, shooterCol);
        }

        // 2. 이미 존재하는 화살들과도 충돌 무시
        Collider[] otherProjectiles = Object.FindObjectsByType<RainOfArrowsProjectile>(FindObjectsSortMode.None)
            .Where(p => p != this)
            .Select(p => p.GetComponent<Collider>())
            .Where(c => c != null)
            .ToArray();

        foreach (Collider col in otherProjectiles)
        {
            Physics.IgnoreCollision(myCol, col);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"💢 화살 착지! 위치: {transform.position}");
        DealAOEDamage();
        Destroy(gameObject);
    }

    void DealAOEDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (Collider hit in hits)
        {
            if (hit.transform.root == shooterTransform) continue;
            if (!hit.CompareTag(targetTag)) continue;

            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                if (particle != null)
                    Instantiate(particle, hit.transform.position, Quaternion.identity);

                health.TakeDamage(damage);
                //Debug.Log($"🩸 {hit.name} 에게 {damage} 데미지!");
            }
        }
    }
}
