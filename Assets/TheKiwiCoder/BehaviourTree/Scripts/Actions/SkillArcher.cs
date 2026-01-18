using UnityEngine;
using TheKiwiCoder;
using System.Collections;

public class SkillArcher : ActionNode
{
    public GameObject skillProjectilePrefab;
    public float skillCooldown = 3f;
    private float lastSkillTime = -999f;

    public float radius = 15f;
    public int circleCount = 2; // 원 개수
    public int arrowsPerCircle = 10; // 원당 화살 수
    public float height = 6f;
    public float scatterRange = 1.0f;
    public float delayBetweenArrows = 0.05f; // 연사 간격

    private UnitAnimator animator;

    private bool isFiring = false;

    protected override void OnStart() {
        animator = context.gameObject.GetComponent<UnitAnimator>();
    }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        if (blackboard.enemyTarget == null || isFiring)
            return State.Failure;

        if (Time.time < lastSkillTime + skillCooldown)
            return State.Failure;


        animator.PlaySkill();
        isFiring = true;
        lastSkillTime = Time.time;

        Vector3 targetPos = blackboard.enemyTarget.position;
        CoroutineRunner.Instance.StartCoroutine(RainOfArrows(targetPos));
        

        return State.Success;
    }

    IEnumerator RainOfArrows(Vector3 targetPosition)
    {
        //Debug.Log("🌧️ 화살비 발사 시작");

        for (int c = 1; c <= circleCount; c++)
        {
            float currentRadius = (c / (float)circleCount) * radius;

            for (int i = 0; i < arrowsPerCircle; i++)
            {
                float angle = i * Mathf.PI * 2f / arrowsPerCircle;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * currentRadius;

                Vector3 randomOffset = new Vector3(
                    Random.Range(-scatterRange, scatterRange),
                    0,
                    Random.Range(-scatterRange, scatterRange)
                );

                Vector3 spawnPos = context.transform.position + Vector3.up * 1.5f;
                Vector3 landingPos = targetPosition + offset + randomOffset;
                landingPos.y = targetPosition.y;

                GameObject arrow = GameObject.Instantiate(skillProjectilePrefab, spawnPos, Quaternion.FromToRotation(Vector3.up, Vector3.forward));
                Rigidbody rb = arrow.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 velocity = CalculateParabolicVelocity(spawnPos, landingPos, height);
                    if (!float.IsNaN(velocity.x))
                    {
                        rb.linearVelocity = velocity;
                    }

                    RainOfArrowsProjectile arrowScript = arrow.GetComponent<RainOfArrowsProjectile>();
                    if (arrowScript != null)
                    {
                        arrowScript.SetShooter(context.transform);
                    }

                    //Debug.Log($"➡️ 화살 발사 (원 {c}/{circleCount}, 화살 {i + 1}/{arrowsPerCircle}) 위치: {landingPos}");
                }

                yield return new WaitForSeconds(delayBetweenArrows);
            }
        }

        isFiring = false;
    }

    Vector3 CalculateParabolicVelocity(Vector3 start, Vector3 end, float height)
    {
        Vector3 displacement = end - start;
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);
        float sy = displacement.y;
        float g = -Physics.gravity.y;

        float timeUp = Mathf.Sqrt(2 * height / g);
        float timeDown = Mathf.Sqrt(2 * Mathf.Max(0.01f, height - sy) / g);
        float totalTime = timeUp + timeDown;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * g * height);
        Vector3 velocityXZ = displacementXZ / totalTime;

        return velocityXZ + velocityY;
    }
}
