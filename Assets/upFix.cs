using UnityEngine;

public class upFix : MonoBehaviour
{
    private Quaternion fixedRotation;

    void Start()
    {
        fixedRotation = transform.rotation; // 초기 회전값 저장
    }

    void Update()
    {
        transform.rotation = fixedRotation; // 회전값을 고정
    }
}
