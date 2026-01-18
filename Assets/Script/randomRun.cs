using UnityEngine;
using System.Collections;

public class randomRun : MonoBehaviour
{
public GameObject movePlane; // 이동할 평면 오브젝트 (Inspector에서 할당)
    public float moveSpeed = 3f; // 이동 속도
    public float rotationSpeed = 5f; // 회전 속도
    public float waitTime = 1.5f; // 이동 후 대기 시간

    private Vector3 planeMinBounds;
    private Vector3 planeMaxBounds;
    private Vector3 targetPosition;
    private bool isMoving = true;

    void Start()
    {
        if (movePlane != null)
        {
            GetPlaneBounds(); // 평면 크기 가져오기
            StartCoroutine(MoveRandomly());
        }
        else
        {
            Debug.LogError("이동할 평면을 설정하세요!");
        }
    }

	void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
			Destroy(gameObject);
	}

    void GetPlaneBounds()
    {
        // 평면이 BoxCollider를 가지고 있다면, Collider를 사용하여 크기 감지
        BoxCollider boxCollider = movePlane.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Vector3 center = boxCollider.bounds.center;
            Vector3 size = boxCollider.bounds.extents; // 절반 크기 (Extents 사용)
            planeMinBounds = new Vector3(center.x - size.x, transform.position.y, center.z - size.z);
            planeMaxBounds = new Vector3(center.x + size.x, transform.position.y, center.z + size.z);
            return;
        }

        // 만약 BoxCollider가 없으면 MeshRenderer를 사용하여 크기 감지
        MeshRenderer meshRenderer = movePlane.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Vector3 center = meshRenderer.bounds.center;
            Vector3 size = meshRenderer.bounds.extents; // 절반 크기
            planeMinBounds = new Vector3(center.x - size.x, transform.position.y, center.z - size.z);
            planeMaxBounds = new Vector3(center.x + size.x, transform.position.y, center.z + size.z);
            return;
        }

        Debug.LogError("이동할 평면에 BoxCollider 또는 MeshRenderer가 없습니다!");
    }

    IEnumerator MoveRandomly()
    {
        while (true)
        {
            targetPosition = GetRandomPointInPlane(); // 랜덤 위치 선정
            yield return StartCoroutine(SmoothTurnTowards(targetPosition)); // 자연스럽게 회전 후 이동

            // 목적지까지 이동
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // 목표 지점 도착 후 대기
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator SmoothTurnTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRotation; // 정확한 회전 보정
    }

    Vector3 GetRandomPointInPlane()
    {
        float randomX = Random.Range(planeMinBounds.x, planeMaxBounds.x);
        float randomZ = Random.Range(planeMinBounds.z, planeMaxBounds.z);
        return new Vector3(randomX, transform.position.y, randomZ);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) // 장애물과 충돌하면 새로운 랜덤 위치로 이동
        {
            StopCoroutine(MoveRandomly());
            StartCoroutine(MoveRandomly()); // 새로운 랜덤 위치 선택
        }
    }
}
