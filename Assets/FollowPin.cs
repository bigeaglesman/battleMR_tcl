using UnityEngine;

public class FollowPin : MonoBehaviour
{
    public GameObject targetA; // 따라갈 대상 (A)
    private Vector3 offset; // 초기 거리(오프셋) 저장

void Start()
{
    if (targetA != null)
    {
        targetA.transform.SetParent(null); // B를 부모(A)에서 분리
        offset = transform.position - targetA.transform.position; // 초기 거리 저장
    }
}

    void LateUpdate()
    {
        if (targetA != null)
        {
			Quaternion targetRoatation = Quaternion.Euler(0, targetA.transform.eulerAngles.y, 0);
			transform.rotation = targetRoatation;
			transform.position = targetA.transform.position + offset; // 초기 거리 유지하면서 이동
        }
    }

	void OnDestroy()
	{
		Destroy(targetA);	
	}
}
