using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Transform target;
    public Slider healthSlider;  // Slider 컴포넌트
    public Vector3 offset;

    void Start()
    {
        if (target == null)
        {
            if (transform.parent != null)
            {
                target = transform.parent;
            }
            else
            {
                Debug.LogError("[UIHealthBar] 부모(유닛)를 찾을 수 없습니다! target을 수동으로 설정하세요.");
            }
        }

        // offset은 프리팹에서 직접 설정한 값 사용
        // 자동 계산은 제거
    }

    void LateUpdate()
    {
        if (target == null || Camera.main == null) return;

        // 유닛 위에 체력바 위치 조정
        transform.position = target.position + offset;

        // 항상 카메라 방향 바라보게
        //transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        /*
        // 카메라를 바라보되, 수평 회전만 반영 (Y축만 회전)
        Vector3 dir = transform.position - Camera.main.transform.position;
        dir.y = 0f; // 위아래 각도 제거
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
        */
        

        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                 Camera.main.transform.rotation * Vector3.up);

    }

    public void SetHealthBarPercentage(float percentage)
    {
        if (healthSlider != null)
        {
            healthSlider.value = percentage;
        }
    }
}
