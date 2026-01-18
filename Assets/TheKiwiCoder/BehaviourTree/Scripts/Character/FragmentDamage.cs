using UnityEngine;

public class FragmentDamage : MonoBehaviour
{
    public int damage = 2;
    public string targetTag = "Enemy";

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Health hp = collision.gameObject.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            Destroy(gameObject); // 1회 충돌 후 파괴
        }
    }
}
